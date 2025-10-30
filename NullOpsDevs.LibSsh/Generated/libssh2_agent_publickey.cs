namespace NullOpsDevs.LibSsh.Generated;

public unsafe partial struct libssh2_agent_publickey
{
    [NativeTypeName("unsigned int")]
    public uint magic;

    public void* node;

    [NativeTypeName("unsigned char *")]
    public byte* blob;

    [NativeTypeName("size_t")]
    public nuint blob_len;

    [NativeTypeName("char *")]
    public sbyte* comment;
}
