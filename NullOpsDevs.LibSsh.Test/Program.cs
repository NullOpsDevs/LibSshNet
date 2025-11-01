using System.Text;

namespace NullOpsDevs.LibSsh.Test;

public static class Program
{
    private const string Host = "127.0.0.1";
    private const int Port = 2222;
    public static void Main()
    {
        if (!NativePreloader.Preload())
            return;

        LibSsh2.GlobalLogger = Console.WriteLine;

        using var sshConnection = new SshSession();
        sshConnection.Connect(Host, Port);
        sshConnection.Authenticate(SshCredential.FromPassword("user", "12345"));

        //Console.WriteLine(sshConnection.ExecuteCommand("ls -l").Stdout);

        var writeStream = new MemoryStream("Hello world!\n"u8.ToArray());
        var successWrite = sshConnection.WriteFile("/tmp/test.txt", writeStream);

        Console.WriteLine($"write -> {successWrite}");

        var stream = new MemoryStream();
        var success = sshConnection.ReadFile("/tmp/test.txt", stream);
        var text = Encoding.UTF8.GetString(stream.ToArray());
    }
}