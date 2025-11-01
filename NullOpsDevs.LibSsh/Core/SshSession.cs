using System.Net.Sockets;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using NullOpsDevs.LibSsh.Credentials;
using NullOpsDevs.LibSsh.Exceptions;
using NullOpsDevs.LibSsh.Extensions;
using NullOpsDevs.LibSsh.Generated;
using NullOpsDevs.LibSsh.Interop;
using NullOpsDevs.LibSsh.Platform;
using NullOpsDevs.LibSsh.Terminal;
using static NullOpsDevs.LibSsh.Generated.LibSshNative;

namespace NullOpsDevs.LibSsh.Core;

/// <summary>
/// Represents an SSH session for connecting to and communicating with remote SSH servers.
/// </summary>
[PublicAPI]
public sealed class SshSession : IDisposable
{
    private static bool libraryInitialized;
    
#if NET9_0_OR_GREATER
    private readonly Lock localLock = new();
#else
    private readonly object localLock = new();
#endif

    private Socket? socket;
    private unsafe _LIBSSH2_SESSION* session;

    /// <summary>
    /// Gets the current connection status of the SSH session.
    /// </summary>
    public SshConnectionStatus ConnectionStatus { get; private set; }

    private static void EnsureInitialized()
    {
        lock (LibSsh2.GlobalLock)
        {
            if(libraryInitialized)
                return;
            
            LibSsh2.Log("Initializing library");

            var initResult = libssh2_init(0);
            LibSsh2.Log($"libssh2_init returned: {initResult}");
            
            initResult.ThrowIfNotSuccessful("LibSSH2 initialization failed");
            libraryInitialized = true;
        }
    }

    private void EnsureInStatus(SshConnectionStatus status)
    {
        if (ConnectionStatus != status)
            throw new SshException($"SshConnection must be in status '{status:G}' to perform that operation.", SshError.DevWrongUse);
    }
    
    /// <summary>
    /// Connects to an SSH server at the specified host and port.
    /// </summary>
    /// <param name="host">The hostname or IP address of the SSH server.</param>
    /// <param name="port">The port number of the SSH server (typically 22).</param>
    /// <exception cref="SshException">Thrown when connection or SSH handshake fails.</exception>
    /// <remarks>
    /// This method establishes a TCP connection and performs the SSH protocol handshake.
    /// After successful connection, the session will be in <see cref="SshConnectionStatus.Connected"/> status.
    /// You must call <see cref="Authenticate"/> before executing commands.
    /// </remarks>
    public unsafe void Connect(string host, int port)
    {
        EnsureInitialized();
        EnsureInStatus(SshConnectionStatus.Disconnected);

        lock (localLock)
        {
            var newSession = libssh2_session_init_ex(null, null, null, null);

            if (newSession == null)
                throw new SshException("Failed to initialize new session", SshError.FailedToInitializeSession);

            try
            {
                LibSsh2.Log($"Connecting to server: '{host}:{port}'...");
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(host, port);
            }
            catch (Exception ex)
            {
                _ = libssh2_session_free(newSession);
                throw ex.AsSshException();
            }

            LibSsh2.Log($"Connected to server: '{host}:{port}'");
            LibSsh2.Log("Handshaking...");

            var result = libssh2_session_handshake(newSession, (ulong)socket.Handle);

            result.ThrowIfNotSuccessful("Failed to handshake with server", also: () =>
            {
                _ = libssh2_session_free(newSession);
                socket.Dispose();
            });

            libssh2_session_set_blocking(newSession, 1);

            ConnectionStatus = SshConnectionStatus.Connected;
            session = newSession;
        }
    }

