# Session Timeouts

Session timeouts control how long SSH operations wait before giving up. This is different from keepalives - timeouts prevent operations from hanging indefinitely, while keepalives prevent idle connections from being closed.

## Understanding Timeouts

By default, libssh2 has **no timeout** - operations will wait indefinitely until they complete or fail. Setting a timeout ensures operations don't hang forever.

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Credentials;

var session = new SshSession();
session.Connect("example.com", 22);

// Set a 30-second timeout for all operations
session.SetSessionTimeout(TimeSpan.FromSeconds(30));

session.Authenticate(SshCredential.FromPassword("user", "password"));

// All subsequent operations will timeout after 30 seconds
var result = session.ExecuteCommand("some-command");
```

## Setting a Timeout

Configure the timeout after connecting but before or after authenticating:

```c#
var session = new SshSession();
session.Connect("example.com", 22);

// Set a 2-minute timeout
session.SetSessionTimeout(TimeSpan.FromMinutes(2));

session.Authenticate(credential);

// This command will timeout if it takes longer than 2 minutes
try
{
    var result = session.ExecuteCommand("long-running-command");
    Console.WriteLine(result.Stdout);
}
catch (SshException ex) when (ex.Error == SshError.Timeout)
{
    Console.WriteLine("Command timed out after 2 minutes");
}
```

## Disabling Timeouts

To restore the default behavior (wait indefinitely):

```c#
// Disable timeout - operations will wait forever
session.DisableSessionTimeout();

// This command will never timeout
var result = session.ExecuteCommand("very-long-command");
```

## When to Use Timeouts

### Use Timeouts When:

1. **Executing commands that might hang**
   ```c#
   session.SetSessionTimeout(TimeSpan.FromMinutes(5));
   var result = session.ExecuteCommand("./unpredictable-script.sh");
   ```

2. **Transferring large files**
   ```c#
   session.SetSessionTimeout(TimeSpan.FromMinutes(30));
   using var stream = File.OpenRead("large-file.zip");
   session.WriteFile("/remote/path/large-file.zip", stream);
   ```

3. **In production environments**
   ```c#
   // Prevent operations from hanging indefinitely
   session.SetSessionTimeout(TimeSpan.FromMinutes(10));
   ```

4. **When connecting to unreliable servers**
   ```c#
   session.Connect("unreliable-server.com", 22);
   session.SetSessionTimeout(TimeSpan.FromSeconds(30));
   ```

### Don't Use Timeouts When:

1. **Running truly long operations** - Use cancellation tokens instead
2. **In development** - Let operations complete naturally to see actual behavior
3. **Operations are already bounded** - If the command itself has a timeout

## Timeout vs. Keepalive

These are different mechanisms with different purposes:

| Feature | Session Timeout | Keepalive |
|---------|----------------|-----------|
| **Purpose** | Prevent operations from hanging | Prevent idle disconnection |
| **When active** | During active operations | During idle periods |
| **What it does** | Aborts operation if too slow | Sends periodic messages |
| **Error on failure** | `SshError.Timeout` exception | Connection drops |
| **Configuration** | `SetSessionTimeout()` | `ConfigureKeepAlive()` + `SendKeepAlive()` |

## Using Both Together

For robust connection management, use both timeouts and keepalives:

```c#
var session = new SshSession();
session.Connect("example.com", 22);

// Set operation timeout to 5 minutes
session.SetSessionTimeout(TimeSpan.FromMinutes(5));

// Configure keepalive to send every 30 seconds when idle
session.ConfigureKeepAlive(wantReply: false, interval: TimeSpan.FromSeconds(30));

session.Authenticate(credential);

// Execute command with timeout protection
var result = session.ExecuteCommand("some-command");

// When idle, manually send keepalives
while (waiting)
{
    Thread.Sleep(30_000);
    session.SendKeepAlive(); // Keep connection alive during idle
}
```

## Handling Timeout Errors

When an operation times out, you'll receive an `SshException` with `SshError.Timeout`:

```c#
try
{
    session.SetSessionTimeout(TimeSpan.FromSeconds(10));
    var result = session.ExecuteCommand("sleep 30"); // This will timeout
}
catch (SshException ex) when (ex.Error == SshError.Timeout)
{
    Console.WriteLine("Operation timed out!");
    Console.WriteLine("The command took too long to execute");

    // Decide what to do:
    // - Retry with a longer timeout?
    // - Cancel the operation?
    // - Notify the user?
}
catch (SshException ex)
{
    Console.WriteLine($"Other SSH error: {ex.Message}");
}
```

## Timeout Scope

The timeout applies to **blocking libssh2 operations**, including:
- Command execution (`ExecuteCommand`)
- File transfers (`ReadFile`, `WriteFile`)
- Authentication (`Authenticate`)
- Keepalive messages (`SendKeepAlive`)

## Best Practices

1. **Set reasonable timeouts**:
   - Too short: Operations fail unnecessarily
   - Too long: Hung operations waste resources
   - Typical: 1-5 minutes for most operations

2. **Adjust for operation type**:
   ```c#
   // Short timeout for quick commands
   session.SetSessionTimeout(TimeSpan.FromSeconds(30));
   var result = session.ExecuteCommand("whoami");

   // Longer timeout for file transfers
   session.SetSessionTimeout(TimeSpan.FromMinutes(30));
   session.WriteFile("/path/large.zip", stream);
   ```

3. **Consider network conditions**:
   - Slower networks need longer timeouts
   - High-latency connections need more time
   - Unreliable networks may need retries

4. **Log timeout events**:
   ```c#
   try
   {
       var result = session.ExecuteCommand(command);
   }
   catch (SshException ex) when (ex.Error == SshError.Timeout)
   {
       logger.LogWarning($"Command timed out: {command}");
       throw;
   }
   ```

5. **Use cancellation tokens for long operations**:
   ```c#
   // Prefer cancellation tokens for user-initiated cancellation
   using var cts = new CancellationTokenSource();
   var result = await session.ExecuteCommandAsync(command, cancellationToken: cts.Token);
   ```

## Example: Configuring for Different Environments

```c#
public class SshConnectionManager
{
    public SshSession CreateSession(string environment)
    {
        var session = new SshSession();
        session.Connect(GetHostForEnvironment(environment), 22);

        // Configure based on environment
        switch (environment)
        {
            case "development":
                // No timeout in dev - let things run
                session.DisableSessionTimeout();
                break;

            case "staging":
                // Moderate timeout for staging
                session.SetSessionTimeout(TimeSpan.FromMinutes(10));
                break;

            case "production":
                // Strict timeout in production
                session.SetSessionTimeout(TimeSpan.FromMinutes(5));
                session.ConfigureKeepAlive(false, TimeSpan.FromSeconds(30));
                break;
        }

        return session;
    }

    private string GetHostForEnvironment(string env) =>
        env switch
        {
            "development" => "dev-server.local",
            "staging" => "staging-server.example.com",
            "production" => "prod-server.example.com",
            _ => throw new ArgumentException($"Unknown environment: {env}")
        };
}
```

## See Also

- `SshSession.SetSessionTimeout()` (SshSession.cs:276) - Set operation timeout
- `SshSession.DisableSessionTimeout()` (SshSession.cs:259) - Disable timeout
- [Keeping Connection Alive](keeping-connection-alive.md) - Prevent idle disconnections
- [Error Handling](error-handling.md) - Handling timeout errors
