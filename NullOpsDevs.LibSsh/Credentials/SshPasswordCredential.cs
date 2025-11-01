using System.Runtime.InteropServices;
using NullOpsDevs.LibSsh.Generated;

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
        
        var usernameBuffer = Marshal.StringToHGlobalAnsi(username);
        var passwordBuffer = Marshal.StringToHGlobalAnsi(password);
        
        var authResult = LibSshNative.libssh2_userauth_password_ex(
            session,
            (sbyte*) usernameBuffer, (uint)username.Length,
            (sbyte*) passwordBuffer, (uint)password.Length, null);
        
        Marshal.FreeHGlobal(usernameBuffer);
        Marshal.FreeHGlobal(passwordBuffer);
        
        return authResult >= 0;
    }
}