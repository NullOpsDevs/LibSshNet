namespace NullOpsDevs.LibSsh.Generated;

public unsafe partial struct _LIBSSH2_USERAUTH_KBDINT_PROMPT
{
    [NativeTypeName("unsigned char *")]
    public byte* text;

    [NativeTypeName("size_t")]
    public nuint length;

    [NativeTypeName("unsigned char")]
    public byte echo;
}
