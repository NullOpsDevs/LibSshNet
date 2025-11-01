using NullOpsDevs.LibSsh.Generated;
using NullOpsDevs.LibSsh.Interop;

namespace NullOpsDevs.LibSsh.Credentials;

/// <summary>
/// Represents SSH authentication using the SSH agent (ssh-agent on Unix, pageant on Windows).
/// </summary>
/// <param name="username">The username for authentication.</param>
/// <remarks>
/// This credential type connects to the running SSH agent and attempts to authenticate
/// using the identities available in the agent. The agent manages the private keys,
/// so no key files or passphrases need to be provided.
/// </remarks>
public class SshAgentCredential(string username) : SshCredential
{
    /// <inheritdoc />
    public override unsafe bool Authenticate(_LIBSSH2_SESSION* session)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        // Initialize the agent
        var agent = LibSshNative.libssh2_agent_init(session);
        if (agent == null)
            return false;

        try
        {
            // Connect to the agent
            var connectResult = LibSshNative.libssh2_agent_connect(agent);
            if (connectResult != 0)
                return false;

            try
            {
                // Request list of identities from the agent
                var listResult = LibSshNative.libssh2_agent_list_identities(agent);
                if (listResult != 0)
                    return false;

                using var usernameBuffer = NativeBuffer.Allocate(username);

                // Iterate through available identities
                libssh2_agent_publickey* identity = null;
                libssh2_agent_publickey* prevIdentity = null;

                while (true)
                {
                    var getIdentityResult = LibSshNative.libssh2_agent_get_identity(agent, &identity, prevIdentity);

                    if (getIdentityResult == 1) // No more identities
                        break;

                    if (getIdentityResult < 0) // Error
                        return false;

                    // Try to authenticate with this identity
                    var authResult = LibSshNative.libssh2_agent_userauth(
                        agent,
                        usernameBuffer.AsPointer<sbyte>(),
                        identity);

                    if (authResult == 0) // Success
                        return true;

                    prevIdentity = identity;
                }

                return false; // No identity worked
            }
            finally
            {
                LibSshNative.libssh2_agent_disconnect(agent);
            }
        }
        finally
        {
            LibSshNative.libssh2_agent_free(agent);
        }
    }
}
