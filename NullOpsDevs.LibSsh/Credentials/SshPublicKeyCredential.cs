using NullOpsDevs.LibSsh.Generated;
using NullOpsDevs.LibSsh.Interop;

namespace NullOpsDevs.LibSsh.Credentials;

/// <summary>
/// Represents SSH authentication using public key from file paths.
/// </summary>
/// <param name="username">The username for authentication.</param>
/// <param name="publicKeyPath">The path to the public key file.</param>
/// <param name="privateKeyPath">The path to the private key file.</param>
/// <param name="passphrase">Optional passphrase to decrypt the private key. Use null or empty string if no passphrase is required.</param>
public class SshPublicKeyCredential(string username, string publicKeyPath, string privateKeyPath, string? passphrase = null) : SshCredential
{
    /// <inheritdoc />
    public override unsafe bool Authenticate(_LIBSSH2_SESSION* session)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        if (string.IsNullOrWhiteSpace(privateKeyPath))
            return false;

        using var usernameBuffer = NativeBuffer.Allocate(username);
        using var privateKeyPathBuffer = NativeBuffer.Allocate(privateKeyPath);
        using var passphraseBuffer = string.IsNullOrEmpty(passphrase)
            ? NativeBuffer.Allocate(0)
            : NativeBuffer.Allocate(passphrase);

        // Try without public key file first (let libssh2 extract it from private key)
        var authResult = LibSshNative.libssh2_userauth_publickey_fromfile_ex(
            session,
            usernameBuffer.AsPointer<sbyte>(),
            (uint)usernameBuffer.Length - 1,
            null,
            privateKeyPathBuffer.AsPointer<sbyte>(),
            string.IsNullOrEmpty(passphrase) ? null : passphraseBuffer.AsPointer<sbyte>());

        // If that fails and we have a public key path, try with explicit public key
        if (authResult < 0 && !string.IsNullOrWhiteSpace(publicKeyPath))
        {
            using var publicKeyPathBuffer = NativeBuffer.Allocate(publicKeyPath);
            authResult = LibSshNative.libssh2_userauth_publickey_fromfile_ex(
                session,
                usernameBuffer.AsPointer<sbyte>(),
                (uint)usernameBuffer.Length - 1,
                publicKeyPathBuffer.AsPointer<sbyte>(),
                privateKeyPathBuffer.AsPointer<sbyte>(),
                string.IsNullOrEmpty(passphrase) ? null : passphraseBuffer.AsPointer<sbyte>());
        }

        return authResult >= 0;
    }
}
