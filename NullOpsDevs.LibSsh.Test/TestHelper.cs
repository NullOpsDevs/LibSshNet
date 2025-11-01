using System.Security.Cryptography;
using System.Text;

namespace NullOpsDevs.LibSsh.Test;

/// <summary>
/// Helper utilities for SSH testing
/// </summary>
public static class TestHelper
{
    /// <summary>
    /// Creates a new SSH session and connects to the test server
    /// </summary>
    public static SshSession CreateAndConnect()
    {
        var session = new SshSession();
        session.Connect(TestConfig.Host, TestConfig.Port);
        return session;
    }

    /// <summary>
    /// Creates a new SSH session, connects, and authenticates with password
    /// </summary>
    public static SshSession CreateConnectAndAuthenticate()
    {
        var session = CreateAndConnect();
        var credential = SshCredential.FromPassword(TestConfig.Username, TestConfig.Password);
        session.Authenticate(credential);
        return session;
    }

    /// <summary>
    /// Compares two streams for equality
    /// </summary>
    public static bool StreamsAreEqual(Stream stream1, Stream stream2)
    {
        stream1.Position = 0;
        stream2.Position = 0;

        if (stream1.Length != stream2.Length)
            return false;

        var buffer1 = new byte[4096];
        var buffer2 = new byte[4096];

        while (true)
        {
            var read1 = stream1.Read(buffer1, 0, buffer1.Length);
            var read2 = stream2.Read(buffer2, 0, buffer2.Length);

            if (read1 != read2)
                return false;

            if (read1 == 0)
                break;

            if (!buffer1.AsSpan(0, read1).SequenceEqual(buffer2.AsSpan(0, read2)))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Computes SHA256 hash of a stream
    /// </summary>
    public static string GetStreamHash(Stream stream)
    {
        stream.Position = 0;
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(stream);
        stream.Position = 0;
        return Convert.ToHexString(hash);
    }

    /// <summary>
    /// Creates a temporary file with random content
    /// </summary>
    public static string CreateTempFile(long sizeInBytes)
    {
        var path = Path.GetTempFileName();
        var random = new Random();
        var buffer = new byte[4096];

        using var fs = File.OpenWrite(path);
        
        var remaining = sizeInBytes;
        while (remaining > 0)
        {
            var toWrite = (int)Math.Min(buffer.Length, remaining);
            random.NextBytes(buffer.AsSpan(0, toWrite));
            fs.Write(buffer, 0, toWrite);
            remaining -= toWrite;
        }

        return path;
    }

    /// <summary>
    /// Generates random text content
    /// </summary>
    public static string GenerateRandomText(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var sb = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            sb.Append(chars[random.Next(chars.Length)]);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Waits for Docker containers to be ready
    /// </summary>
    public static async Task WaitForContainersAsync(TimeSpan timeout)
    {
        var start = DateTime.UtcNow;

        while (DateTime.UtcNow - start < timeout)
        {
            try
            {
                using var session = new SshSession();
                session.Connect(TestConfig.Host, TestConfig.Port);
                var credential = SshCredential.FromPassword(TestConfig.Username, TestConfig.Password);
                if (session.Authenticate(credential))
                {
                    return;
                }
            }
            catch
            {
                // Ignore and retry
            }

            await Task.Delay(1000);
        }

        throw new TimeoutException($"Docker containers did not become ready within {timeout.TotalSeconds} seconds");
    }

    /// <summary>
    /// Loads a key file into a byte array
    /// </summary>
    public static byte[] LoadKeyFile(string path)
    {
        return File.ReadAllBytes(TestConfig.GetKeyPath(path));
    }
}
