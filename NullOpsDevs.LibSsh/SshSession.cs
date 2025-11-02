using System.Net.Sockets;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NullOpsDevs.LibSsh.Core;
using NullOpsDevs.LibSsh.Credentials;
using NullOpsDevs.LibSsh.Exceptions;
using NullOpsDevs.LibSsh.Extensions;
using NullOpsDevs.LibSsh.Generated;
using NullOpsDevs.LibSsh.Interop;
using NullOpsDevs.LibSsh.Platform;
using NullOpsDevs.LibSsh.Terminal;
using static NullOpsDevs.LibSsh.Generated.LibSshNative;

namespace NullOpsDevs.LibSsh;

/// <summary>
/// Represents an SSH session for connecting to and communicating with remote SSH servers.
/// </summary>
[PublicAPI]
public sealed class SshSession(ILogger? logger = null) : IDisposable
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

    private void EnsureInitialized()
    {
        lock (LibSsh2.GlobalLock)
        {
            if(libraryInitialized)
                return;
            
            logger?.LogDebug("Initializing libssh2 library");

            var initResult = libssh2_init(0);
            logger?.LogDebug("libssh2_init returned: {InitResult}", initResult);
            
            initResult.ThrowIfNotSuccessful("LibSSH2 initialization failed");
            libraryInitialized = true;
        }
    }

    private void EnsureInStatus(SshConnectionStatus status)
    {
        if (ConnectionStatus != status)
            throw new SshException($"SshConnection must be in status '{status:G}' to perform that operation.", SshError.DevWrongUse);
    }
    
    private void EnsureInStatuses(params SshConnectionStatus[] statuses)
    {
        foreach (var status in statuses)
        {
            if (ConnectionStatus == status)
                return;
        }
        
        throw new SshException($"SshConnection must be in one of the statuses '{string.Join(", ", statuses.Select(s => $"{s:G}"))}' to perform that operation.", SshError.DevWrongUse);
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
                logger?.LogDebug("Connecting to server: '{Host}:{Port}'...", host, port);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(host, port);
            }
            catch (Exception ex)
            {
                _ = libssh2_session_free(newSession);
                throw ex.AsSshException();
            }

            logger?.LogDebug("Connected to server: '{Host}:{Port}'", host, port);
            logger?.LogDebug("Handshaking...");

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
    
    public unsafe SshHostKey GetHostKey()
    {
        EnsureInitialized();
        EnsureInStatuses(SshConnectionStatus.Connected, SshConnectionStatus.LoggedIn);

        UIntPtr length = 0;
        var type = 0;
        
        var ptr = libssh2_session_hostkey(session, &length, &type);
        
        if (ptr == null || length == 0)
            throw SshException.FromLastSessionError(session);
        
        var buffer = new byte[length];
        Marshal.Copy(new IntPtr(ptr), buffer, 0, (int) length);

        return new SshHostKey
        {
            Key = buffer,
            Type = (SshHostKeyType)type
        };
    }

    public unsafe string? GetNegotiatedMethod(SshMethod method)
    {
        EnsureInitialized();
        EnsureInStatuses(SshConnectionStatus.Connected, SshConnectionStatus.LoggedIn);
        
        if(!Enum.IsDefined(method))
            throw new ArgumentOutOfRangeException(nameof(method), method, null);
        
        var result = libssh2_session_methods(session, (int) method);
        
        if(result == null)
            throw SshException.FromLastSessionError(session);

        return Marshal.PtrToStringAnsi(new IntPtr(result));
    }
    
    public unsafe void SetMethodPreferences(SshMethod method, string preferences)
    {
        EnsureInitialized();
        EnsureInStatus(SshConnectionStatus.Disconnected);
        
        using var preferencesBuffer = NativeBuffer.Allocate(preferences);
        
        var result = libssh2_session_method_pref(session, (int) method, preferencesBuffer.AsPointer<sbyte>());
        result.ThrowIfNotSuccessful("Failed to set preferences");
    }

    public void SetSecureMethodPreferences()
    {
        SetMethodPreferences(SshMethod.Kex, "curve25519-sha256,curve25519-sha256@libssh.org,ecdh-sha2-nistp521,ecdh-sha2-nistp384,ecdh-sha2-nistp256,diffie-hellman-group-exchange-sha256,diffie-hellman-group16-sha512,diffie-hellman-group18-sha512,diffie-hellman-group14-sha256");
        SetMethodPreferences(SshMethod.HostKey, "ssh-ed25519,ecdsa-sha2-nistp521,ecdsa-sha2-nistp384,ecdsa-sha2-nistp256,rsa-sha2-512,rsa-sha2-256");
        SetMethodPreferences(SshMethod.CryptCs, "chacha20-poly1305@openssh.com,aes256-gcm@openssh.com,aes128-gcm@openssh.com,aes256-ctr,aes192-ctr,aes128-ctr");
        SetMethodPreferences(SshMethod.CryptSc, "chacha20-poly1305@openssh.com,aes256-gcm@openssh.com,aes128-gcm@openssh.com,aes256-ctr,aes192-ctr,aes128-ctr");
        SetMethodPreferences(SshMethod.MacCs, "hmac-sha2-256-etm@openssh.com,hmac-sha2-512-etm@openssh.com,hmac-sha2-256,hmac-sha2-512");
        SetMethodPreferences(SshMethod.MacSc, "hmac-sha2-256-etm@openssh.com,hmac-sha2-512-etm@openssh.com,hmac-sha2-256,hmac-sha2-512");
        SetMethodPreferences(SshMethod.CompCs, "none");
        SetMethodPreferences(SshMethod.CompSc, "none");
    }

    public unsafe void DisableSessionTimeout() => libssh2_session_set_timeout(session, 0);

    public unsafe void SetSessionTimeout(TimeSpan timeout)
    {
        if (timeout < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "Timeout must be greater than zero");
        
        if(timeout.TotalMilliseconds > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "Timeout cannot be greater than ");
        
        libssh2_session_set_timeout(session, (int) timeout.TotalMilliseconds);
    }

    public unsafe byte[] GetHostKeyHash(SshHashType keyHashType)
    {
        EnsureInitialized();
        EnsureInStatuses(SshConnectionStatus.Connected, SshConnectionStatus.LoggedIn);
        
        var keySize = keyHashType switch
        {
            SshHashType.MD5 => 16,
            SshHashType.SHA1 => 20,
            SshHashType.SHA256 => 32,
            _ => throw new ArgumentOutOfRangeException(nameof(keyHashType), keyHashType, null)
        };
        
        var hash = libssh2_hostkey_hash(session, (int) keyHashType);

        if (hash == null)
            throw SshException.FromLastSessionError(session);
        
        var keyHash = new byte[keySize];
        Marshal.Copy(new IntPtr(hash), keyHash, 0, keySize);
        
        return keyHash;
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
        EnsureInitialized();
        EnsureInStatus(SshConnectionStatus.LoggedIn);
        
        options ??= CommandExecutionOptions.Default;

        logger?.LogDebug("Opening channel for command execution: '{Command}'", command);
        
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
            logger?.LogDebug("Failed to open channel");
            return SshCommandResult.Unsuccessful;
        }

        logger?.LogDebug("Channel opened successfully");

        // Request PTY if specified
        if (options.RequestPty)
        {
            logger?.LogDebug("Requesting PTY (type: {OptionsTerminalType}, size: {OptionsTerminalWidth}x{OptionsTerminalHeight})", options.TerminalType, options.TerminalWidth, options.TerminalHeight);
            
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

            logger?.LogDebug("PTY requested successfully");
        }

        logger?.LogDebug("Starting command process execution");
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

        logger?.LogDebug("Command process started, reading output");
        var stdout = ChannelReader.ReadUtf8String(channel, ChannelReader.StdoutStreamId, cancellationToken);
        logger?.LogDebug("Read {StdoutLength} bytes from stdout", stdout.Length);

        var stderr = ChannelReader.ReadUtf8String(channel, ChannelReader.StderrStreamId, cancellationToken);
        logger?.LogDebug("Read {StderrLength} bytes from stderr", stderr.Length);

        logger?.LogDebug("Closing channel");
        libssh2_channel_close(channel);
        libssh2_channel_wait_closed(channel);

        logger?.LogDebug("Retrieving exit status");
        var exitStatus = libssh2_channel_get_exit_status(channel);
        logger?.LogDebug("Command exit status: {ExitStatus}", exitStatus);

        logger?.LogDebug("Retrieving exit signal");
        sbyte* exitSignalPtr = null;
        nuint exitSignalLen = 0;
        var exitSignalResult = libssh2_channel_get_exit_signal(channel, &exitSignalPtr, &exitSignalLen, null, null, null, null);

        string? exitSignal = null;
        if (exitSignalResult == 0 && exitSignalPtr != null && exitSignalLen > 0)
        {
            exitSignal = Marshal.PtrToStringUTF8((IntPtr)exitSignalPtr, (int)exitSignalLen);
            logger?.LogDebug("Command exit signal: {ExitSignal}", exitSignal);
        }
        else
        {
            logger?.LogDebug("No exit signal");
        }

        libssh2_channel_free(channel);
        logger?.LogDebug("Channel closed");

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
        EnsureInitialized();
        EnsureInStatus(SshConnectionStatus.LoggedIn);
        
        logger?.LogDebug("Starting SCP download of file: '{Path}'", path);
        using var remotePathBuffer = NativeBuffer.Allocate(path);
        using var statBuffer = NativeBuffer.Allocate(512);

        logger?.LogDebug("Opening SCP receive channel");
        var scpChannel = libssh2_scp_recv2(
            session,
            remotePathBuffer.AsPointer<sbyte>(),
            statBuffer.AsPointer());

        if (scpChannel == null)
        {
            logger?.LogDebug("Failed to open SCP receive channel");
            throw SshException.FromLastSessionError(session);
        }

        logger?.LogDebug("SCP receive channel opened successfully");

        try
        {
            var stat = PlatformInDependentStat.From(statBuffer.AsPointer());
            logger?.LogDebug("Remote file size: {StatFileSize} bytes", stat.FileSize);

            logger?.LogDebug("Starting file transfer (buffer size: {BufferSize})", bufferSize);
            var totalReceived = ChannelReader.CopyToStream(scpChannel, ChannelReader.StdoutStreamId, destination, bufferSize, (int) stat.FileSize, cancellationToken);
            logger?.LogDebug("Transfer complete. Received {TotalReceived}/{StatFileSize} bytes", totalReceived, stat.FileSize);

            return totalReceived == stat.FileSize;
        }
        finally
        {
            logger?.LogDebug("Closing SCP channel");
            libssh2_channel_send_eof(scpChannel);
            libssh2_channel_wait_eof(scpChannel);
            libssh2_channel_wait_closed(scpChannel);
            libssh2_channel_free(scpChannel);
            logger?.LogDebug("SCP channel closed");
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
        EnsureInitialized();
        EnsureInStatus(SshConnectionStatus.LoggedIn);
        
        logger?.LogDebug("Starting SCP upload to file: '{Path}'", path);

        if (!source.CanRead)
            throw new ArgumentException("Source stream must be readable", nameof(source));

        if (!source.CanSeek)
            throw new ArgumentException("Source stream must be seekable", nameof(source));

        var fileSize = source.Length - source.Position;
        logger?.LogDebug("File size to upload: {FileSize} bytes, mode: {Mode}", fileSize, mode);

        using var remotePathBuffer = NativeBuffer.Allocate(path);

        logger?.LogDebug("Opening SCP send channel");
        var scpChannel = libssh2_scp_send64(
            session,
            remotePathBuffer.AsPointer<sbyte>(),
            mode,
            fileSize,
            0,
            0);

        if (scpChannel == null)
        {
            logger?.LogDebug("Failed to open SCP send channel");
            throw SshException.FromLastSessionError(session);
        }

        logger?.LogDebug("SCP send channel opened successfully");

        try
        {
            logger?.LogDebug("Starting file transfer (buffer size: {BufferSize})", bufferSize);
            var totalSent = ChannelReader.CopyToChannel(scpChannel, ChannelReader.StdoutStreamId, source, fileSize, bufferSize, cancellationToken);
            logger?.LogDebug("Transfer complete. Sent {TotalSent}/{FileSize} bytes", totalSent, fileSize);

            return totalSent == fileSize;
        }
        finally
        {
            logger?.LogDebug("Closing SCP channel");
            libssh2_channel_send_eof(scpChannel);
            libssh2_channel_wait_eof(scpChannel);
            libssh2_channel_wait_closed(scpChannel);
            libssh2_channel_free(scpChannel);
            logger?.LogDebug("SCP channel closed");
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

        logger?.LogDebug("Starting authentication with credential type: {Name}", credential.GetType().Name);
        var result = credential.Authenticate(session);

        if (result)
        {
            logger?.LogDebug("Authentication successful");
            ConnectionStatus = SshConnectionStatus.LoggedIn;
        }
        else
        {
            logger?.LogDebug("Authentication failed");
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
        var errorMsgLen = 0;
        var errorCode = libssh2_session_last_error(session, &errorMsg, &errorMsgLen, 0);

        if (errorCode == 0 || errorMsg == null)
            return "No error";

        return Marshal.PtrToStringUTF8((IntPtr)errorMsg) ?? "Unknown error";
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