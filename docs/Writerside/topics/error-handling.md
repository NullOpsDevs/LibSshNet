# Error Handling

NullOpsDevs.LibSsh reports errors by throwing `SshException` when operations fail. Understanding how errors are thrown and what they mean is essential for building robust SSH applications.

## How Errors Are Thrown

The library throws `SshException` when SSH operations fail. This exception contains:
- **Message**: Human-readable error description
- **Error**: An `SshError` enum value indicating the specific error type
- **InnerException**: Optional underlying exception that caused the error

```c#
try
{
    session.Connect("example.com", 22);
}
catch (SshException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Error Code: {ex.Error}");

    if (ex.InnerException != null)
    {
        Console.WriteLine($"Caused by: {ex.InnerException.Message}");
    }
}
```

## Basic Error Handling

Always wrap SSH operations in try-catch blocks:

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Credentials;
using NullOpsDevs.LibSsh.Exceptions;

var session = new SshSession();

try
{
    session.Connect("example.com", 22);
    session.Authenticate(SshCredential.FromPassword("user", "password"));

    var result = session.ExecuteCommand("ls -la");
    Console.WriteLine(result.Stdout);
}
catch (SshException ex)
{
    Console.WriteLine($"SSH operation failed: {ex.Message}");
}
finally
{
    session.Dispose();
}
```

## Error Types Reference

### Connection Errors

These errors occur during connection establishment:

| Error Code | Description | Common Causes |
|------------|-------------|---------------|
| `SocketNone` | No socket established | Called operation before connecting |
| `BannerRecv` | Failed to receive SSH banner | Network issue, wrong port, non-SSH service |
| `BannerSend` | Failed to send SSH banner | Network issue, connection interrupted |
| `BannerNone` | No SSH banner exchanged | Protocol mismatch, connection failed |
| `SocketDisconnect` | Socket disconnected | Network interruption, server closed connection |
| `SocketSend` | Failed to send data | Network issue, connection lost |
| `SocketRecv` | Failed to receive data | Network issue, connection lost |
| `BadSocket` | Invalid socket descriptor | Socket closed or corrupted |
| `Timeout` | Operation timed out | Network latency, server not responding |
| `SocketTimeout` | Socket operation timed out | Network issue, timeout settings too low |

### Key Exchange Errors

These errors occur during the SSH handshake and key exchange:

| Error Code | Description | Common Causes |
|------------|-------------|---------------|
| `KexFailure` | Key exchange negotiation failed | No common algorithms supported |
| `KeyExchangeFailure` | Key exchange process failed | Incompatible SSH versions, crypto failure |
| `HostkeyInit` | Host key initialization failed | Server key corrupted, unsupported key type |
| `HostkeySign` | Host key signing failed | Server key error, crypto failure |
| `AlgoUnsupported` | Algorithm not supported | Client/server algorithm mismatch |

### Authentication Errors

These errors occur during user authentication:

| Error Code | Description | Common Causes |
|------------|-------------|---------------|
| `AuthenticationFailed` | Authentication failed | Wrong credentials, method not allowed |
| `PublickeyUnverified` | Public key not verified | Key not authorized on server |
| `KeyfileAuthFailed` | Key file authentication failed | Invalid key file, wrong passphrase |
| `PasswordExpired` | Password has expired | Account password needs reset |
| `MethodNone` | No authentication method available | Server doesn't support requested method |
| `AgentProtocol` | SSH agent protocol error | Agent not running, communication failure |
| `MissingUserauthBanner` | Expected authentication banner missing | Protocol issue, unusual server config |

### Channel Errors

These errors occur during SSH channel operations (command execution, file transfers):

| Error Code | Description | Common Causes |
|------------|-------------|---------------|
| `ChannelFailure` | General channel failure | Channel operation failed |
| `ChannelRequestDenied` | Channel request denied by server | Permission denied, invalid request |
| `ChannelUnknown` | Unknown or invalid channel | Channel closed, internal error |
| `ChannelOutoforder` | Channel packets out of order | Protocol error, corrupted data |
| `ChannelClosed` | Channel has been closed | Channel already closed |
| `ChannelEofSent` | EOF already sent on channel | Cannot send more data |
| `ChannelWindowExceeded` | Channel window size exceeded | Flow control issue |
| `ChannelPacketExceeded` | Channel packet size exceeded | Packet too large |
| `ChannelWindowFull` | Channel window is full | Cannot send until window adjusted |

### File Transfer Errors

These errors occur during SCP or SFTP operations:

| Error Code | Description | Common Causes |
|------------|-------------|---------------|
| `ScpProtocol` | SCP protocol error | File doesn't exist, permission denied |
| `SftpProtocol` | SFTP protocol error | SFTP subsystem unavailable |
| `File` | File operation failed | File not found, permission denied |

### Cryptographic Errors

These errors occur during encryption/decryption operations:

| Error Code | Description | Common Causes |
|------------|-------------|---------------|
| `InvalidMac` | MAC verification failed | Data corrupted or tampered with |
| `Decrypt` | Decryption failed | Corrupted data, wrong key |
| `Encrypt` | Encryption failed | Crypto library error |
| `MacFailure` | MAC operation failed | Crypto operation error |
| `HashInit` | Hash initialization failed | Crypto library issue |
| `HashCalc` | Hash calculation failed | Crypto operation error |

### Resource Errors

These errors occur due to resource limitations:

| Error Code | Description | Common Causes |
|------------|-------------|---------------|
| `Alloc` | Memory allocation failed | Out of memory |
| `BufferTooSmall` | Buffer too small | Internal buffer sizing issue |
| `OutOfBoundary` | Out-of-boundary access | Internal error, corrupted data |
| `Randgen` | Random number generation failed | Crypto library issue |

### Compression Errors

These errors occur during data compression:

| Error Code | Description | Common Causes |
|------------|-------------|---------------|
| `Zlib` | Compression/decompression error | zlib error, corrupted data |
| `Compress` | Compression failed | Compression algorithm error |

### Protocol Errors

These errors indicate SSH protocol violations:

| Error Code | Description | Common Causes |
|------------|-------------|---------------|
| `Proto` | SSH protocol error | Protocol violation, incompatible versions |
| `RequestDenied` | Request denied by server | Server rejected request |
| `MethodNotSupported` | Method not supported | Server doesn't support requested feature |
| `PublickeyProtocol` | Public key protocol error | Public key subsystem error |

### Known Hosts Errors

These errors occur during known hosts operations:

| Error Code | Description | Common Causes |
|------------|-------------|---------------|
| `KnownHosts` | Known hosts file operation failed | File access issue, parse error |

### API Usage Errors

These errors indicate incorrect use of the library:

| Error Code | Description | Common Causes |
|------------|-------------|---------------|
| `BadUse` | Incorrect API usage | Wrong parameter, invalid state |
| `Inval` | Invalid argument | Invalid parameter value |
| `InvalidPollType` | Invalid polling type | Non-blocking mode error |
| `DevWrongUse` | Developer error (custom) | Session in wrong state, invalid operation |
| `FailedToInitializeSession` | Session init failed (custom) | Library initialization error |
| `InnerException` | Inner exception occurred (custom) | See InnerException property |

### Special Errors

| Error Code | Description | Notes |
|------------|-------------|-------|
| `None` | No error occurred | Operation successful |
| `Eagain` | Operation would block | Non-blocking mode only (library uses blocking mode) |

## Handling Specific Errors

### Connection Failures

```c#
try
{
    session.Connect("example.com", 22);
}
catch (SshException ex) when (ex.Error == SshError.BannerRecv)
{
    Console.WriteLine("Failed to connect - check hostname and port");
}
catch (SshException ex) when (ex.Error == SshError.Timeout)
{
    Console.WriteLine("Connection timed out - check network connectivity");
}
catch (SshException ex) when (ex.Error == SshError.SocketDisconnect)
{
    Console.WriteLine("Connection was interrupted");
}
```

### Authentication Failures

```c#
try
{
    session.Authenticate(SshCredential.FromPassword("user", "wrong-password"));
}
catch (SshException ex) when (ex.Error == SshError.AuthenticationFailed)
{
    Console.WriteLine("Authentication failed - check your credentials");
}
catch (SshException ex) when (ex.Error == SshError.PublickeyUnverified)
{
    Console.WriteLine("Public key not authorized on server");
}
catch (SshException ex) when (ex.Error == SshError.PasswordExpired)
{
    Console.WriteLine("Your password has expired and must be changed");
}
```

### Command Execution Errors

```c#
try
{
    var result = session.ExecuteCommand("some-command");

    // Note: Command execution usually succeeds even if the command fails
    // Check result.ExitCode instead of catching exceptions
    if (result.ExitCode != 0)
    {
        Console.WriteLine($"Command failed with exit code {result.ExitCode}");
    }
}
catch (SshException ex) when (ex.Error == SshError.ChannelFailure)
{
    Console.WriteLine("Failed to open SSH channel");
}
catch (SshException ex) when (ex.Error == SshError.ChannelRequestDenied)
{
    Console.WriteLine("Server denied the channel request");
}
```

### File Transfer Errors {id="handling-file-transfer-errors"}

```c#
try
{
    using var stream = File.OpenRead("local-file.txt");
    session.WriteFile("/remote/path/file.txt", stream);
}
catch (SshException ex) when (ex.Error == SshError.ScpProtocol)
{
    Console.WriteLine("SCP transfer failed - check file permissions");
}
catch (FileNotFoundException ex)
{
    Console.WriteLine("Local file not found");
}
catch (IOException ex)
{
    Console.WriteLine($"I/O error: {ex.Message}");
}
```

## Comprehensive Error Handling Example

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Core;
using NullOpsDevs.LibSsh.Credentials;
using NullOpsDevs.LibSsh.Exceptions;

public class RobustSshConnection
{
    public bool ConnectAndExecute(string host, int port, string username, string password)
    {
        var session = new SshSession();

        try
        {
            // Connection phase
            Console.WriteLine("Connecting...");
            session.Connect(host, port);
            Console.WriteLine("Connected successfully");

            // Authentication phase
            Console.WriteLine("Authenticating...");
            var credential = SshCredential.FromPassword(username, password);
            bool authenticated = session.Authenticate(credential);

            if (!authenticated)
            {
                Console.WriteLine("Authentication failed");
                return false;
            }

            Console.WriteLine("Authenticated successfully");

            // Command execution phase
            Console.WriteLine("Executing command...");
            var result = session.ExecuteCommand("whoami");

            if (result.ExitCode == 0)
            {
                Console.WriteLine($"Logged in as: {result.Stdout.Trim()}");
                return true;
            }
            else
            {
                Console.WriteLine($"Command failed: {result.Stderr}");
                return false;
            }
        }
        catch (SshException ex) when (ex.Error == SshError.BannerRecv)
        {
            Console.WriteLine("Connection failed: Unable to connect to SSH server");
            Console.WriteLine("Check that the host and port are correct");
            return false;
        }
        catch (SshException ex) when (ex.Error == SshError.AuthenticationFailed)
        {
            Console.WriteLine("Authentication failed: Invalid credentials");
            return false;
        }
        catch (SshException ex) when (ex.Error == SshError.Timeout)
        {
            Console.WriteLine("Operation timed out");
            Console.WriteLine("Check network connectivity or increase timeout");
            return false;
        }
        catch (SshException ex) when (ex.Error == SshError.DevWrongUse)
        {
            Console.WriteLine($"API usage error: {ex.Message}");
            Console.WriteLine("This is a programming error - check session state");
            return false;
        }
        catch (SshException ex)
        {
            Console.WriteLine($"SSH error: {ex.Message}");
            Console.WriteLine($"Error code: {ex.Error}");

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Caused by: {ex.InnerException.Message}");
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return false;
        }
        finally
        {
            session.Dispose();
        }
    }
}
```

