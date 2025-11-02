namespace NullOpsDevs.LibSsh.Core;

public readonly struct SshHostKey
{
    public byte[] Key { get; init;  }
    public SshHostKeyType Type { get; init; }
}