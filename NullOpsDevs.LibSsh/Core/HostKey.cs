namespace NullOpsDevs.LibSsh.Core;

/// <summary>
/// Represents an SSH server's host key used for server authentication and verification.
/// </summary>
public readonly struct SshHostKey
{
    /// <summary>
    /// Gets the raw host key data as a byte array.
    /// </summary>
    public byte[] Key { get; init;  }

    /// <summary>
    /// Gets the type of the host key algorithm (e.g., RSA, Ed25519, ECDSA).
    /// </summary>
    public SshHostKeyType Type { get; init; }
}