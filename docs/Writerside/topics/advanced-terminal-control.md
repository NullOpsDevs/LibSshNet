# Advanced Terminal Control

When requesting a pseudo-terminal (PTY), you can configure detailed terminal behavior using terminal modes. This allows fine-grained control over input processing, output formatting, signal handling, and echo behavior.

## When You Need Terminal Modes

Terminal modes control how the remote terminal processes input and output. You can customize terminal modes for:
- Controlling how output is processed (ANSI codes, line endings)
- Raw vs. canonical (line-buffered) mode
- Custom control character mappings
- Flow control configuration

> **Important**: This library does not support truly interactive terminal sessions. Terminal modes are useful for controlling output formatting and terminal behavior, but you cannot interactively type input or respond to prompts.

## Basic Terminal Mode Configuration

Use `TerminalModesBuilder` to create custom terminal configurations:

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Core;
using NullOpsDevs.LibSsh.Terminal;

var session = new SshSession();
session.Connect("example.com", 22);
session.Authenticate(credential);

// Build custom terminal modes
var terminalModes = new TerminalModesBuilder()
    .SetFlag(TerminalMode.ONLCR, true)     // Map NL to CR-NL on output
    .SetFlag(TerminalMode.OPOST, true)     // Enable output processing
    .Build();

var options = new CommandExecutionOptions
{
    RequestPty = true,
    TerminalModes = terminalModes
};

var result = session.ExecuteCommand("ls --color=always", options);
```

## Terminal Mode Categories

Terminal modes are organized into several categories:

### Control Characters (Input Processing)

Define special character mappings for terminal control:

| Mode | Default | Description | Common Use |
|------|---------|-------------|------------|
| `VINTR` | ^C (3) | Interrupt signal (SIGINT) | Ctrl-C to terminate |
| `VQUIT` | ^\ (28) | Quit signal (SIGQUIT) | Ctrl-\ to quit with core dump |
| `VERASE` | ^? (127) | Erase previous character | Backspace behavior |
| `VKILL` | ^U (21) | Kill entire line | Ctrl-U to clear line |
| `VEOF` | ^D (4) | End-of-file | Ctrl-D to signal EOF |
| `VEOL` | - | End of line | Alternative to Enter |
| `VSTART` | ^Q (17) | Resume output | XON flow control |
| `VSTOP` | ^S (19) | Pause output | XOFF flow control |
| `VSUSP` | ^Z (26) | Suspend signal (SIGTSTP) | Ctrl-Z to background |

### Input Flags

Control how input is processed:

| Mode | Description | When to Use |
|------|-------------|-------------|
| `IGNPAR` | Ignore parity errors | Serial communication |
| `INPCK` | Enable parity checking | Serial communication |
| `ISTRIP` | Strip 8th bit | 7-bit systems |
| `INLCR` | Map NL to CR on input | Line ending conversion |
| `IGNCR` | Ignore carriage return | Unix line endings only |
| `ICRNL` | Map CR to NL on input | Windows/Unix compatibility |
| `IXON` | Enable XON/XOFF flow control | Flow control |
| `IXOFF` | Enable input flow control | Prevent buffer overflow |

### Local Flags (Terminal Behavior)

Control major terminal features:

| Mode | Description | When to Enable | When to Disable |
|------|-------------|----------------|-----------------|
| `ISIG` | Enable signal generation | Interactive shells | Raw input mode |
| `ICANON` | Canonical (line-buffered) mode | Line editing | Character-by-character input |
| `ECHO` | Echo input characters | Interactive input | Password entry, automation |
| `ECHOE` | Visual character erase | User-friendly editing | Raw mode |
| `ECHOK` | Echo kill character | Show line clear | Silent operations |
| `ECHONL` | Echo newline even if ECHO off | Debugging | Normal operation |
| `NOFLSH` | Don't flush after interrupt | Keep data | Standard behavior |
| `IEXTEN` | Extended input processing | Full features | Simple processing |
| `ECHOCTL` | Echo control chars as ^X | Visible controls | Clean output |

### Output Flags

Control output formatting:

| Mode | Description | When to Use |
|------|-------------|-------------|
| `OPOST` | Enable output processing | Normal terminal | Raw output |
| `ONLCR` | Map NL to CR-NL | Windows compatibility | Unix systems |
| `OCRNL` | Map CR to NL | Unusual case | Standard operation |
| `ONOCR` | No CR at column 0 | Special formatting | Normal output |

## Common Terminal Mode Recipes

### Recipe 1: Disable All Flow Control

Prevent XON/XOFF interfering with binary data:

```c#
var modes = new TerminalModesBuilder()
    .SetFlag(TerminalMode.IXON, false)      // Disable output flow control
    .SetFlag(TerminalMode.IXOFF, false)     // Disable input flow control
    .SetFlag(TerminalMode.IXANY, false)     // Don't restart with any char
    .Build();

