using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh;

/// <summary>
/// Represents SSH authentication using host-based authentication.
/// </summary>
/// <param name="username">The username for authentication.</param>
/// <param name="publicKeyPath">The path to the public key file.</param>
/// <param name="privateKeyPath">The path to the private key file.</param>
/// <param name="passphrase">Optional passphrase to decrypt the private key. Use null or empty string if no passphrase is required.</param>
/// <param name="hostname">The hostname of the client machine.</param>
/// <param name="localUsername">The local username on the client machine. If null or empty, uses the same value as username.</param>
/// <remarks>
/// Host-based authentication allows a host to authenticate on behalf of its users.
/// This is typically used in trusted environments where the server trusts certain client hosts.
/// This authentication method is rarely used in modern SSH deployments.
/// </remarks>
public class SshHostBasedCredential(
    string username,
    string publicKeyPath,
    string privateKeyPath,
    string? passphrase,
    string hostname,
    string? localUsername = null) : SshCredential
{
    /// <inheritdoc />
    public override unsafe bool Authenticate(_LIBSSH2_SESSION* session)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        if (string.IsNullOrWhiteSpace(publicKeyPath) || string.IsNullOrWhiteSpace(privateKeyPath))
            return false;

        if (string.IsNullOrWhiteSpace(hostname))
            return false;

        var effectiveLocalUsername = string.IsNullOrWhiteSpace(localUsername) ? username : localUsername;

        using var usernameBuffer = NativeBuffer.Allocate(username);
        using var publicKeyPathBuffer = NativeBuffer.Allocate(publicKeyPath);
        using var privateKeyPathBuffer = NativeBuffer.Allocate(privateKeyPath);
        using var passphraseBuffer = string.IsNullOrEmpty(passphrase)
            ? NativeBuffer.Allocate(0)
            : NativeBuffer.Allocate(passphrase);
        using var hostnameBuffer = NativeBuffer.Allocate(hostname);
        using var localUsernameBuffer = NativeBuffer.Allocate(effectiveLocalUsername);

        var authResult = LibSshNative.libssh2_userauth_hostbased_fromfile_ex(
            session,
            usernameBuffer.AsPointer<sbyte>(),
            (uint)usernameBuffer.Length,
            publicKeyPathBuffer.AsPointer<sbyte>(),
            privateKeyPathBuffer.AsPointer<sbyte>(),
            string.IsNullOrEmpty(passphrase) ? null : passphraseBuffer.AsPointer<sbyte>(),
            hostnameBuffer.AsPointer<sbyte>(),
            (uint)hostnameBuffer.Length,
            localUsernameBuffer.AsPointer<sbyte>(),
            (uint)localUsernameBuffer.Length);

        return authResult >= 0;
    }
}
