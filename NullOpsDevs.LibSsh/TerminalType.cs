using JetBrains.Annotations;

namespace NullOpsDevs.LibSsh;

/// <summary>
/// Terminal type for PTY (pseudo-terminal) requests.
/// </summary>
[PublicAPI]
public enum TerminalType
{
    /// <summary>
    /// Standard xterm terminal emulator.
    /// </summary>
    Xterm,

    /// <summary>
    /// xterm with color support.
    /// </summary>
    XtermColor,

    /// <summary>
    /// xterm with 256 color support.
    /// </summary>
    Xterm256Color,

    /// <summary>
    /// DEC VT100 terminal.
    /// </summary>
    VT100,

    /// <summary>
    /// DEC VT220 terminal.
    /// </summary>
    VT220,

    /// <summary>
    /// Linux console terminal.
    /// </summary>
    Linux,

    /// <summary>
    /// GNU Screen terminal multiplexer.
    /// </summary>
    Screen
}

/// <summary>
/// Extension methods for <see cref="TerminalType"/>.
/// </summary>
public static class TerminalTypeExtensions
{
    /// <summary>
    /// Converts the terminal type to the string value expected by libssh2.
    /// </summary>
    public static string ToLibSsh2String(this TerminalType terminalType) => terminalType switch
    {
        TerminalType.Xterm => "xterm",
        TerminalType.XtermColor => "xterm-color",
        TerminalType.Xterm256Color => "xterm-256color",
        TerminalType.VT100 => "vt100",
        TerminalType.VT220 => "vt220",
        TerminalType.Linux => "linux",
        TerminalType.Screen => "screen",
        _ => "xterm"
    };
}
