using System.Runtime.InteropServices;
using System.Text;
using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh.Test;

public static class Program
{
    private const string Host = "127.0.0.1";
    private const int Port = 2222;
    private const string Username = "user";
    private const string Password = "12345";

    public static unsafe void Main(string[] args)
    {
        Console.WriteLine("LibSSH2 .NET Bindings Test");
        Console.WriteLine("==========================\n");

        // Initialize libssh2
        int rc = LibSSH2.libssh2_init(0);
        if (rc != 0)
        {
            Console.WriteLine($"Failed to initialize libssh2: {rc}");
            return;
        }

        Console.WriteLine("libssh2 initialized successfully");

        try
        {
            // Create session
            _LIBSSH2_SESSION* session = LibSSH2.libssh2_session_init_ex(null, null, null, null);
            if (session == null)
            {
                Console.WriteLine("Failed to create session");
                return;
            }

            Console.WriteLine("Session created");

            // Connect to server
            Console.WriteLine($"Connecting to {Host}:{Port}...");
            var socket = ConnectSocket(Host, Port);
            if (socket == null)
            {
                Console.WriteLine("Failed to connect socket");
                LibSSH2.libssh2_session_free(session);
                return;
            }

            Console.WriteLine("Socket connected");

            // Start SSH handshake
            rc = LibSSH2.libssh2_session_handshake(session, (ulong) socket.Handle);
            if (rc != 0)
            {
                Console.WriteLine($"SSH handshake failed: {rc}");
                socket.Close();
                LibSSH2.libssh2_session_free(session);
                return;
            }

            Console.WriteLine("SSH handshake completed");

            // Authenticate with password
            Console.WriteLine($"Authenticating as {Username}...");
            fixed (byte* userBytes = Encoding.UTF8.GetBytes(Username))
            fixed (byte* passBytes = Encoding.UTF8.GetBytes(Password))
            {
                rc = LibSSH2.libssh2_userauth_password_ex(
                    session,
                    (sbyte*)userBytes,
                    (uint)Username.Length,
                    (sbyte*)passBytes,
                    (uint)Password.Length,
                    null
                );
            }

            if (rc != 0)
            {
                Console.WriteLine($"Authentication failed: {rc}");
                socket.Close();
                LibSSH2.libssh2_session_free(session);
                return;
            }

            Console.WriteLine("Authentication successful!\n");

            // Execute a command
            ExecuteCommand(session, "whoami");
            ExecuteCommand(session, "pwd");
            ExecuteCommand(session, "ls -la");

            // Cleanup
            Console.WriteLine("\nDisconnecting...");
            LibSSH2.libssh2_session_disconnect_ex(
                session,
                0x000B,
                (sbyte*)Marshal.StringToHGlobalAnsi("Normal Shutdown"),
                (sbyte*)null
            );

            LibSSH2.libssh2_session_free(session);
            socket.Close();
        }
        finally
        {
            LibSSH2.libssh2_exit();
            Console.WriteLine("libssh2 cleaned up");
        }
    }

    private static unsafe void ExecuteCommand(_LIBSSH2_SESSION* session, string command)
    {
        Console.WriteLine($"\n> {command}");
        Console.WriteLine(new string('-', 50));

        // Open channel
        _LIBSSH2_CHANNEL* channel = LibSSH2.libssh2_channel_open_ex(
            session,
            (sbyte*)Marshal.StringToHGlobalAnsi("session"),
            7,
            0x20000,  // LIBSSH2_CHANNEL_WINDOW_DEFAULT
            0x8000,   // LIBSSH2_CHANNEL_PACKET_DEFAULT
            null,
            0
        );

        if (channel == null)
        {
            Console.WriteLine("Failed to open channel");
            return;
        }

        // Execute command
        fixed (byte* cmdBytes = Encoding.UTF8.GetBytes(command))
        {
            int rc = LibSSH2.libssh2_channel_process_startup(
                channel,
                (sbyte*)Marshal.StringToHGlobalAnsi("exec"),
                4,
                (sbyte*)cmdBytes,
                (uint)command.Length
            );

            if (rc != 0)
            {
                Console.WriteLine($"Failed to execute command: {rc}");
                LibSSH2.libssh2_channel_free(channel);
                return;
            }
        }

        // Read output
        byte[] buffer = new byte[4096];
        while (true)
        {
            fixed (byte* bufPtr = buffer)
            {
                long bytesRead = LibSSH2.libssh2_channel_read_ex(channel, 0, (sbyte*)bufPtr, (nuint)buffer.Length);

                if (bytesRead > 0)
                {
                    string output = Encoding.UTF8.GetString(buffer, 0, (int)bytesRead);
                    Console.Write(output);
                }
                else if (bytesRead == 0)
                {
                    break;
                }
                else if (bytesRead == LibSSH2.LIBSSH2_ERROR_EAGAIN)
                {
                    Thread.Sleep(10);
                    continue;
                }
                else
                {
                    Console.WriteLine($"\nError reading: {bytesRead}");
                    break;
                }
            }
        }

        // Close channel
        LibSSH2.libssh2_channel_close(channel);
        LibSSH2.libssh2_channel_wait_closed(channel);
        LibSSH2.libssh2_channel_free(channel);
    }

    private static System.Net.Sockets.Socket? ConnectSocket(string host, int port)
    {
        try
        {
            var socket = new System.Net.Sockets.Socket(
                System.Net.Sockets.AddressFamily.InterNetwork,
                System.Net.Sockets.SocketType.Stream,
                System.Net.Sockets.ProtocolType.Tcp
            );

            socket.Connect(host, port);
            return socket;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Socket connection failed: {ex.Message}");
            return null;
        }
    }
}