var options = new CommandExecutionOptions
{
    RequestPty = true,
    TerminalModes = modes
};

// Safe for binary output
var result = session.ExecuteCommand("cat binary-file", options);
```

### Recipe 2: Clean Output Processing

Configure proper line ending handling:

```c#
var modes = new TerminalModesBuilder()
    .SetFlag(TerminalMode.OPOST, true)      // Enable output processing
    .SetFlag(TerminalMode.ONLCR, true)      // NL to CR-NL on output
    .SetFlag(TerminalMode.ICRNL, true)      // CR to NL on input
    .Build();

var options = new CommandExecutionOptions
{
    RequestPty = true,
    TerminalModes = modes
};

var result = session.ExecuteCommand("./script-with-output.sh", options);
```

### Recipe 3: Raw Output Mode

Get output exactly as produced without terminal processing:

```c#
var modes = new TerminalModesBuilder()
    .SetFlag(TerminalMode.OPOST, false)     // Disable output processing
    .SetFlag(TerminalMode.ONLCR, false)     // No line ending conversion
    .Build();

var options = new CommandExecutionOptions
{
    RequestPty = true,
    TerminalModes = modes
};

var result = session.ExecuteCommand("./binary-output-program", options);
```

## TerminalModesBuilder API Reference

### SetCharacter()

Set control character values (0-255):

```c#
builder.SetCharacter(TerminalMode.VINTR, 0x03);  // Ctrl-C = ASCII 3
builder.SetCharacter(TerminalMode.VEOF, 0x04);   // Ctrl-D = ASCII 4
```

Common control character codes:
- `^C` (Ctrl-C) = 3
- `^D` (Ctrl-D) = 4
- `^H` (Backspace) = 8
- `^S` (Ctrl-S) = 19
- `^Q` (Ctrl-Q) = 17
- `^Z` (Ctrl-Z) = 26
- `DEL` = 127

### SetFlag()

Enable or disable boolean mode flags:

```c#
builder.SetFlag(TerminalMode.ECHO, true);    // Enable
builder.SetFlag(TerminalMode.ICANON, false); // Disable
```

### SetMode()

Set mode with custom uint32 value:

```c#
builder.SetMode(TerminalMode.TTY_OP_ISPEED, 38400); // Baud rate
```

### SetSpeed()

Set both input and output baud rate:

```c#
builder.SetSpeed(38400); // 38400 baud
```

Common baud rates: 9600, 19200, 38400, 57600, 115200

## Advanced Example: Custom Terminal Configuration

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Core;
using NullOpsDevs.LibSsh.Credentials;
using NullOpsDevs.LibSsh.Terminal;

public class CustomTerminalSession
{
    public void RunCommandWithCustomTerminal()
    {
        var session = new SshSession();
        session.Connect("example.com", 22);
        session.Authenticate(SshCredential.FromPassword("user", "password"));

        // Configure terminal for clean output processing
        var modes = new TerminalModesBuilder()
            // Input flags
            .SetFlag(TerminalMode.ICRNL, true)        // CR to NL
            .SetFlag(TerminalMode.IXON, false)        // No XON/XOFF flow control
            .SetFlag(TerminalMode.IXOFF, false)       // No input flow control

            // Output flags
            .SetFlag(TerminalMode.OPOST, true)        // Enable output processing
            .SetFlag(TerminalMode.ONLCR, true)        // NL to CR-NL for proper line endings

            // Control flags
            .SetFlag(TerminalMode.CS8, true)          // 8-bit characters

            .Build();

        var options = new CommandExecutionOptions
        {
            RequestPty = true,
            TerminalType = TerminalType.Xterm256Color,
            TerminalWidth = 120,
            TerminalHeight = 40,
            TerminalModes = modes
        };

        Console.WriteLine("Running command with custom terminal settings...");
        var result = session.ExecuteCommand("ls --color=always -lh", options);

        Console.WriteLine("Command output:");
        Console.WriteLine(result.Stdout);

        if (result.ExitCode != 0)
        {
            Console.WriteLine("Errors:");
            Console.WriteLine(result.Stderr);
        }

        session.Dispose();
    }
}
```

