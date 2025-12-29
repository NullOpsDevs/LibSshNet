# Command Execution

Execute commands on remote SSH servers and retrieve their output, exit codes, and error messages. NullOpsDevs.LibSsh makes it simple to run commands remotely and process their results.

## Basic Command Execution

Execute a simple command and get the result:

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Credentials;

var session = new SshSession();
session.Connect("example.com", 22);
session.Authenticate(SshCredential.FromPassword("user", "password"));

// Execute a command
var result = session.ExecuteCommand("ls -la /home/user");

Console.WriteLine("Output:");
Console.WriteLine(result.Stdout);

Console.WriteLine($"Exit code: {result.ExitCode}");
```

## Understanding Command Results

The `SshCommandResult` structure contains all information about the command execution:

```c#
var result = session.ExecuteCommand("whoami");

// Standard output (stdout)
Console.WriteLine($"Output: {result.Stdout}");

// Standard error (stderr)
if (!string.IsNullOrEmpty(result.Stderr))
{
    Console.WriteLine($"Errors: {result.Stderr}");
}

// Exit code (0 typically means success)
if (result.ExitCode == 0)
{
    Console.WriteLine("Command succeeded!");
}
else
{
    Console.WriteLine($"Command failed with exit code: {result.ExitCode}");
}

// Exit signal (if the command was terminated by a signal)
if (result.ExitSignal != null)
{
    Console.WriteLine($"Terminated by signal: {result.ExitSignal}");
}

// Overall success indicator
if (result.Successful)
{
    Console.WriteLine("Execution was successful");
}
```

## Async Command Execution

For non-blocking operations, use the async version:

```c#
var result = await session.ExecuteCommandAsync("apt update", cancellationToken: cancellationToken);
Console.WriteLine(result.Stdout);
```

## Handling Command Errors

Always check the exit code to determine if a command succeeded:

```c#
var result = session.ExecuteCommand("cat /nonexistent/file");

if (result.ExitCode != 0)
{
    Console.WriteLine("Command failed!");
    Console.WriteLine($"Exit code: {result.ExitCode}");
    Console.WriteLine($"Error output: {result.Stderr}");
}
else
{
    Console.WriteLine(result.Stdout);
}
```

## Commands Requiring PTY (Pseudo-Terminal)

Some commands need a pseudo-terminal to work correctly. Enable PTY when:
- Commands check for terminal presence
- You need ANSI color output and terminal control codes
- Programs behave differently when attached to a terminal

```c#
using NullOpsDevs.LibSsh.Core;

var options = new CommandExecutionOptions
{
    RequestPty = true,
    TerminalType = TerminalType.Xterm256Color  // Enable color support
};

var result = session.ExecuteCommand("ls --color=always", options);
Console.WriteLine(result.Stdout);  // Contains ANSI color codes
```

> **Note**: PTY enables passthrough of ANSI terminal control codes (colors, cursor positioning, etc.). However, truly interactive commands that require user input (like password prompts, `vim`, interactive `sudo`) are not supported as the library cannot handle interactive terminal I/O.

### Common PTY Use Cases

#### Getting Colored Output

```c#
var options = new CommandExecutionOptions
{
    RequestPty = true,
    TerminalType = TerminalType.Xterm256Color
};

var result = session.ExecuteCommand("ls --color=always", options);
// Output contains ANSI color codes that can be displayed in a terminal
Console.WriteLine(result.Stdout);
```

#### Running Commands That Check for TTY

```c#
// Some commands behave differently when they detect a terminal
var options = new CommandExecutionOptions { RequestPty = true };
var result = session.ExecuteCommand("./script-that-checks-tty.sh", options);

if (result.ExitCode == 0)
{
    Console.WriteLine("Script executed successfully");
}
```

## Customizing Terminal Settings

When requesting a PTY, you can customize the terminal:

```c#
using NullOpsDevs.LibSsh.Core;
using NullOpsDevs.LibSsh.Terminal;

