namespace NullOpsDevs.LibSsh.Credentials;

/// <summary>
/// Abstract base class for SSH authentication credentials.
/// </summary>
public abstract class SshCredential
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SshCredential"/> class.
    /// </summary>
    protected SshCredential() {}

    /// <summary>
    /// Authenticates the session using this credential.
    /// </summary>
    /// <param name="session">The libssh2 session pointer.</param>
    /// <returns>True if authentication succeeded; false otherwise.</returns>
    public abstract bool Authenticate(SshSession session);

    /// <summary>
    /// Creates a password-based SSH credential.
    /// </summary>
    /// <param name="username">The username for authentication.</param>
    /// <param name="password">The password for authentication.</param>
    /// <returns>A password-based SSH credential.</returns>
    public static SshCredential FromPassword(string username, string password) => new SshPasswordCredential(username, password);

    /// <summary>
    /// Creates a public key-based SSH credential using key files.
    /// </summary>
    /// <param name="username">The username for authentication.</param>
    /// <param name="publicKeyPath">The path to the public key file.</param>
    /// <param name="privateKeyPath">The path to the private key file.</param>
    /// <param name="passphrase">Optional passphrase to decrypt the private key. Use null or empty string if no passphrase is required.</param>
    /// <returns>A public key-based SSH credential.</returns>
    public static SshCredential FromPublicKeyFile(string username, string publicKeyPath, string privateKeyPath, string? passphrase = null)
        => new SshPublicKeyCredential(username, publicKeyPath, privateKeyPath, passphrase);

    /// <summary>
    /// Creates a public key-based SSH credential using key data from memory.
    /// </summary>
    /// <param name="username">The username for authentication.</param>
    /// <param name="publicKeyData">The public key data as a byte array.</param>
    /// <param name="privateKeyData">The private key data as a byte array.</param>
    /// <param name="passphrase">Optional passphrase to decrypt the private key. Use null or empty string if no passphrase is required.</param>
    /// <returns>A public key-based SSH credential.</returns>
    public static SshCredential FromPublicKeyMemory(string username, byte[] publicKeyData, byte[] privateKeyData, string? passphrase = null)
        => new SshPublicKeyFromMemoryCredential(username, publicKeyData, privateKeyData, passphrase);

    /// <summary>
    /// Creates an SSH credential that uses the SSH agent for authentication.
    /// </summary>
    /// <param name="username">The username for authentication.</param>
    /// <returns>An SSH agent-based credential.</returns>
    /// <remarks>
    /// This credential type connects to the running SSH agent (ssh-agent on Unix, pageant on Windows)
    /// and attempts to authenticate using the identities available in the agent.
    /// </remarks>
    public static SshCredential FromAgent(string username) => new SshAgentCredential(username);

    /// <summary>
    /// Creates a host-based SSH credential.
    /// </summary>
    /// <param name="username">The username for authentication.</param>
    /// <param name="publicKeyPath">The path to the public key file.</param>
    /// <param name="privateKeyPath">The path to the private key file.</param>
    /// <param name="passphrase">Optional passphrase to decrypt the private key. Use null or empty string if no passphrase is required.</param>
    /// <param name="hostname">The hostname of the client machine.</param>
    /// <param name="localUsername">The local username on the client machine. If null or empty, uses the same value as username.</param>
    /// <returns>A host-based SSH credential.</returns>
    /// <remarks>
    /// Host-based authentication allows a host to authenticate on behalf of its users.
    /// This is typically used in trusted environments where the server trusts certain client hosts.
    /// This authentication method is rarely used in modern SSH deployments.
    /// </remarks>
    public static SshCredential FromHostBased(string username, string publicKeyPath, string privateKeyPath, string? passphrase, string hostname, string? localUsername = null)
        => new SshHostBasedCredential(username, publicKeyPath, privateKeyPath, passphrase, hostname, localUsername);
}