using JetBrains.Annotations;
using NullOpsDevs.LibSsh.Generated;
using NullOpsDevs.LibSsh.Terminal;

namespace NullOpsDevs.LibSsh.Core;

/// <summary>
/// Options for SSH command execution.
/// </summary>
[PublicAPI]
public class CommandExecutionOptions
{
    /// <summary>
    /// Gets the default command execution options.
    /// </summary>
    public static readonly CommandExecutionOptions Default = new();

    /// <summary>
    /// Gets or sets the SSH channel window size (flow control buffer).
    /// Default is 2MB. This controls how much data the remote server can send before acknowledgment.
    /// </summary>
    public uint WindowSize { get; set; } = LibSshNative.LIBSSH2_CHANNEL_WINDOW_DEFAULT;

    /// <summary>
    /// Gets or sets the SSH channel packet size.
    /// Default is 32KB. This controls the maximum packet size for the channel.
    /// </summary>
    public uint PacketSize { get; set; } = LibSshNative.LIBSSH2_CHANNEL_PACKET_DEFAULT;

    /// <summary>
    /// Gets or sets whether to request a pseudo-terminal (PTY) for the command.
    /// Default is false. Enable this for commands that need terminal features like color output or interactive input.
    /// </summary>
    public bool RequestPty { get; set; } = false;

    /// <summary>
    /// Gets or sets the terminal type when PTY is requested.
    /// Default is Xterm. Only used when <see cref="RequestPty"/> is true.
    /// </summary>
    public TerminalType TerminalType { get; set; } = TerminalType.Xterm;

    /// <summary>
    /// Gets or sets the terminal width in characters (columns).
    /// Default is 80. Only used when <see cref="RequestPty"/> is true.
    /// </summary>
    public int TerminalWidth { get; set; } = LibSshNative.LIBSSH2_TERM_WIDTH;

    /// <summary>
    /// Gets or sets the terminal height in characters (rows).
    /// Default is 24. Only used when <see cref="RequestPty"/> is true.
    /// </summary>
    public int TerminalHeight { get; set; } = LibSshNative.LIBSSH2_TERM_HEIGHT;

    /// <summary>
    /// Gets or sets the terminal width in pixels.
    /// Default is 0. Only used when <see cref="RequestPty"/> is true.
    /// </summary>
    public int TerminalWidthPixels { get; set; } = LibSshNative.LIBSSH2_TERM_WIDTH_PX;

    /// <summary>
    /// Gets or sets the terminal height in pixels.
    /// Default is 0. Only used when <see cref="RequestPty"/> is true.
    /// </summary>
    public int TerminalHeightPixels { get; set; } = LibSshNative.LIBSSH2_TERM_HEIGHT_PX;

    /// <summary>
    /// Gets or sets the terminal modes byte array (RFC 4254 encoding).
    /// Default is null (uses empty modes). Only used when <see cref="RequestPty"/> is true.
    /// Use <see cref="TerminalModesBuilder"/> to construct custom terminal modes.
    /// </summary>
    public byte[]? TerminalModes { get; set; } = null;
}