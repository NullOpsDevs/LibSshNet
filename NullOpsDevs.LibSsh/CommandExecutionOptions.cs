using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh;

public class CommandExecutionOptions
{
    public static readonly CommandExecutionOptions Default = new();
    
    public uint WindowSize { get; set; } = LibSshNative.LIBSSH2_CHANNEL_WINDOW_DEFAULT;
    
    public uint PacketSize { get; set; } = LibSshNative.LIBSSH2_CHANNEL_PACKET_DEFAULT;
}