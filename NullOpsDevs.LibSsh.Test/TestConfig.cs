namespace NullOpsDevs.LibSsh.Test;

/// <summary>
/// Configuration constants for SSH testing
/// </summary>
public static class TestConfig
{
    // SSH Server connection
    public const string Host = "127.0.0.1";
    public const int Port = 2222;

    // Test credentials
    public const string Username = "user";
    public const string Password = "12345";

    // SSH key paths (relative to test project directory)
    public const string PrivateKeyPath = "docker/test-keys/id_rsa";
    public const string PublicKeyPath = "docker/test-keys/id_rsa.pub";
    public const string PrivateKeyProtectedPath = "docker/test-keys/id_rsa_protected";
    public const string PublicKeyProtectedPath = "docker/test-keys/id_rsa_protected.pub";
    public const string KeyPassphrase = "testpass";

    // Test file paths (remote paths on SSH server)
    public const string RemoteSmallFile = "/test-files/small.txt";
    public const string RemoteLargeFile = "/test-files/large.dat";

    /// <summary>
    /// Gets the absolute path to a test key file
    /// </summary>
    public static string GetKeyPath(string relativePath)
    {
        relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);
        
        // Get the directory where the test assembly is located
        var assemblyDir = AppContext.BaseDirectory;

        // First try the output directory (files are copied via .csproj)
        var outputPath = Path.Combine(assemblyDir, relativePath);
        if (File.Exists(outputPath))
            return outputPath;

        // Fallback: Navigate up to the project directory (from bin/Release/net9.0 or bin/Debug/net9.0)
        var projectDir = Path.Combine(assemblyDir, "..", "..", "..");
        var normalizedProjectDir = Path.GetFullPath(projectDir);

        return Path.Combine(normalizedProjectDir, relativePath);
    }
}