## Best Practices

1. **Always use try-catch-finally**:
   - Catch `SshException` for SSH-specific errors
   - Use `finally` block to dispose the session

2. **Check specific error codes**:
   - Use `when` clauses to handle specific errors differently
   - Provide user-friendly messages based on error type

3. **Don't ignore errors**:
   - Log all errors for troubleshooting
   - Provide meaningful feedback to users

4. **Dispose sessions properly**:
   - Always dispose sessions in `finally` blocks or use `using` statements
   - Prevents resource leaks

5. **Distinguish between SSH errors and command failures**:
   - SSH errors throw exceptions
   - Command failures set `ExitCode` to non-zero but don't throw

6. **Handle network errors gracefully**:
   - Connection errors are common in network applications
   - Implement retry logic for transient failures

7. **Check InnerException**:
   - Some errors wrap underlying exceptions
   - InnerException may contain valuable diagnostic information

## See Also

- `SshException` (SshException.cs:13) - Exception class for SSH errors
- `SshError` (SshError.cs:12) - Enumeration of all error codes
- [Session Lifecycle](session-lifecycle.md) - Understanding state-related errors
- [Authentication](authentication.md) - Handling authentication-specific errors
- [Command Execution](command-execution.md) - Command execution error handling
- [File Transfer with SCP](scp.md) - File transfer error handling
- [Algorithm and Method Preferences](algorithm-preferences.md) - Algorithm negotiation errors
- [Session Timeouts](session-timeouts.md) - Timeout errors
