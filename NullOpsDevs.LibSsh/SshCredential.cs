using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh;

/// <summary>
/// Abstract base class for SSH authentication credentials.
/// </summary>
public abstract unsafe class SshCredential
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
    public abstract bool Authenticate(_LIBSSH2_SESSION* session);

    /// <summary>
    /// Creates a password-based SSH credential.
    /// </summary>
    /// <param name="username">The username for authentication.</param>
    /// <param name="password">The password for authentication.</param>
    /// <returns>A password-based SSH credential.</returns>
    public static SshCredential FromPassword(string username, string password) => new SshPasswordCredential(username, password);
}