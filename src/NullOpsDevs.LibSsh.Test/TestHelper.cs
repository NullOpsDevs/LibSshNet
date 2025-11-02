using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using NullOpsDevs.LibSsh.Credentials;

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
        ILogger? logger = null;

        if (Environment.GetEnvironmentVariable("DEBUG_LOGGING") != null)
            logger = new AnsiConsoleLogger();
        
        var session = new SshSession(logger);
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
