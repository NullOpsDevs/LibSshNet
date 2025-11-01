using System.Text;
using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh;

/// <summary>
/// Provides utilities for reading data from SSH channel streams.
/// </summary>
internal static class ChannelReader
{
    /// <summary>
    /// Stream ID for standard output (stdout).
    /// </summary>
    public const int StdoutStreamId = 0;

    /// <summary>
    /// Stream ID for standard error (stderr).
    /// </summary>
    public const int StderrStreamId = 1;

    /// <summary>
    /// Reads all data from a channel stream and decodes it as a UTF-8 string.
    /// </summary>
    /// <param name="channel">The libssh2 channel pointer.</param>
    /// <param name="streamId">The stream ID to read from (0 for stdout, 1 for stderr).</param>
    /// <param name="cancellationToken">Cancellation token to cancel the read operation.</param>
    /// <returns>The UTF-8 decoded string from the channel stream.</returns>
    public static unsafe string ReadUtf8String(_LIBSSH2_CHANNEL* channel, int streamId, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream(4096);
        CopyToStream(channel, streamId, memoryStream, cancellationToken: cancellationToken);
        
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    /// <summary>
    /// Reads data from a libssh2 channel stream and copies it to a destination stream.
    /// </summary>
    /// <param name="channel">The libssh2 channel pointer to read from.</param>
    /// <param name="streamId">The stream ID to read from (0 for stdout, 1 for stderr).</param>
    /// <param name="destination">The destination stream to write the data to.</param>
    /// <param name="bufferSize">The size of the buffer to use for reading. Default is 4096 bytes.</param>
    /// <param name="expectedSize">Optional expected number of bytes to read. If null, reads until the channel returns 0 bytes.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the read operation.</param>
    /// <returns>The total number of bytes read from the channel and written to the destination stream.</returns>
    /// <remarks>
    /// This method reads data in chunks using the specified buffer size. It continues reading until
    /// either the expected size is reached (if specified), the channel returns 0 bytes indicating EOF,
    /// or the operation is cancelled.
    /// </remarks>
    public static unsafe long CopyToStream(_LIBSSH2_CHANNEL* channel, int streamId, Stream destination, int bufferSize = 4096, int? expectedSize = null, CancellationToken cancellationToken = default)
    {
        using var buffer = NativeBuffer.Allocate(bufferSize);
        var totalBytesRead = 0L;

        while (expectedSize == null || totalBytesRead < expectedSize)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var bytesRead = LibSshNative.libssh2_channel_read_ex(channel, streamId, buffer.AsPointer<sbyte>(), (nuint)buffer.Length);

            if (bytesRead > 0)
            {
                // Limit bytes to write if we have an expected size
                var bytesToWrite = bytesRead;
                if (expectedSize.HasValue)
                {
                    var remaining = expectedSize.Value - totalBytesRead;
                    bytesToWrite = (int)Math.Min(bytesRead, remaining);
                }

                totalBytesRead += bytesToWrite;
                destination.Write(new ReadOnlySpan<byte>(buffer.AsPointer<byte>(), (int)bytesToWrite));

                // If we've written exactly the expected amount, stop reading
                if (expectedSize.HasValue && totalBytesRead >= expectedSize.Value)
                    break;
            }
            else if (bytesRead == 0)
            {
                break;
            }
        }

        return totalBytesRead;
    }

    /// <summary>
    /// Writes data from a source stream to a libssh2 channel stream.
    /// </summary>
    /// <param name="channel">The libssh2 channel pointer to write to.</param>
    /// <param name="streamId">The stream ID to write to (typically 0 for standard input).</param>
    /// <param name="source">The source stream to read data from.</param>
    /// <param name="bytesToWrite">The total number of bytes to write to the channel.</param>
    /// <param name="bufferSize">The size of the buffer to use for writing. Default is 4096 bytes.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the write operation.</param>
    /// <returns>The total number of bytes successfully written to the channel. Returns 0 if a write error occurs.</returns>
    /// <remarks>
    /// This method reads data from the source stream in chunks and writes it to the channel.
    /// It handles partial writes by continuing to write remaining data from the current buffer.
    /// If a write operation returns a negative value (indicating an error), the method returns 0.
    /// The operation continues until all bytes are written, the source stream ends, or the operation is cancelled.
    /// </remarks>
    public static unsafe long CopyToChannel(_LIBSSH2_CHANNEL* channel, int streamId, Stream source, long bytesToWrite, int bufferSize = 4096, CancellationToken cancellationToken = default)
    {
        using var buffer = NativeBuffer.Allocate(bufferSize);
        var totalBytesWritten = 0L;

        while (totalBytesWritten < bytesToWrite)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var bytesRead = source.Read(buffer.Span);

            if (bytesRead == 0)
                break;

            var offset = 0;
            while (offset < bytesRead)
            {
                var bytesWritten = LibSshNative.libssh2_channel_write_ex(
                    channel,
                    streamId,
                    (sbyte*)(buffer.AsPointer<byte>() + offset),
                    (nuint)(bytesRead - offset));

                if (bytesWritten < 0)
                    return 0;

                offset += (int)bytesWritten;
                totalBytesWritten += bytesWritten;
            }
        }

        return totalBytesWritten;
    }
}