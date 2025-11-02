using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NullOpsDevs.LibSsh.Generated;
using NullOpsDevs.LibSsh.Interop;

namespace NullOpsDevs.LibSsh.Credentials;

/// <summary>
/// Represents SSH authentication using username and password.
/// </summary>
/// <param name="username">The username for authentication.</param>
/// <param name="password">The password for authentication.</param>
public class SshPasswordCredential(string username, string password) : SshCredential
{
    /// <inheritdoc />
    public override unsafe bool Authenticate(_LIBSSH2_SESSION* session)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        using var usernameBuffer = NativeBuffer.Allocate(username);
        using var passwordBuffer = NativeBuffer.Allocate(password);
        
        var authResult = LibSshNative.libssh2_userauth_password_ex(
            session,
            usernameBuffer.AsPointer<sbyte>(), (uint)usernameBuffer.Length,
            passwordBuffer.AsPointer<sbyte>(), (uint)passwordBuffer.Length, null);
        
        return authResult >= 0;
    }
}