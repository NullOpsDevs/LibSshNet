namespace NullOpsDevs.LibSsh.Generated;

public unsafe partial struct _LIBSSH2_SK_SIG_INFO
{
    [NativeTypeName("uint8_t")]
    public byte flags;

    [NativeTypeName("uint32_t")]
    public uint counter;

    [NativeTypeName("unsigned char *")]
    public byte* sig_r;

    [NativeTypeName("size_t")]
    public nuint sig_r_len;

    [NativeTypeName("unsigned char *")]
    public byte* sig_s;

    [NativeTypeName("size_t")]
    public nuint sig_s_len;
}
