namespace NullOpsDevs.LibSsh.Generated;

internal struct _LIBSSH2_SFTP_ATTRIBUTES
{
    [NativeTypeName("unsigned long")]
    public uint flags;

    [NativeTypeName("libssh2_uint64_t")]
    public ulong filesize;

    [NativeTypeName("unsigned long")]
    public uint uid;

    [NativeTypeName("unsigned long")]
    public uint gid;

    [NativeTypeName("unsigned long")]
    public uint permissions;

    [NativeTypeName("unsigned long")]
    public uint atime;

    [NativeTypeName("unsigned long")]
    public uint mtime;
}
