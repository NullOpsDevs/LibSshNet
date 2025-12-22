using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh.Core;

/// <summary>
/// A read-only stream that reads from a libssh2 channel.
/// This stream does not own the channel and will not close it when disposed.
/// </summary>
internal sealed unsafe class SshChannelStream : Stream
{
    private readonly _LIBSSH2_CHANNEL* _channel;
    private readonly int _streamId;
    private bool _isDisposed;
    private bool _isEof;

    /// <summary>
    /// Creates a new channel stream for reading from the specified stream ID.
    /// </summary>
    /// <param name="channel">The libssh2 channel pointer.</param>
    /// <param name="streamId">The stream ID to read from (0 for stdout, 1 for stderr).</param>
    public SshChannelStream(_LIBSSH2_CHANNEL* channel, int streamId)
    {
        _channel = channel;
        _streamId = streamId;
    }

    /// <inheritdoc />
    public override bool CanRead => !_isDisposed;

    /// <inheritdoc />
    public override bool CanSeek => false;

    /// <inheritdoc />
    public override bool CanWrite => false;

    /// <inheritdoc />
    public override long Length => throw new NotSupportedException("Length is not supported on SshChannelStream.");

    /// <inheritdoc />
    public override long Position
    {
        get => throw new NotSupportedException("Position is not supported on SshChannelStream.");
        set => throw new NotSupportedException("Position is not supported on SshChannelStream.");
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), "Offset cannot be negative.");
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
        if (offset + count > buffer.Length)
            throw new ArgumentException("The sum of offset and count is greater than the buffer length.");
        
        return ReadCore(buffer.AsSpan(offset, count));
    }

#if NET6_0_OR_GREATER
    /// <inheritdoc />
    public override int Read(Span<byte> buffer)
    {
        return ReadCore(buffer);
    }
#endif

    private int ReadCore(Span<byte> buffer)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(SshChannelStream));

        if (_isEof || buffer.Length == 0)
            return 0;

        // Pin the managed buffer and read directly into it
        fixed (byte* bufferPtr = buffer)
        {
            var bytesRead = LibSshNative.libssh2_channel_read_ex(
                _channel,
                _streamId,
                (sbyte*)bufferPtr,
                (nuint)buffer.Length);

            if (bytesRead < 0)
            {
                // Error occurred - treat as EOF for stream purposes
                // The actual error can be retrieved via the session
                _isEof = true;
                return 0;
            }

            if (bytesRead == 0)
            {
                _isEof = true;
                return 0;
            }

            return (int)bytesRead;
        }
    }

    /// <inheritdoc />
    public override void Flush()
    {
        // No-op for read-only stream
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException("Seek is not supported on SshChannelStream.");
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        throw new NotSupportedException("SetLength is not supported on SshChannelStream.");
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException("Write is not supported on SshChannelStream.");
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        base.Dispose(disposing);
    }
}
