using System.Net.Sockets;
using JetBrains.Annotations;
using NullOpsDevs.LibSsh.Exceptions;
using NullOpsDevs.LibSsh.Extensions;
using NullOpsDevs.LibSsh.Generated;
using static NullOpsDevs.LibSsh.Generated.LibSshNative;

namespace NullOpsDevs.LibSsh;

/// <summary>
/// Represents an SSH session for connecting to and communicating with remote SSH servers.
/// </summary>
[PublicAPI]
public sealed class SshSession : IDisposable
{
    private static bool libraryInitialized;
    private readonly Lock localLock = new();

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
            return SshCommandResult.Unsuccessful;

        // Request PTY if specified
        if (options.RequestPty)
        {
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
        }

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

        var stdout = ChannelReader.ReadUtf8String(channel, ChannelReader.StdoutStreamId, cancellationToken);
        var stderr = ChannelReader.ReadUtf8String(channel, ChannelReader.StderrStreamId, cancellationToken);

        libssh2_channel_close(channel);
        libssh2_channel_wait_closed(channel);
        libssh2_channel_free(channel);

        return new SshCommandResult
        {
            Successful = true,
            Stdout = stdout,
            Stderr = stderr
        };
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
        using var remotePathBuffer = NativeBuffer.Allocate(path);
        using var statBuffer = NativeBuffer.Allocate(512);

        var scpChannel = libssh2_scp_recv2(
            session,
            remotePathBuffer.AsPointer<sbyte>(),
            statBuffer.AsPointer());

        if (scpChannel == null)
            throw SshException.FromLastSessionError(session);

        try
        {
            var stat = PlatformInDependentStat.From(statBuffer.AsPointer());
            var totalReceived = ChannelReader.CopyToStream(scpChannel, ChannelReader.StdoutStreamId, destination, bufferSize, (int) stat.FileSize, cancellationToken);
            return totalReceived == stat.FileSize;
        }
        finally
        {
            libssh2_channel_send_eof(scpChannel);
            libssh2_channel_wait_eof(scpChannel);
            libssh2_channel_wait_closed(scpChannel);
            libssh2_channel_free(scpChannel);
        }
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
        if (!source.CanRead)
            throw new ArgumentException("Source stream must be readable", nameof(source));

        if (!source.CanSeek)
            throw new ArgumentException("Source stream must be seekable", nameof(source));

        var fileSize = source.Length - source.Position;

        using var remotePathBuffer = NativeBuffer.Allocate(path);

        var scpChannel = libssh2_scp_send64(
            session,
            remotePathBuffer.AsPointer<sbyte>(),
            mode,
            fileSize,
            0,
            0);

        if (scpChannel == null)
            throw SshException.FromLastSessionError(session);

        try
        {
            var totalSent = ChannelReader.CopyToChannel(scpChannel, ChannelReader.StdoutStreamId, source, fileSize, bufferSize, cancellationToken);
            return totalSent == fileSize;
        }
        finally
        {
            libssh2_channel_send_eof(scpChannel);
            libssh2_channel_wait_eof(scpChannel);
            libssh2_channel_wait_closed(scpChannel);
            libssh2_channel_free(scpChannel);
        }
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

        var result = credential.Authenticate(session);

        if (result)
            ConnectionStatus = SshConnectionStatus.LoggedIn;

        return result;
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