var options = new CommandExecutionOptions
{
    RequestPty = true,
    TerminalType = TerminalType.Xterm256Color,  // Terminal emulation type
    TerminalWidth = 120,                         // Width in characters
    TerminalHeight = 40,                         // Height in characters
};

var result = session.ExecuteCommand("top -b -n 1", options);
Console.WriteLine(result.Stdout);
```

### Available Terminal Types

| Terminal Type | Description | Use Case |
|---------------|-------------|----------|
| `Xterm` | Standard xterm (default) | General purpose |
| `XtermColor` | xterm with color support | Basic color output |
| `Xterm256Color` | xterm with 256 colors | Full color support |
| `VT100` | DEC VT100 | Legacy compatibility |
| `VT220` | DEC VT220 | Legacy compatibility |
| `Linux` | Linux console | Linux-specific features |
| `Screen` | GNU Screen multiplexer | Screen sessions |

## Running Multiple Commands

### Sequential Commands (with error handling)

```c#
// Stop on first failure
var result1 = session.ExecuteCommand("mkdir -p /tmp/myapp");
if (result1.ExitCode != 0)
{
    Console.WriteLine("Failed to create directory");
    return;
}

var result2 = session.ExecuteCommand("cd /tmp/myapp && touch file.txt");
if (result2.ExitCode != 0)
{
    Console.WriteLine("Failed to create file");
    return;
}

Console.WriteLine("All commands executed successfully");
```

### Using Shell Operators

```c#
// Run multiple commands in a single execution (with && for conditional execution)
var result = session.ExecuteCommand("cd /tmp && mkdir test && cd test && pwd");
Console.WriteLine(result.Stdout);  // Should print: /tmp/test

// Run commands regardless of success (with ;)
result = session.ExecuteCommand("command1; command2; command3");

// Run command in background (with &)
result = session.ExecuteCommand("long-running-task &");
```

## Long-Running Commands

For commands that take a long time, consider using cancellation tokens:

```c#
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

try
{
    var result = await session.ExecuteCommandAsync(
        "./long-script.sh",
        cancellationToken: cts.Token
    );

    Console.WriteLine("Script completed!");
    Console.WriteLine(result.Stdout);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Command timed out after 5 minutes");
}
```

## Streaming Command Execution

For commands that produce large amounts of output or when you need to process output as it arrives, use streaming execution. Unlike standard command execution which buffers all output in memory, streaming provides direct access to stdout and stderr streams.

### Basic Streaming

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Core;
using NullOpsDevs.LibSsh.Credentials;

var session = new SshSession();
session.Connect("example.com", 22);
session.Authenticate(SshCredential.FromPassword("user", "password"));

// Execute command with streaming output
using var stream = session.ExecuteCommandStreaming("cat /var/log/syslog");

// Read stdout as a stream
using var reader = new StreamReader(stream.Stdout);
while (!reader.EndOfStream)
{
    var line = reader.ReadLine();
    Console.WriteLine(line);
}

// Get exit code after consuming the streams
var result = stream.WaitForExit();
Console.WriteLine($"Exit code: {result.ExitCode}");
```

### Stream Output Directly to a File

Streaming is ideal for downloading large command output without buffering in memory:

```c#
using var stream = session.ExecuteCommandStreaming("mysqldump database_name");

// Stream directly to a file
using var file = File.Create("backup.sql");
stream.Stdout.CopyTo(file);

var result = stream.WaitForExit();
if (result.ExitCode != 0)
{
    Console.WriteLine("Backup failed!");
}
```

### Reading Both Stdout and Stderr

Access both output streams separately:

