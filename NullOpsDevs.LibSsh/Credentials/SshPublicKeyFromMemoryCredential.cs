using NullOpsDevs.LibSsh.Generated;
using NullOpsDevs.LibSsh.Interop;

namespace NullOpsDevs.LibSsh.Credentials;

/// <summary>
/// Represents SSH authentication using public key from memory buffers.
/// </summary>
/// <param name="username">The username for authentication.</param>
/// <param name="publicKeyData">The public key data as a byte array.</param>
/// <param name="privateKeyData">The private key data as a byte array.</param>
/// <param name="passphrase">Optional passphrase to decrypt the private key. Use null or empty string if no passphrase is required.</param>
public class SshPublicKeyFromMemoryCredential(string username, byte[] publicKeyData, byte[] privateKeyData, string? passphrase = null) : SshCredential
{
    /// <inheritdoc />
    public override unsafe bool Authenticate(_LIBSSH2_SESSION* session)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        if (publicKeyData.Length == 0)
            return false;

        if (privateKeyData.Length == 0)
            return false;

        using var usernameBuffer = NativeBuffer.Allocate(username);
        using var publicKeyBuffer = NativeBuffer.Allocate(publicKeyData);
        using var privateKeyBuffer = NativeBuffer.Allocate(privateKeyData);
        
        using var passphraseBuffer = string.IsNullOrEmpty(passphrase) ?
            NativeBuffer.Allocate(0) :
            NativeBuffer.Allocate(passphrase);

        var authResult = LibSshNative.libssh2_userauth_publickey_frommemory(
            session,
            usernameBuffer.AsPointer<sbyte>(),
            (nuint)usernameBuffer.Length - 1,
            publicKeyBuffer.AsPointer<sbyte>(),
            (nuint)publicKeyBuffer.Length,
            privateKeyBuffer.AsPointer<sbyte>(),
            (nuint)privateKeyBuffer.Length,
            string.IsNullOrEmpty(passphrase) ? null : passphraseBuffer.AsPointer<sbyte>());

        return authResult >= 0;
    }
}
