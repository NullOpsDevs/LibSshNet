namespace NullOpsDevs.LibSsh.Generated;

internal unsafe struct _libssh2_publickey_attribute
{
    [NativeTypeName("const char *")]
    public sbyte* name;

    [NativeTypeName("unsigned long")]
    public uint name_len;

    [NativeTypeName("const char *")]
    public sbyte* value;

    [NativeTypeName("unsigned long")]
    public uint value_len;

    [NativeTypeName("char")]
    public sbyte mandatory;
}