```c#
using var stream = session.ExecuteCommandStreaming("./build.sh");

// Read both streams
using var stdoutReader = new StreamReader(stream.Stdout);
using var stderrReader = new StreamReader(stream.Stderr);

var stdout = stdoutReader.ReadToEnd();
var stderr = stderrReader.ReadToEnd();

var result = stream.WaitForExit();

Console.WriteLine("Output:");
Console.WriteLine(stdout);

if (!string.IsNullOrEmpty(stderr))
{
    Console.WriteLine("Errors:");
    Console.WriteLine(stderr);
}
```

### Async Streaming

For non-blocking streaming operations:

```c#
using var stream = await session.ExecuteCommandStreamingAsync("tail -f /var/log/app.log",
    cancellationToken: cancellationToken);

using var reader = new StreamReader(stream.Stdout);

// Process lines as they arrive
while (!reader.EndOfStream)
{
    var line = await reader.ReadLineAsync();
    ProcessLogLine(line);
}
```

### Processing Incremental Output

For commands that produce output over time (like progress indicators), read incrementally:

```c#
using var stream = session.ExecuteCommandStreaming("./slow-process.sh");

var buffer = new byte[4096];
int bytesRead;

while ((bytesRead = stream.Stdout.Read(buffer, 0, buffer.Length)) > 0)
{
    var text = Encoding.UTF8.GetString(buffer, 0, bytesRead);
    Console.Write(text);  // Output as it arrives
}

var result = stream.WaitForExit();
```

### Streaming with PTY

Combine streaming with PTY for commands that require terminal features:

```c#
var options = new CommandExecutionOptions
{
    RequestPty = true,
    TerminalType = TerminalType.Xterm256Color
};

using var stream = session.ExecuteCommandStreaming("htop -n 1", options);

using var reader = new StreamReader(stream.Stdout);
var output = reader.ReadToEnd();
Console.WriteLine(output);  // Contains ANSI color codes

stream.WaitForExit();
```

### Important Usage Notes

1. **Consume streams before getting exit code**: Always read from `Stdout` and `Stderr` before calling `WaitForExit()`. The streams must be consumed to allow the command to complete.

2. **Dispose the stream**: The `SshCommandStream` owns the underlying SSH channel. Always dispose it when done:
   ```c#
   using var stream = session.ExecuteCommandStreaming("command");
   // ... use streams ...
   ```

3. **Streams are read-only**: The `Stdout` and `Stderr` properties return read-only streams. You cannot write to them.

4. **WaitForExit can only be called once**: After calling `WaitForExit()`, the streams are no longer readable.

### When to Use Streaming vs. Standard Execution

| Scenario | Use |
|----------|-----|
| Small command output (< 1MB) | `ExecuteCommand()` |
| Large file downloads via command | `ExecuteCommandStreaming()` |
| Real-time log monitoring | `ExecuteCommandStreaming()` |
| Simple scripts and automation | `ExecuteCommand()` |
| Processing output incrementally | `ExecuteCommandStreaming()` |
| Commands with predictable output | `ExecuteCommand()` |
| Memory-constrained environments | `ExecuteCommandStreaming()` |

## Advanced: Channel Settings

For fine-tuning performance, adjust channel settings:

```c#
var options = new CommandExecutionOptions
{
    WindowSize = 4 * 1024 * 1024,  // 4MB window (default: 2MB)
    PacketSize = 64 * 1024,         // 64KB packets (default: 32KB)
};

var result = session.ExecuteCommand("cat large-file.txt", options);
```

> **Note:** Only adjust these settings if you experience performance issues with large data transfers. The defaults work well for most use cases.

## Complete Example

Here's a comprehensive example showing various command execution scenarios:

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Core;
using NullOpsDevs.LibSsh.Credentials;
using NullOpsDevs.LibSsh.Exceptions;

var session = new SshSession();