    /// <summary>
    /// Asynchronously connects to an SSH server at the specified host and port.
    /// </summary>
    /// <param name="host">The hostname or IP address of the SSH server.</param>
    /// <param name="port">The port number of the SSH server (typically 22).</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <exception cref="SshException">Thrown when connection or SSH handshake fails.</exception>
    /// <remarks>
    /// This method offloads the blocking connection and handshake operations to a thread pool thread.
    /// After successful connection, the session will be in <see cref="SshConnectionStatus.Connected"/> status.
    /// You must call <see cref="AuthenticateAsync"/> or <see cref="Authenticate"/> before executing commands.
    /// </remarks>
    public Task ConnectAsync(string host, int port, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Connect(host, port), cancellationToken);
    }

    /// <summary>
    /// Executes a command on the remote SSH server.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="options">Optional execution options including PTY settings. If null, uses default options.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>The result of the command execution including stdout and stderr output.</returns>
    /// <remarks>
    /// When <see cref="CommandExecutionOptions.RequestPty"/> is true, a pseudo-terminal (PTY) will be requested
    /// before executing the command. This enables terminal features like color output and interactive input.
    /// </remarks>
    public unsafe SshCommandResult ExecuteCommand(string command, CommandExecutionOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= CommandExecutionOptions.Default;

        LibSsh2.Log($"Opening channel for command execution: '{command}'");
        var channel = libssh2_channel_open_ex(
            session,
            StringPointers.Session,
            7,
            options.WindowSize,
            options.PacketSize,
            null,
            0
        );

        if (channel == null)
        {
            LibSsh2.Log("Failed to open channel");
            return SshCommandResult.Unsuccessful;
        }

        LibSsh2.Log("Channel opened successfully");

        // Request PTY if specified
        if (options.RequestPty)
        {
            LibSsh2.Log($"Requesting PTY (type: {options.TerminalType}, size: {options.TerminalWidth}x{options.TerminalHeight})");
            var terminalType = options.TerminalType.ToLibSsh2String();
            var terminalModes = options.TerminalModes ?? TerminalModesBuilder.Empty;

            using var terminalTypeBuffer = NativeBuffer.Allocate(terminalType);
            using var terminalModesBuffer = NativeBuffer.Allocate(terminalModes);

            var ptyResult = libssh2_channel_request_pty_ex(
                channel,
                (sbyte*)terminalTypeBuffer.Pointer,
                (uint)terminalTypeBuffer.Length,
                (sbyte*)terminalModesBuffer.Pointer,
                (uint)terminalModesBuffer.Length,
                options.TerminalWidth,
                options.TerminalHeight,
                options.TerminalWidthPixels,
                options.TerminalHeightPixels
            );

            ptyResult.ThrowIfNotSuccessful("Failed to request PTY", also: () =>
            {
                libssh2_channel_close(channel);
                libssh2_channel_wait_closed(channel);
                libssh2_channel_free(channel);
            });

            LibSsh2.Log("PTY requested successfully");
        }

        LibSsh2.Log("Starting command process execution");
        using var commandBytes = NativeBuffer.Allocate(command);

        var processStartupResult = libssh2_channel_process_startup(
            channel,
            StringPointers.Exec,
            4,
            (sbyte*)commandBytes.Pointer,
            (uint)commandBytes.Length
        );

        processStartupResult.ThrowIfNotSuccessful("Unable to execute command", also: () =>
        {
            libssh2_channel_close(channel);
            libssh2_channel_wait_closed(channel);
            libssh2_channel_free(channel);
        });

        LibSsh2.Log("Command process started, reading output");
        var stdout = ChannelReader.ReadUtf8String(channel, ChannelReader.StdoutStreamId, cancellationToken);
        LibSsh2.Log($"Read {stdout.Length} bytes from stdout");

        var stderr = ChannelReader.ReadUtf8String(channel, ChannelReader.StderrStreamId, cancellationToken);
        LibSsh2.Log($"Read {stderr.Length} bytes from stderr");

        LibSsh2.Log("Closing channel");
        libssh2_channel_close(channel);
        libssh2_channel_wait_closed(channel);

        LibSsh2.Log("Retrieving exit status");
        var exitStatus = libssh2_channel_get_exit_status(channel);
        LibSsh2.Log($"Command exit status: {exitStatus}");

        LibSsh2.Log("Retrieving exit signal");
        sbyte* exitSignalPtr = null;
        nuint exitSignalLen = 0;
        var exitSignalResult = libssh2_channel_get_exit_signal(channel, &exitSignalPtr, &exitSignalLen, null, null, null, null);

        string? exitSignal = null;
        if (exitSignalResult == 0 && exitSignalPtr != null && exitSignalLen > 0)
        {
            exitSignal = Marshal.PtrToStringUTF8((IntPtr)exitSignalPtr, (int)exitSignalLen);
            LibSsh2.Log($"Command exit signal: {exitSignal}");
        }
        else
        {
            LibSsh2.Log("No exit signal");
        }

        libssh2_channel_free(channel);
        LibSsh2.Log("Channel closed");

        return new SshCommandResult
        {
            Successful = true,
            Stdout = stdout,
            Stderr = stderr,
            ExitCode = exitStatus,
            ExitSignal = exitSignal
        };
    }

    /// <summary>
    /// Asynchronously executes a command on the remote SSH server.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="options">Optional execution options including PTY settings. If null, uses default options.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the command execution including stdout and stderr output.</returns>
    /// <remarks>
    /// This method offloads the blocking command execution to a thread pool thread.
    /// When <see cref="CommandExecutionOptions.RequestPty"/> is true, a pseudo-terminal (PTY) will be requested
    /// before executing the command. This enables terminal features like color output and interactive input.
    /// </remarks>
    public Task<SshCommandResult> ExecuteCommandAsync(string command, CommandExecutionOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => ExecuteCommand(command, options, cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Downloads a file from the remote SSH server using SCP protocol.
    /// </summary>
    /// <param name="path">The full path to the remote file to download.</param>
    /// <param name="destination">The stream to write the downloaded file contents to. This stream will not be closed by this method.</param>
    /// <param name="bufferSize">The size of the buffer to use for reading data. Default is 32768 bytes.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>True if the entire file was successfully downloaded; false otherwise.</returns>
    /// <exception cref="SshException">Thrown when the SCP channel cannot be created or other SSH errors occur.</exception>
    /// <remarks>
    /// This method does not close the destination stream. The caller is responsible for managing the stream's lifecycle.
    /// The method uses SCP (Secure Copy Protocol) to transfer the file.
    /// </remarks>
    public unsafe bool ReadFile(string path, Stream destination, int bufferSize = 32768, CancellationToken cancellationToken = default)
    {
        LibSsh2.Log($"Starting SCP download of file: '{path}'");
        using var remotePathBuffer = NativeBuffer.Allocate(path);
        using var statBuffer = NativeBuffer.Allocate(512);

        LibSsh2.Log("Opening SCP receive channel");
        var scpChannel = libssh2_scp_recv2(
            session,
            remotePathBuffer.AsPointer<sbyte>(),
            statBuffer.AsPointer());

        if (scpChannel == null)
        {
            LibSsh2.Log("Failed to open SCP receive channel");
            throw SshException.FromLastSessionError(session);
        }

        LibSsh2.Log("SCP receive channel opened successfully");

        try
        {
            var stat = PlatformInDependentStat.From(statBuffer.AsPointer());
            LibSsh2.Log($"Remote file size: {stat.FileSize} bytes");

            LibSsh2.Log($"Starting file transfer (buffer size: {bufferSize})");
            var totalReceived = ChannelReader.CopyToStream(scpChannel, ChannelReader.StdoutStreamId, destination, bufferSize, (int) stat.FileSize, cancellationToken);
            LibSsh2.Log($"Transfer complete. Received {totalReceived}/{stat.FileSize} bytes");

            return totalReceived == stat.FileSize;
        }
        finally
        {
            LibSsh2.Log("Closing SCP channel");
            libssh2_channel_send_eof(scpChannel);
            libssh2_channel_wait_eof(scpChannel);
            libssh2_channel_wait_closed(scpChannel);
            libssh2_channel_free(scpChannel);
            LibSsh2.Log("SCP channel closed");
        }
    }

    /// <summary>
    /// Asynchronously downloads a file from the remote SSH server using SCP protocol.
    /// </summary>
    /// <param name="path">The full path to the remote file to download.</param>
    /// <param name="destination">The stream to write the downloaded file contents to. This stream will not be closed by this method.</param>
    /// <param name="bufferSize">The size of the buffer to use for reading data. Default is 32768 bytes.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a boolean value indicating whether the entire file was successfully downloaded.</returns>
    /// <exception cref="SshException">Thrown when the SCP channel cannot be created or other SSH errors occur.</exception>
    /// <remarks>
    /// This method offloads the blocking SCP file transfer to a thread pool thread.
    /// The destination stream will not be closed by this method. The caller is responsible for managing the stream's lifecycle.
    /// </remarks>
    public Task<bool> ReadFileAsync(string path, Stream destination, int bufferSize = 32768, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => ReadFile(path, destination, bufferSize, cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Uploads a file to the remote SSH server using SCP protocol.
    /// </summary>
    /// <param name="path">The full path where the file should be created on the remote server.</param>
    /// <param name="source">The stream containing the data to upload. Must be readable and seekable. This stream will not be closed by this method.</param>
    /// <param name="mode">Unix file permissions for the created file. Default is 420 (octal 0644, equivalent to rw-r--r--). Only permission bits are used (0-0777).</param>
    /// <param name="bufferSize">The size of the buffer to use for writing data. Default is 32768 bytes.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>True if the entire file was successfully uploaded; false otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when the source stream is not readable or not seekable.</exception>
    /// <exception cref="SshException">Thrown when the SCP channel cannot be created or other SSH errors occur.</exception>
    /// <remarks>
    /// This method does not close the source stream. The caller is responsible for managing the stream's lifecycle.
    /// The method uses SCP (Secure Copy Protocol) to transfer the file.
    /// The source stream must be seekable because the total file size must be known before transmission begins.
    /// </remarks>
    public unsafe bool WriteFile(string path, Stream source, int mode = 420, int bufferSize = 32768, CancellationToken cancellationToken = default)
    {
        LibSsh2.Log($"Starting SCP upload to file: '{path}'");

        if (!source.CanRead)
            throw new ArgumentException("Source stream must be readable", nameof(source));

        if (!source.CanSeek)
            throw new ArgumentException("Source stream must be seekable", nameof(source));

        var fileSize = source.Length - source.Position;
        LibSsh2.Log($"File size to upload: {fileSize} bytes, mode: {mode}");

        using var remotePathBuffer = NativeBuffer.Allocate(path);

        LibSsh2.Log("Opening SCP send channel");
        var scpChannel = libssh2_scp_send64(
            session,
            remotePathBuffer.AsPointer<sbyte>(),
            mode,
            fileSize,
            0,
            0);

        if (scpChannel == null)
        {
            LibSsh2.Log("Failed to open SCP send channel");
            throw SshException.FromLastSessionError(session);
        }

        LibSsh2.Log("SCP send channel opened successfully");

        try
        {
            LibSsh2.Log($"Starting file transfer (buffer size: {bufferSize})");
            var totalSent = ChannelReader.CopyToChannel(scpChannel, ChannelReader.StdoutStreamId, source, fileSize, bufferSize, cancellationToken);
            LibSsh2.Log($"Transfer complete. Sent {totalSent}/{fileSize} bytes");

            return totalSent == fileSize;
        }
        finally
        {
            LibSsh2.Log("Closing SCP channel");
            libssh2_channel_send_eof(scpChannel);
            libssh2_channel_wait_eof(scpChannel);
            libssh2_channel_wait_closed(scpChannel);
            libssh2_channel_free(scpChannel);
            LibSsh2.Log("SCP channel closed");
        }
    }

    /// <summary>
    /// Asynchronously uploads a file to the remote SSH server using SCP protocol.
    /// </summary>
    /// <param name="path">The full path where the file should be created on the remote server.</param>
    /// <param name="source">The stream containing the data to upload. Must be readable and seekable. This stream will not be closed by this method.</param>
    /// <param name="mode">Unix file permissions for the created file. Default is 420 (octal 0644, equivalent to rw-r--r--). Only permission bits are used (0-0777).</param>
    /// <param name="bufferSize">The size of the buffer to use for writing data. Default is 32768 bytes.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a boolean value indicating whether the entire file was successfully uploaded.</returns>
    /// <exception cref="ArgumentException">Thrown when the source stream is not readable or not seekable.</exception>
    /// <exception cref="SshException">Thrown when the SCP channel cannot be created or other SSH errors occur.</exception>
    /// <remarks>
    /// This method offloads the blocking SCP file transfer to a thread pool thread.
    /// The source stream will not be closed by this method. The caller is responsible for managing the stream's lifecycle.
    /// The source stream must be seekable because the total file size must be known before transmission begins.
    /// </remarks>
    public Task<bool> WriteFileAsync(string path, Stream source, int mode = 420, int bufferSize = 32768, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => WriteFile(path, source, mode, bufferSize, cancellationToken), cancellationToken);
    }
    
    /// <summary>
    /// Authenticates the SSH session using the provided credentials.
    /// </summary>
    /// <param name="credential">The credentials to use for authentication (e.g., password, key-based).</param>
    /// <returns>True if authentication succeeded; false otherwise.</returns>
    /// <exception cref="SshException">Thrown when the session is not in the Connected state.</exception>
    /// <remarks>
    /// The session must be in <see cref="SshConnectionStatus.Connected"/> status before calling this method.
    /// After successful authentication, the session will be in <see cref="SshConnectionStatus.LoggedIn"/> status.
    /// </remarks>
    public unsafe bool Authenticate(SshCredential credential)
    {
        EnsureInitialized();
        EnsureInStatus(SshConnectionStatus.Connected);

        LibSsh2.Log($"Starting authentication with credential type: {credential.GetType().Name}");
        var result = credential.Authenticate(session);

        if (result)
        {
            LibSsh2.Log("Authentication successful");
            ConnectionStatus = SshConnectionStatus.LoggedIn;
        }
        else
        {
            LibSsh2.Log("Authentication failed");
        }

        return result;
    }

    /// <summary>
    /// Asynchronously authenticates the SSH session using the provided credentials.
    /// </summary>
    /// <param name="credential">The credentials to use for authentication (e.g., password, key-based).</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a boolean value indicating whether authentication succeeded.</returns>
    /// <exception cref="SshException">Thrown when the session is not in the Connected state.</exception>
    /// <remarks>
    /// This method offloads the blocking authentication operation to a thread pool thread.
    /// The session must be in <see cref="SshConnectionStatus.Connected"/> status before calling this method.
    /// After successful authentication, the session will be in <see cref="SshConnectionStatus.LoggedIn"/> status.
    /// </remarks>
    public Task<bool> AuthenticateAsync(SshCredential credential, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Authenticate(credential), cancellationToken);
    }

    /// <summary>
    /// Gets the last error message from the libssh2 session (for debugging).
    /// </summary>
    internal unsafe string GetLastError()
    {
        if (session == null)
            return "Session is null";

        sbyte* errorMsg = null;
        int errorMsgLen = 0;
        var errorCode = LibSshNative.libssh2_session_last_error(session, &errorMsg, &errorMsgLen, 0);

        if (errorCode == 0 || errorMsg == null)
            return "No error";

        return System.Runtime.InteropServices.Marshal.PtrToStringUTF8((IntPtr)errorMsg) ?? "Unknown error";
    }

    /// <inheritdoc />
    public unsafe void Dispose()
    {
        if(ConnectionStatus == SshConnectionStatus.Disposed)
            return;

        if (session != null)
        {
            _ = libssh2_session_free(session);
            session = null;
        }

        socket?.Dispose();
        ConnectionStatus = SshConnectionStatus.Disposed;
    }
}