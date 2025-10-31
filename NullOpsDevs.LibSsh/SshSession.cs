using System.Net.Sockets;
using NullOpsDevs.LibSsh.Exceptions;
using NullOpsDevs.LibSsh.Extensions;
using NullOpsDevs.LibSsh.Generated;
using static NullOpsDevs.LibSsh.Generated.LibSshNative;

namespace NullOpsDevs.LibSsh;

public sealed class SshSession : IDisposable
{
    private static bool libraryInitialized;
    private readonly Lock localLock = new();
    
    private Socket? socket;
    private unsafe _LIBSSH2_SESSION* session;
    
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
            
            ConnectionStatus = SshConnectionStatus.Connected;
            session = newSession;
        }
    }

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
        socket?.Dispose();

        if (session != null)
        {
            _ = libssh2_session_free(session);
            session = null;
        }
    }
}