try
{
    // Connect and authenticate
    session.Connect("example.com", 22);
    session.Authenticate(SshCredential.FromPublicKeyFile(
        "username",
        "~/.ssh/id_ed25519.pub",
        "~/.ssh/id_ed25519"
    ));

    // 1. Simple command
    Console.WriteLine("=== System Information ===");
    var result = session.ExecuteCommand("uname -a");
    Console.WriteLine(result.Stdout);

    // 2. Command with error checking
    Console.WriteLine("\n=== Disk Usage ===");
    result = session.ExecuteCommand("df -h /");
    if (result.ExitCode == 0)
    {
        Console.WriteLine(result.Stdout);
    }
    else
    {
        Console.WriteLine($"Error: {result.Stderr}");
    }

    // 3. Command requiring PTY
    Console.WriteLine("\n=== Running with sudo ===");
    var ptyOptions = new CommandExecutionOptions { RequestPty = true };
    result = session.ExecuteCommand("sudo ls /root", ptyOptions);

    if (result.ExitCode == 0)
    {
        Console.WriteLine(result.Stdout);
    }

    // 4. Multiple commands
    Console.WriteLine("\n=== Creating test directory ===");
    result = session.ExecuteCommand("mkdir -p /tmp/test && cd /tmp/test && pwd");
    Console.WriteLine($"Created: {result.Stdout.Trim()}");

    // 5. Async command
    Console.WriteLine("\n=== Async operation ===");
    result = await session.ExecuteCommandAsync("ps aux | head -n 5");
    Console.WriteLine(result.Stdout);
}
catch (SshException ex)
{
    Console.WriteLine($"SSH error: {ex.Message}");
}
finally
{
    session.Dispose();
}
```

## Best Practices

1. **Always check exit codes**:
   - Don't rely solely on `Successful` - check `ExitCode`
   - Zero typically means success, non-zero means failure

2. **Use PTY when needed**:
   - Enable for `sudo`, interactive commands, or color output
   - Disable for scripting and automation (default)

3. **Handle both stdout and stderr**:
   - Some programs write to stderr even on success
   - Check both streams for complete information

4. **Quote arguments properly**:
   - Use shell quoting for arguments with spaces
   - Example: `"ls '/path with spaces/'"`

5. **Avoid command injection**:
   - Don't concatenate user input directly into commands
   - Validate and sanitize all user-provided data

6. **Set appropriate timeouts**:
   - Use cancellation tokens for long-running commands
   - Consider session timeouts with `SetSessionTimeout()`

## Common Issues

### Issue: Command works locally but not via SSH
**Solution**: The command might require PTY. Enable `RequestPty = true`.

### Issue: Interactive commands hang or fail
**Solution**: Interactive commands (password prompts, `vim`, interactive `sudo`) are not supported. The library cannot handle interactive terminal I/O. Use non-interactive alternatives:
- For sudo: Configure passwordless sudo
- For passwords: Pass via command arguments or environment variables
- For interactive tools: Use non-interactive flags (e.g., `vim -e` for ex mode)

### Issue: Color codes appear as garbage
**Solution**: Either enable PTY with appropriate terminal type, or disable colors in the command (e.g., `ls --color=never`).

### Issue: Working directory not persisted
**Solution**: Each `ExecuteCommand()` starts in the user's home directory. Use `cd /path && command` or absolute paths.

## See Also

- `SshSession.ExecuteCommand()` - Execute commands synchronously
- `SshSession.ExecuteCommandAsync()` - Execute commands asynchronously
- `SshSession.ExecuteCommandStreaming()` - Execute commands with streaming output
- `SshSession.ExecuteCommandStreamingAsync()` - Execute commands with streaming output asynchronously
- `SshCommandStream` - Streaming command result with stdout/stderr streams
- `CommandExecutionOptions` - Configure command execution
- `SshCommandResult` - Command execution results
- [Authentication](authentication.md) - Authenticate before executing commands
- [Advanced Terminal Control](advanced-terminal-control.md) - Configure terminal modes for PTY
- [Session Timeouts](session-timeouts.md) - Set timeouts for long-running commands
- [Session Lifecycle](session-lifecycle.md) - Understanding session states
- [Error Handling](error-handling.md) - Handle command execution errors
