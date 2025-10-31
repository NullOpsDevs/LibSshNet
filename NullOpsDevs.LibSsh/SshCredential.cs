using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh;

public abstract unsafe class SshCredential
{
    protected SshCredential() {}

    public abstract bool Authenticate(_LIBSSH2_SESSION* session);
    
    public static SshCredential FromPassword(string username, string password) => new SshPasswordCredential(username, password);
}