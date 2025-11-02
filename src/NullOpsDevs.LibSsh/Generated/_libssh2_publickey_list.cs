namespace NullOpsDevs.LibSsh.Generated;

internal unsafe struct _libssh2_publickey_list
{
    [NativeTypeName("unsigned char *")]
    public byte* packet;

    [NativeTypeName("const unsigned char *")]
    public byte* name;

    [NativeTypeName("unsigned long")]
    public uint name_len;

    [NativeTypeName("const unsigned char *")]
    public byte* blob;

    [NativeTypeName("unsigned long")]
    public uint blob_len;

    [NativeTypeName("unsigned long")]
    public uint num_attrs;

    [NativeTypeName("libssh2_publickey_attribute *")]
    public _libssh2_publickey_attribute* attrs;
}
