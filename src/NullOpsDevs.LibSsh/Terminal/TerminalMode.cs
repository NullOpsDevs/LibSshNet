using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace NullOpsDevs.LibSsh.Terminal;

/// <summary>
/// Terminal mode opcodes as defined in RFC 4254, Section 8.
/// Used for encoding terminal behavior settings in PTY requests.
/// </summary>
[PublicAPI]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum TerminalMode : byte
{
    /// <summary>
    /// End of terminal modes. Must be last in the encoded stream.
    /// </summary>
    TTY_OP_END = 0,

    // Character codes (opcodes 1-18)

    /// <summary>
    /// Interrupt character (typically Ctrl-C, sends SIGINT).
    /// </summary>
    VINTR = 1,

    /// <summary>
    /// Quit character (typically Ctrl-\, sends SIGQUIT).
    /// </summary>
    VQUIT = 2,

    /// <summary>
    /// Erase the character to the left of the cursor.
    /// </summary>
    VERASE = 3,

    /// <summary>
    /// Kill the current input line.
    /// </summary>
    VKILL = 4,

    /// <summary>
    /// End-of-file character (typically Ctrl-D).
    /// </summary>
    VEOF = 5,

    /// <summary>
    /// End-of-line character.
    /// </summary>
    VEOL = 6,

    /// <summary>
    /// Additional end-of-line character.
    /// </summary>
    VEOL2 = 7,

    /// <summary>
    /// Continue paused output (typically Ctrl-Q).
    /// </summary>
    VSTART = 8,

    /// <summary>
    /// Pause output (typically Ctrl-S).
    /// </summary>
    VSTOP = 9,

    /// <summary>
    /// Suspend character (typically Ctrl-Z, sends SIGTSTP).
    /// </summary>
    VSUSP = 10,

    /// <summary>
    /// Delayed suspend character.
    /// </summary>
    VDSUSP = 11,

    /// <summary>
    /// Reprint the current input line.
    /// </summary>
    VREPRINT = 12,

    /// <summary>
    /// Erase the previous word.
    /// </summary>
    VWERASE = 13,

    /// <summary>
    /// Enter the next character literally.
    /// </summary>
    VLNEXT = 14,

    /// <summary>
    /// Flush output.
    /// </summary>
    VFLUSH = 15,

    /// <summary>
    /// Switch to a different shell layer.
    /// </summary>
    VSWTCH = 16,

    /// <summary>
    /// Status request character.
    /// </summary>
    VSTATUS = 17,

    /// <summary>
    /// Toggle discarding of output.
    /// </summary>
    VDISCARD = 18,

    // Input flags (opcodes 30-41)

    /// <summary>
    /// Ignore parity errors.
    /// </summary>
    IGNPAR = 30,

    /// <summary>
    /// Mark parity and framing errors.
    /// </summary>
    PARMRK = 31,

    /// <summary>
    /// Enable input parity checking.
    /// </summary>
    INPCK = 32,

    /// <summary>
    /// Strip 8th bit off characters.
    /// </summary>
    ISTRIP = 33,

    /// <summary>
    /// Map NL to CR on input.
    /// </summary>
    INLCR = 34,

    /// <summary>
    /// Ignore CR.
    /// </summary>
    IGNCR = 35,

    /// <summary>
    /// Map CR to NL on input.
    /// </summary>
    ICRNL = 36,

    /// <summary>
    /// Translate uppercase to lowercase on input.
    /// </summary>
    IUCLC = 37,

    /// <summary>
    /// Enable XON/XOFF flow control on output.
    /// </summary>
    IXON = 38,

    /// <summary>
    /// Any character will restart output.
    /// </summary>
    IXANY = 39,

    /// <summary>
    /// Enable XON/XOFF flow control on input.
    /// </summary>
    IXOFF = 40,

    /// <summary>
    /// Ring bell when input queue is full.
    /// </summary>
    IMAXBEL = 41,

    // Local flags (opcodes 50-62)

    /// <summary>
    /// Enable signals (INTR, QUIT, SUSP).
    /// </summary>
    ISIG = 50,

    /// <summary>
    /// Canonical input mode (line-buffered).
    /// </summary>
    ICANON = 51,

    /// <summary>
    /// Enable extended case processing.
    /// </summary>
    XCASE = 52,

    /// <summary>
    /// Echo input characters.
    /// </summary>
    ECHO = 53,

    /// <summary>
    /// Visually erase characters.
    /// </summary>
    ECHOE = 54,

    /// <summary>
    /// Echo KILL character.
    /// </summary>
    ECHOK = 55,

    /// <summary>
    /// Echo NL.
    /// </summary>
    ECHONL = 56,

    /// <summary>
    /// Disable flushing after interrupt or quit.
    /// </summary>
    NOFLSH = 57,

    /// <summary>
    /// Send SIGTTOU for background output.
    /// </summary>
    TOSTOP = 58,

    /// <summary>
    /// Enable implementation-defined input processing.
    /// </summary>
    IEXTEN = 59,

    /// <summary>
    /// Echo control characters as ^X.
    /// </summary>
    ECHOCTL = 60,

    /// <summary>
    /// Visual erase for line kill.
    /// </summary>
    ECHOKE = 61,

    /// <summary>
    /// Retype pending input.
    /// </summary>
    PENDIN = 62,

    // Output flags (opcodes 70-75)

    /// <summary>
    /// Enable output processing.
    /// </summary>
    OPOST = 70,

    /// <summary>
    /// Map lowercase to uppercase on output.
    /// </summary>
    OLCUC = 71,

    /// <summary>
    /// Map NL to CR-NL on output.
    /// </summary>
    ONLCR = 72,

    /// <summary>
    /// Map CR to NL on output.
    /// </summary>
    OCRNL = 73,

    /// <summary>
    /// Do not output CR at column 0.
    /// </summary>
    ONOCR = 74,

    /// <summary>
    /// NL performs CR function.
    /// </summary>
    ONLRET = 75,

    // Control flags (opcodes 90-93)

    /// <summary>
    /// 7-bit mode.
    /// </summary>
    CS7 = 90,

    /// <summary>
    /// 8-bit mode.
    /// </summary>
    CS8 = 91,

    /// <summary>
    /// Enable parity generation and detection.
    /// </summary>
    PARENB = 92,

    /// <summary>
    /// Use odd parity (else even).
    /// </summary>
    PARODD = 93,

    // Speed settings (opcodes 128-129)

    /// <summary>
    /// Input baud rate (uint32 value).
    /// </summary>
    TTY_OP_ISPEED = 128,

    /// <summary>
    /// Output baud rate (uint32 value).
    /// </summary>
    TTY_OP_OSPEED = 129
}
