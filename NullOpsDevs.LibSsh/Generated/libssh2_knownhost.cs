namespace NullOpsDevs.LibSsh.Generated;

public unsafe partial struct libssh2_knownhost
{
    [NativeTypeName("unsigned int")]
    public uint magic;

    public void* node;

    [NativeTypeName("char *")]
    public sbyte* name;

    [NativeTypeName("char *")]
    public sbyte* key;

    public int typemask;
}
