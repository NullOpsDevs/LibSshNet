using System.Runtime.InteropServices;
using JetBrains.Annotations;
using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh.Core;

/// <summary>
/// Represents a streaming command execution result that provides direct access to stdout and stderr streams.
/// This class owns the underlying SSH channel and will close it when disposed.
/// </summary>
/// <remarks>
/// <para>
/// Unlike <see cref="SshCommandResult"/> which buffers all output in memory, this class provides
/// streaming access to command output. This is useful for commands that produce large amounts of output
/// or when you want to process output as it arrives.
/// </para>
/// <para>
/// The <see cref="Stdout"/> and <see cref="Stderr"/> streams read directly from the SSH channel.
/// You must consume both streams before calling <see cref="WaitForExit"/> to get the exit code.
/// </para>
/// <para>
/// This class must be disposed when you are done to release the underlying SSH channel resources.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// using var commandStream = session.ExecuteCommandStreaming("cat /large/file.txt");
/// 
/// // Stream stdout directly to a file
/// using var fileStream = File.Create("output.txt");
/// commandStream.Stdout.CopyTo(fileStream);
/// 
/// // Get the exit code after consuming the streams
/// var exitCode = commandStream.WaitForExit();
/// </code>
/// </example>
[PublicAPI]
public sealed unsafe class SshCommandStream : IDisposable
{
    private readonly _LIBSSH2_CHANNEL* _channel;
    private readonly SshChannelStream _stdout;
    private readonly SshChannelStream _stderr;
    private bool _isDisposed;
    private bool _isClosed;

    internal SshCommandStream(_LIBSSH2_CHANNEL* channel)
    {
        _channel = channel;
        _stdout = new SshChannelStream(channel, ChannelReader.StdoutStreamId);
        _stderr = new SshChannelStream(channel, ChannelReader.StderrStreamId);
    }

    /// <summary>
    /// Gets the standard output stream from the command execution.
    /// </summary>
    /// <remarks>
    /// This stream reads directly from the SSH channel without buffering.
    /// The stream will return 0 bytes when EOF is reached.
    /// </remarks>
    public Stream Stdout => _stdout;

    /// <summary>
    /// Gets the standard error stream from the command execution.
    /// </summary>
    /// <remarks>
    /// This stream reads directly from the SSH channel without buffering.
    /// The stream will return 0 bytes when EOF is reached.
    /// </remarks>
    public Stream Stderr => _stderr;

    /// <summary>
    /// Waits for the command to complete and returns the exit information.
    /// </summary>
    /// <returns>The command result containing exit code and exit signal.</returns>
    /// <remarks>
    /// <para>
    /// You should consume the <see cref="Stdout"/> and <see cref="Stderr"/> streams before calling this method.
    /// This method closes the channel and waits for the remote side to acknowledge.
    /// </para>
    /// <para>
    /// After calling this method, the <see cref="Stdout"/> and <see cref="Stderr"/> streams will no longer be readable.
    /// </para>
    /// </remarks>
    public SshCommandResult WaitForExit()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(SshCommandStream));

        if (_isClosed)
            throw new InvalidOperationException("WaitForExit has already been called.");

        _isClosed = true;

        // Close the channel and wait for remote acknowledgment
        LibSshNative.libssh2_channel_close(_channel);
        LibSshNative.libssh2_channel_wait_closed(_channel);

        // Get exit status
        var exitStatus = LibSshNative.libssh2_channel_get_exit_status(_channel);

        // Get exit signal
        sbyte* exitSignalPtr = null;
        nuint exitSignalLen = 0;
        var exitSignalResult = LibSshNative.libssh2_channel_get_exit_signal(
            _channel, &exitSignalPtr, &exitSignalLen, null, null, null, null);

        string? exitSignal = null;
        if (exitSignalResult == 0 && exitSignalPtr != null && exitSignalLen > 0)
        {
            exitSignal = Marshal.PtrToStringUTF8((IntPtr)exitSignalPtr, (int)exitSignalLen);
        }

        return new SshCommandResult
        {
            Successful = true,
            ExitCode = exitStatus,
            ExitSignal = exitSignal
        };
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _stdout.Dispose();
        _stderr.Dispose();

        if (!_isClosed)
        {
            LibSshNative.libssh2_channel_close(_channel);
            LibSshNative.libssh2_channel_wait_closed(_channel);
        }

        LibSshNative.libssh2_channel_free(_channel);
        _isDisposed = true;
    }
}
