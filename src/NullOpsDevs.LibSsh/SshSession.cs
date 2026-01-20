using System.Net.Sockets;
using System.Runtime.CompilerServices;
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

    /// <summary>
    /// Gets the current connection status of the SSH session.
    /// </summary>
    public SshConnectionStatus ConnectionStatus { get; private set; }
    
    internal unsafe _LIBSSH2_SESSION* SessionPtr { get; private set; }
    
    private readonly Dictionary<SshMethod, string> methodPreferences = new();

#if NET5_0_OR_GREATER
    // Trace handler support - maps session pointers to handlers
    private static readonly Dictionary<nint, Action<string>> TraceHandlers = new();
    private static readonly object TraceHandlersLock = new();
#endif

    private void EnsureInitialized()
    {
        lock (LibSsh2.GlobalLock)
        {
            if(libraryInitialized)
                return;
            
            logger?.LogDebug("Initializing libssh2 library");

            var initResult = libssh2_init(0);
            logger?.LogDebug("libssh2_init returned: {InitResult}", initResult);
            
            initResult.ThrowIfNotSuccessful(this, "LibSSH2 initialization failed");
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
    /// <param name="socketTimeout">Optional timeout for socket send and receive operations. If not specified, the socket will have no timeout (infinite). Must be greater than zero and less than <see cref="int.MaxValue"/> milliseconds.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when socketTimeout is negative or exceeds <see cref="int.MaxValue"/> milliseconds.</exception>
    /// <exception cref="SshException">Thrown when connection or SSH handshake fails.</exception>
    /// <remarks>
    /// <para>This method establishes a TCP connection and performs the SSH protocol handshake.</para>
    /// <para>The session must be in <see cref="SshConnectionStatus.Disconnected"/> status before calling this method.</para>
    /// <para>After successful connection, the session will be in <see cref="SshConnectionStatus.Connected"/> status.</para>
    /// <para>You must call <see cref="Authenticate"/> before executing commands.</para>
    /// <para>The <paramref name="socketTimeout"/> parameter controls the underlying TCP socket's send and receive timeout values. This is separate from the SSH session timeout configured via <see cref="SetSessionTimeout"/>. Socket timeouts affect low-level network operations, while session timeouts affect SSH protocol operations.</para>
    /// </remarks>
    public unsafe void Connect(string host, int port, TimeSpan? socketTimeout = null)
    {
        if(socketTimeout.HasValue && (socketTimeout.Value < TimeSpan.Zero || socketTimeout.Value.TotalMilliseconds > int.MaxValue))
            throw new ArgumentOutOfRangeException(nameof(socketTimeout), socketTimeout, "Socket timeout must be greater than zero and less than int.MaxValue milliseconds");
            
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
                socket?.Dispose();
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
                if (socketTimeout.HasValue)
                {
                    socket.ReceiveTimeout = (int)socketTimeout.Value.TotalMilliseconds;
                    socket.SendTimeout = (int)socketTimeout.Value.TotalMilliseconds;
                }
                
                socket.Connect(host, port);
            }
            catch (Exception ex)
            {
                _ = libssh2_session_free(newSession);
                throw ex.AsSshException();
            }

            logger?.LogDebug("Connected to server: '{Host}:{Port}'", host, port);
            
            logger?.LogDebug("Setting method preferences...");

            foreach (var (method, pref) in methodPreferences)
            {
                logger?.LogDebug("Setting method preference for '{Method:G}' to '{Pref}'", method, pref);
                
                using var prefBuffer = NativeBuffer.Allocate(pref);
                
                var prefResult = libssh2_session_method_pref(newSession, (int) method, prefBuffer.AsPointer<sbyte>());
                
                prefResult.ThrowIfNotSuccessful(this, $"Failed to set preference to '{method:G}' to '{pref}'", also: () =>
                {
                    _ = libssh2_session_free(newSession);
                    socket.Dispose();
                });
            }
            
            logger?.LogDebug("Handshaking...");

            var result = libssh2_session_handshake(newSession, (ulong)socket.Handle);

            result.ThrowIfNotSuccessful(this, "Failed to handshake with server", also: () =>
            {
                _ = libssh2_session_free(newSession);
                socket.Dispose();
            });

            libssh2_session_set_blocking(newSession, 1);

            ConnectionStatus = SshConnectionStatus.Connected;
            SessionPtr = newSession;
        }
    }

    /// <summary>
    /// Retrieves the host key from the connected SSH server.
    /// </summary>
    /// <returns>The server's host key including the raw key data and algorithm type.</returns>
    /// <exception cref="SshException">Thrown if the session is not connected or the host key cannot be retrieved.</exception>
    /// <remarks>
    /// This method must be called after connecting to the server. The session must be in <see cref="SshConnectionStatus.Connected"/> or <see cref="SshConnectionStatus.LoggedIn"/> status.
    /// </remarks>
    public unsafe SshHostKey GetHostKey()
    {
        EnsureInitialized();
        EnsureInStatuses(SshConnectionStatus.Connected, SshConnectionStatus.LoggedIn);

        var length = UIntPtr.Zero;
        var type = 0;
        
        var ptr = libssh2_session_hostkey(SessionPtr, &length, &type);
        
        if (ptr == null || length == UIntPtr.Zero)
            throw SshException.FromLastSessionError(SessionPtr);
        
        var buffer = new byte[length.ToUInt64()];
        Marshal.Copy(new IntPtr(ptr), buffer, 0, (int) length);

        return new SshHostKey
        {
            Key = buffer,
            Type = (SshHostKeyType)type
        };
    }

    /// <summary>
    /// Gets the negotiated algorithm name for the specified SSH method type.
    /// </summary>
    /// <param name="method">The SSH method type to query (e.g., <see cref="SshMethod.Kex"/>, <see cref="SshMethod.CryptCs"/>).</param>
    /// <returns>The negotiated algorithm name, or null if not available.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the method parameter is not a valid <see cref="SshMethod"/> value.</exception>
    /// <exception cref="SshException">Thrown if the session is not connected or the method information cannot be retrieved.</exception>
    /// <remarks>
    /// This method must be called after connecting to the server. The session must be in <see cref="SshConnectionStatus.Connected"/> or <see cref="SshConnectionStatus.LoggedIn"/> status.
    /// </remarks>
    public unsafe string? GetNegotiatedMethod(SshMethod method)
    {
        EnsureInitialized();
        EnsureInStatuses(SshConnectionStatus.Connected, SshConnectionStatus.LoggedIn);
        
        if(!Enum.IsDefined(typeof(SshMethod), method))
            throw new ArgumentOutOfRangeException(nameof(method), method, null);
        
        var result = libssh2_session_methods(SessionPtr, (int) method);
        
        if(result == null)
            throw SshException.FromLastSessionError(SessionPtr);

        return Marshal.PtrToStringAnsi(new IntPtr(result));
    }

    /// <summary>
    /// Sets the algorithm preference list for the specified SSH method type.
    /// </summary>
    /// <param name="method">The SSH method type to configure (e.g., <see cref="SshMethod.Kex"/>, <see cref="SshMethod.CryptCs"/>).</param>
    /// <param name="preferences">A comma-separated list of algorithm names in preference order (e.g., "aes256-ctr,aes128-ctr").</param>
    /// <exception cref="SshException">Thrown if the preferences cannot be set or the session is not in the correct state.</exception>
    /// <remarks>
    /// This method must be called before connecting to the server. The session must be in <see cref="SshConnectionStatus.Disconnected"/> status.
    /// Use <see cref="SetSecureMethodPreferences"/> to apply a predefined set of secure defaults.
    /// </remarks>
    public void SetMethodPreferences(SshMethod method, string preferences)
    {
        if(string.IsNullOrWhiteSpace(preferences))
            throw new ArgumentException("Preferences cannot be empty", nameof(preferences));
        
        EnsureInitialized();
        EnsureInStatus(SshConnectionStatus.Disconnected);
        
        methodPreferences[method] = preferences;
    }

    /// <summary>
    /// Applies a secure set of default algorithm preferences for all SSH method types.
    /// </summary>
    /// <remarks>
    /// <para>This method configures the following secure defaults:</para>
    /// <list type="bullet">
    /// <item><description>Key Exchange: curve25519-sha256, ECDH with NIST curves, Diffie-Hellman group exchange</description></item>
    /// <item><description>Host Keys: Ed25519, ECDSA, RSA-SHA2</description></item>
    /// <item><description>Ciphers: ChaCha20-Poly1305, AES-GCM, AES-CTR</description></item>
    /// <item><description>MACs: HMAC-SHA2-256/512 with encrypt-then-MAC</description></item>
    /// <item><description>Compression: None</description></item>
    /// </list>
    /// <para>This method must be called before connecting to the server. The session must be in <see cref="SshConnectionStatus.Disconnected"/> status.</para>
    /// </remarks>
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

    /// <summary>
    /// Disables the session timeout, allowing operations to wait indefinitely.
    /// </summary>
    /// <remarks>
    /// <para>By default, libssh2 has no timeout. Use this method to explicitly disable any previously set timeout.</para>
    /// <para>The session must be in <see cref="SshConnectionStatus.Connected"/> or <see cref="SshConnectionStatus.LoggedIn"/> status before calling this method.</para>
    /// </remarks>
    public unsafe void DisableSessionTimeout()
    {
        EnsureInitialized();
        EnsureInStatuses(SshConnectionStatus.Connected, SshConnectionStatus.LoggedIn);
        
        libssh2_session_set_timeout(SessionPtr, 0);
    }

    /// <summary>
    /// Sets the maximum time to wait for SSH operations to complete.
    /// </summary>
    /// <param name="timeout">The timeout duration. Must be greater than zero and less than <see cref="int.MaxValue"/> milliseconds.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the timeout is negative or exceeds the maximum allowed value.</exception>
    /// <remarks>
    /// <para>This timeout applies to blocking libssh2 operations. If an operation does not complete within the specified time, it will return an error.</para>
    /// <para>The session must be in <see cref="SshConnectionStatus.Connected"/> or <see cref="SshConnectionStatus.LoggedIn"/> status before calling this method.</para>
    /// </remarks>
    public unsafe void SetSessionTimeout(TimeSpan timeout)
    {
        if (timeout < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "Timeout must be greater than zero");
        
        if(timeout.TotalMilliseconds > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "Timeout cannot be greater than ");
     
        EnsureInitialized();
        EnsureInStatuses(SshConnectionStatus.Connected, SshConnectionStatus.LoggedIn);
        
        libssh2_session_set_timeout(SessionPtr, (int) timeout.TotalMilliseconds);
    }

    /// <summary>
    /// Sends a keepalive message to the remote SSH server.
    /// </summary>
    /// <returns>The number of seconds until the next keepalive should be sent, based on the configured interval.</returns>
    /// <exception cref="SshException">Thrown if the keepalive message cannot be sent or the session is not in the correct state.</exception>
    /// <remarks>
    /// <para>This method sends an SSH protocol keepalive message to prevent the connection from timing out due to inactivity.</para>
    /// <para>This method must be called after connecting to the server. The session must be in <see cref="SshConnectionStatus.Connected"/> or <see cref="SshConnectionStatus.LoggedIn"/> status.</para>
    /// <para>Use <see cref="ConfigureKeepAlive"/> to set the keepalive interval and whether the server should reply.</para>
    /// </remarks>
    public unsafe int SendKeepAlive()
    {
        EnsureInitialized();
        EnsureInStatuses(SshConnectionStatus.Connected, SshConnectionStatus.LoggedIn);

        var untilNext = 0;
        var result = libssh2_keepalive_send(SessionPtr, &untilNext);

        result.ThrowIfNotSuccessful(this, "Failed to send keepalive");

        return untilNext;
    }

    /// <summary>
    /// Configures the SSH session's keepalive behavior.
    /// </summary>
    /// <param name="wantReply">If true, the server will be requested to send a reply to keepalive messages. If false, keepalive messages are sent without expecting a reply.</param>
    /// <param name="interval">The interval between keepalive messages. Must be greater than or equal to zero.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the interval is negative.</exception>
    /// <exception cref="SshException">Thrown if the session is not in the correct state.</exception>
    /// <remarks>
    /// <para>This method configures the keepalive settings for the SSH session. Keepalive messages help maintain connections that might otherwise timeout due to inactivity or be terminated by firewalls.</para>
    /// <para>This method must be called after connecting to the server. The session must be in <see cref="SshConnectionStatus.Connected"/> or <see cref="SshConnectionStatus.LoggedIn"/> status.</para>
    /// <para>Use <see cref="SendKeepAlive"/> to manually send keepalive messages once configured.</para>
    /// </remarks>
    public unsafe void ConfigureKeepAlive(bool wantReply, TimeSpan interval)
    {
        if (interval < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(interval), interval, "Interval must be greater than zero");

        EnsureInitialized();
        EnsureInStatuses(SshConnectionStatus.Connected, SshConnectionStatus.LoggedIn);

        libssh2_keepalive_config(SessionPtr, wantReply ? 1 : 0, (uint) interval.TotalSeconds);
    }
    
    /// <summary>
    /// Computes a cryptographic hash of the server's host key for verification purposes.
    /// </summary>
    /// <param name="keyHashType">The hash algorithm to use (e.g., <see cref="SshHashType.SHA256"/>).</param>
    /// <returns>A byte array containing the computed hash value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the keyHashType is not a valid <see cref="SshHashType"/> value.</exception>
    /// <exception cref="SshException">Thrown if the session is not connected or the hash cannot be computed.</exception>
    /// <remarks>
    /// <para>This method must be called after connecting to the server. The session must be in <see cref="SshConnectionStatus.Connected"/> or <see cref="SshConnectionStatus.LoggedIn"/> status.</para>
    /// <para>The returned hash can be used to verify the server's identity by comparing it against a known-good fingerprint. SHA-256 is recommended for new implementations.</para>
    /// </remarks>
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
        
        var hash = libssh2_hostkey_hash(SessionPtr, (int) keyHashType);

        if (hash == null)
            throw SshException.FromLastSessionError(SessionPtr);
        
        var keyHash = new byte[keySize];
        Marshal.Copy(new IntPtr(hash), keyHash, 0, keySize);
        
        return keyHash;
    }

    /// <summary>
    /// Asynchronously connects to an SSH server at the specified host and port.
    /// </summary>
    /// <param name="host">The hostname or IP address of the SSH server.</param>
    /// <param name="port">The port number of the SSH server (typically 22).</param>
    /// <param name="socketTimeout">Optional timeout for socket send and receive operations. If not specified, the socket will have no timeout (infinite). Must be greater than zero and less than <see cref="int.MaxValue"/> milliseconds.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when socketTimeout is negative or exceeds <see cref="int.MaxValue"/> milliseconds.</exception>
    /// <exception cref="SshException">Thrown when connection or SSH handshake fails.</exception>
    /// <remarks>
    /// <para>This method offloads the blocking connection and handshake operations to a thread pool thread.</para>
    /// <para>The session must be in <see cref="SshConnectionStatus.Disconnected"/> status before calling this method.</para>
    /// <para>After successful connection, the session will be in <see cref="SshConnectionStatus.Connected"/> status.</para>
    /// <para>You must call <see cref="AuthenticateAsync"/> or <see cref="Authenticate"/> before executing commands.</para>
    /// <para>The <paramref name="socketTimeout"/> parameter controls the underlying TCP socket's send and receive timeout values. This is separate from the SSH session timeout configured via <see cref="SetSessionTimeout"/>. Socket timeouts affect low-level network operations, while session timeouts affect SSH protocol operations.</para>
    /// </remarks>
    public Task ConnectAsync(string host, int port, TimeSpan? socketTimeout = null, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Connect(host, port, socketTimeout), cancellationToken);
    }

    /// <summary>
    /// Executes a command on the remote SSH server.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="options">Optional execution options including PTY settings. If null, uses default options.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>The result of the command execution including stdout and stderr output.</returns>
    /// <remarks>
    /// <para>The session must be in <see cref="SshConnectionStatus.LoggedIn"/> status before calling this method.</para>
    /// <para>When <see cref="CommandExecutionOptions.RequestPty"/> is true, a pseudo-terminal (PTY) will be requested
    /// before executing the command. This enables terminal features like color output and interactive input.</para>
    /// </remarks>
    public unsafe SshCommandResult ExecuteCommand(string command, CommandExecutionOptions? options = null, CancellationToken cancellationToken = default)
    {
        EnsureInitialized();
        EnsureInStatus(SshConnectionStatus.LoggedIn);
        
        options ??= CommandExecutionOptions.Default;

        logger?.LogDebug("Opening channel for command execution: '{Command}'", command);
        
        var channel = libssh2_channel_open_ex(
            SessionPtr,
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
            throw SshException.FromLastSessionError(SessionPtr);
        }

        logger?.LogDebug("Channel opened successfully");

        try
        {
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

                ptyResult.ThrowIfNotSuccessful(this, "Failed to request PTY");

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

            processStartupResult.ThrowIfNotSuccessful(this, "Unable to execute command");

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

            return new SshCommandResult
            {
                Successful = true,
                Stdout = stdout,
                Stderr = stderr,
                ExitCode = exitStatus,
                ExitSignal = exitSignal
            };
        }
        finally
        {
            libssh2_channel_free(channel);
            logger?.LogDebug("Channel freed");
        }
    }

    /// <summary>
    /// Asynchronously executes a command on the remote SSH server.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="options">Optional execution options including PTY settings. If null, uses default options.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the command execution including stdout and stderr output.</returns>
    /// <remarks>
    /// <para>This method offloads the blocking command execution to a thread pool thread.</para>
    /// <para>The session must be in <see cref="SshConnectionStatus.LoggedIn"/> status before calling this method.</para>
    /// <para>When <see cref="CommandExecutionOptions.RequestPty"/> is true, a pseudo-terminal (PTY) will be requested
    /// before executing the command. This enables terminal features like color output and interactive input.</para>
    /// </remarks>
    public Task<SshCommandResult> ExecuteCommandAsync(string command, CommandExecutionOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => ExecuteCommand(command, options, cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Executes a command on the remote SSH server and returns streams for stdout and stderr.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="options">Optional execution options including PTY settings. If null, uses default options.</param>
    /// <returns>A <see cref="SshCommandStream"/> providing streaming access to stdout and stderr.</returns>
    /// <remarks>
    /// <para>Unlike <see cref="ExecuteCommand"/>, this method does not buffer the output in memory.
    /// Instead, it returns streams that read directly from the SSH channel.</para>
    /// <para>The session must be in <see cref="SshConnectionStatus.LoggedIn"/> status before calling this method.</para>
    /// <para>The returned <see cref="SshCommandStream"/> must be disposed when you are done to release resources.</para>
    /// <para>You should consume the <see cref="SshCommandStream.Stdout"/> and <see cref="SshCommandStream.Stderr"/> streams
    /// before calling <see cref="SshCommandStream.WaitForExit"/> to get the exit code.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// using var stream = session.ExecuteCommandStreaming("cat /large/file.txt");
    /// 
    /// // Stream stdout to a file without buffering in memory
    /// using var file = File.Create("output.txt");
    /// stream.Stdout.CopyTo(file);
    /// 
    /// // Get exit code after consuming streams
    /// var result = stream.WaitForExit();
    /// Console.WriteLine($"Exit code: {result.ExitCode}");
    /// </code>
    /// </example>
    public unsafe SshCommandStream ExecuteCommandStreaming(string command, CommandExecutionOptions? options = null)
    {
        EnsureInitialized();
        EnsureInStatus(SshConnectionStatus.LoggedIn);
        
        options ??= CommandExecutionOptions.Default;

        logger?.LogDebug("Opening channel for streaming command execution: '{Command}'", command);
        
        var channel = libssh2_channel_open_ex(
            SessionPtr,
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
            throw SshException.FromLastSessionError(SessionPtr);
        }

        logger?.LogDebug("Channel opened successfully");

        try
        {
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

                ptyResult.ThrowIfNotSuccessful(this, "Failed to request PTY", also: () =>
                {
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

            processStartupResult.ThrowIfNotSuccessful(this, "Unable to execute command", also: () =>
            {
                libssh2_channel_free(channel);
            });

            logger?.LogDebug("Command process started, returning streams");
            
            // Transfer ownership of the channel to SshCommandStream
            return new SshCommandStream(channel);
        }
        catch
        {
            // If anything goes wrong after opening the channel, clean it up
            libssh2_channel_free(channel);
            throw;
        }
    }

    /// <summary>
    /// Asynchronously executes a command on the remote SSH server and returns streams for stdout and stderr.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="options">Optional execution options including PTY settings. If null, uses default options.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a <see cref="SshCommandStream"/> providing streaming access to stdout and stderr.</returns>
    /// <remarks>
    /// <para>This method offloads the blocking channel setup to a thread pool thread.</para>
    /// <para>Unlike <see cref="ExecuteCommandAsync"/>, this method does not buffer the output in memory.
    /// Instead, it returns streams that read directly from the SSH channel.</para>
    /// <para>The session must be in <see cref="SshConnectionStatus.LoggedIn"/> status before calling this method.</para>
    /// <para>The returned <see cref="SshCommandStream"/> must be disposed when you are done to release resources.</para>
    /// </remarks>
    public Task<SshCommandStream> ExecuteCommandStreamingAsync(string command, CommandExecutionOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => ExecuteCommandStreaming(command, options), cancellationToken);
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
    /// <para>The session must be in <see cref="SshConnectionStatus.LoggedIn"/> status before calling this method.</para>
    /// <para>This method does not close the destination stream. The caller is responsible for managing the stream's lifecycle.</para>
    /// <para>The method uses SCP (Secure Copy Protocol) to transfer the file.</para>
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
            SessionPtr,
            remotePathBuffer.AsPointer<sbyte>(),
            statBuffer.AsPointer());

        if (scpChannel == null)
        {
            logger?.LogDebug("Failed to open SCP receive channel");
            throw SshException.FromLastSessionError(SessionPtr);
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
    /// <para>This method offloads the blocking SCP file transfer to a thread pool thread.</para>
    /// <para>The session must be in <see cref="SshConnectionStatus.LoggedIn"/> status before calling this method.</para>
    /// <para>The destination stream will not be closed by this method. The caller is responsible for managing the stream's lifecycle.</para>
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
    /// <para>The session must be in <see cref="SshConnectionStatus.LoggedIn"/> status before calling this method.</para>
    /// <para>This method does not close the source stream. The caller is responsible for managing the stream's lifecycle.</para>
    /// <para>The method uses SCP (Secure Copy Protocol) to transfer the file.</para>
    /// <para>The source stream must be seekable because the total file size must be known before transmission begins.</para>
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
            SessionPtr,
            remotePathBuffer.AsPointer<sbyte>(),
            mode,
            fileSize,
            0,
            0);

        if (scpChannel == null)
        {
            logger?.LogDebug("Failed to open SCP send channel");
            throw SshException.FromLastSessionError(SessionPtr);
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
    /// <para>This method offloads the blocking SCP file transfer to a thread pool thread.</para>
    /// <para>The session must be in <see cref="SshConnectionStatus.LoggedIn"/> status before calling this method.</para>
    /// <para>The source stream will not be closed by this method. The caller is responsible for managing the stream's lifecycle.</para>
    /// <para>The source stream must be seekable because the total file size must be known before transmission begins.</para>
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
    public bool Authenticate(SshCredential credential)
    {
        EnsureInitialized();
        EnsureInStatus(SshConnectionStatus.Connected);

        logger?.LogDebug("Starting authentication with credential type: {Name}", credential.GetType().Name);
        var result = credential.Authenticate(this);

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

#if NET5_0_OR_GREATER
    /// <summary>
    /// Sets the trace level bitmask and handler for libssh2 debugging output.
    /// </summary>
    /// <param name="traceLevel">A combination of <see cref="SshTraceLevel"/> flags indicating which categories to trace.</param>
    /// <param name="handler">A callback function that receives trace messages. Pass null to disable tracing.</param>
    /// <remarks>
    /// <para>The session must be in <see cref="SshConnectionStatus.Connected"/> or <see cref="SshConnectionStatus.LoggedIn"/> status.</para>
    /// <para>This method is only available on .NET 5.0 or later.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// session.SetTrace(SshTraceLevel.Authentication | SshTraceLevel.Error,
    ///     message => Console.WriteLine($"[SSH] {message}"));
    /// </code>
    /// </example>
    public unsafe void SetTrace(SshTraceLevel traceLevel, Action<string>? handler)
    {
        EnsureInitialized();
        EnsureInStatuses(SshConnectionStatus.Connected, SshConnectionStatus.LoggedIn);

        var sessionKey = (nint)SessionPtr;

        lock (TraceHandlersLock)
        {
            if (handler == null || traceLevel == SshTraceLevel.None)
            {
                TraceHandlers.Remove(sessionKey);
                libssh2_trace(SessionPtr, 0);
                libssh2_trace_sethandler(SessionPtr, null, null);
            }
            else
            {
                TraceHandlers[sessionKey] = handler;
                libssh2_trace(SessionPtr, (int)traceLevel);
                libssh2_trace_sethandler(SessionPtr, (void*)sessionKey, &NativeTraceCallback);
            }
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void NativeTraceCallback(_LIBSSH2_SESSION* session, void* context, sbyte* message, nuint length)
    {
        Action<string>? handler;
        lock (TraceHandlersLock)
        {
            TraceHandlers.TryGetValue((nint)context, out handler);
        }

        if (handler == null) return;

        try
        {
            var msg = length > 0 ? Marshal.PtrToStringUTF8((IntPtr)message, (int)length) : string.Empty;
            handler(msg ?? string.Empty);
        }
        catch
        {
            // Swallow exceptions to prevent crashes in native code
        }
    }
#endif

    /// <summary>
    /// Gets the last error message from the libssh2 session (for debugging).
    /// </summary>
    internal unsafe string GetLastError()
    {
        if (SessionPtr == null)
            return "Session is null";

        sbyte* errorMsg = null;
        var errorMsgLen = 0;
        var errorCode = libssh2_session_last_error(SessionPtr, &errorMsg, &errorMsgLen, 0);

        if (errorCode == 0 || errorMsg == null)
            return "No error";

        return Marshal.PtrToStringUTF8((IntPtr)errorMsg) ?? "Unknown error";
    }

    /// <inheritdoc />
    public unsafe void Dispose()
    {
        if(ConnectionStatus == SshConnectionStatus.Disposed)
            return;

        if (SessionPtr != null)
        {
#if NET5_0_OR_GREATER
            // Clean up trace handler
            lock (TraceHandlersLock)
            {
                TraceHandlers.Remove((nint)SessionPtr);
            }
#endif

            _ = libssh2_session_disconnect_ex(SessionPtr, SSH_DISCONNECT_BY_APPLICATION, StringPointers.SessionDisposed, null);
            _ = libssh2_session_free(SessionPtr);
            SessionPtr = null;
        }

        if (socket != null)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                // Socket may already be closed, ignore errors
            }
            socket.Dispose();
        }

        ConnectionStatus = SshConnectionStatus.Disposed;
    }
}