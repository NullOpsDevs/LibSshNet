using System.Runtime.InteropServices;

namespace NullOpsDevs.LibSsh.Generated;

public unsafe partial struct _LIBSSH2_POLLFD
{
    [NativeTypeName("unsigned char")]
    public byte type;

    [NativeTypeName("__AnonymousRecord_libssh2_L452_C5")]
    public _fd_e__Union fd;

    [NativeTypeName("unsigned long")]
    public uint events;

    [NativeTypeName("unsigned long")]
    public uint revents;

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct _fd_e__Union
    {
        [FieldOffset(0)]
        [NativeTypeName("libssh2_socket_t")]
        public ulong socket;

        [FieldOffset(0)]
        [NativeTypeName("LIBSSH2_CHANNEL *")]
        public _LIBSSH2_CHANNEL* channel;

        [FieldOffset(0)]
        [NativeTypeName("LIBSSH2_LISTENER *")]
        public _LIBSSH2_LISTENER* listener;
    }
}
