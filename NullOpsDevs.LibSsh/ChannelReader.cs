using System.Text;
using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh;

public static class ChannelReader
{
    public const int StdoutStreamId = 0;
    public const int StderrStreamId = 1;
    
    public static unsafe string ReadUtf8String(_LIBSSH2_CHANNEL* channel, int streamId, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream(4096);
        using var buffer = NativeBuffer.Allocate(4096);

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var bytesRead = LibSshNative.libssh2_channel_read_ex(channel, streamId, buffer.AsPointer<sbyte>(), (nuint)buffer.Length);
            
            if (bytesRead > 0)
            {
                memoryStream.Write(new ReadOnlySpan<byte>(buffer.AsPointer<byte>(), (int)bytesRead));
            }
            else if (bytesRead == 0)
            {
                break;
            }
        }
        
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }
}