## Debugging Terminal Modes

To see what terminal modes are configured on the remote system:

```c#
// Check current terminal settings
var result = session.ExecuteCommand("stty -a");
Console.WriteLine(result.Stdout);
```

This shows all terminal settings on the remote server, useful for understanding defaults and troubleshooting issues.

## Best Practices

1. **Start with defaults**:
   ```c#
   // Empty modes = server defaults
   var options = new CommandExecutionOptions { RequestPty = true };
   ```

2. **Only customize what you need**:
   ```c#
   // Minimal changes for specific need
   var modes = new TerminalModesBuilder()
       .SetFlag(TerminalMode.ECHO, false)  // Only disable echo
       .Build();
   ```

3. **Test terminal behavior**:
   - Different servers may interpret modes differently
   - Test with your target environment

4. **Use appropriate recipes**:
   - Don't disable signals in interactive shells
   - Don't enable echo for password input

5. **Consider terminal type**:
   ```c#
   options.TerminalType = TerminalType.Xterm256Color;  // Match capabilities
   ```

6. **Document custom configurations**:
   ```c#
   // Comment why you're using specific modes
   .SetFlag(TerminalMode.ECHO, false)  // Required for silent password prompt
   ```

## Common Issues

### Issue: Line endings are wrong
**Solution**: Configure line ending translation:
```c#
builder.SetFlag(TerminalMode.ICRNL, true);  // Input: CR→NL
builder.SetFlag(TerminalMode.ONLCR, true);  // Output: NL→CR-NL
```

### Issue: Binary data is corrupted
**Solution**: Disable flow control and processing:
```c#
builder.SetFlag(TerminalMode.IXON, false);   // No XON/XOFF
builder.SetFlag(TerminalMode.IXOFF, false);  // No flow control
builder.SetFlag(TerminalMode.OPOST, false);  // No output processing
```

### Issue: Need interactive input
**Solution**: This library does not support interactive terminal I/O. You cannot type input or respond to prompts interactively. Use non-interactive alternatives or pass input via command arguments.

## Terminal Mode Reference

See the complete list of all 50+ terminal modes in `TerminalMode.cs`:
- **Control Characters** (opcodes 1-18): VINTR, VQUIT, VERASE, VKILL, VEOF, VEOL, VSTART, VSTOP, VSUSP, etc.
- **Input Flags** (opcodes 30-41): IGNPAR, INPCK, ISTRIP, INLCR, IGNCR, ICRNL, IXON, IXOFF, etc.
- **Local Flags** (opcodes 50-62): ISIG, ICANON, ECHO, ECHOE, ECHOK, NOFLSH, IEXTEN, ECHOCTL, etc.
- **Output Flags** (opcodes 70-75): OPOST, ONLCR, OCRNL, ONOCR, ONLRET, etc.
- **Control Flags** (opcodes 90-93): CS7, CS8, PARENB, PARODD
- **Speed Settings** (opcodes 128-129): TTY_OP_ISPEED, TTY_OP_OSPEED

## See Also

- `TerminalModesBuilder` (TerminalModesBuilder.cs:10) - Build terminal mode configurations
- `TerminalMode` enum (TerminalMode.cs:12) - All available terminal modes
- `CommandExecutionOptions` (CommandExecutionOptions.cs:11) - Configure PTY requests
- [Command Execution](command-execution.md) - Using PTY for command execution
- [Session Lifecycle](session-lifecycle.md) - Understanding session states
