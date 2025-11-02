namespace NullOpsDevs.LibSsh.Generated;

internal unsafe struct _LIBSSH2_PRIVKEY_SK
{
    public int algorithm;

    [NativeTypeName("uint8_t")]
    public byte flags;

    [NativeTypeName("const char *")]
    public sbyte* application;

    [NativeTypeName("const unsigned char *")]
    public byte* key_handle;

    [NativeTypeName("size_t")]
    public nuint handle_len;

    [NativeTypeName("int (*)(LIBSSH2_SESSION *, LIBSSH2_SK_SIG_INFO *, const unsigned char *, size_t, int, uint8_t, const char *, const unsigned char *, size_t, void **)")]
    public delegate* unmanaged[Cdecl]<_LIBSSH2_SESSION*, _LIBSSH2_SK_SIG_INFO*, byte*, nuint, int, byte, sbyte*, byte*, nuint, void**, int> sign_callback;

    public void** orig_abstract;
}
