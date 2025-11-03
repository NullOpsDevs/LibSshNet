# Keeping Connection Alive

SSH connections can be terminated by network infrastructure (firewalls, NAT devices, load balancers) or the remote server when there's no activity for an extended period. If you're not actively using the connection for a long time, you need to send keepalive messages to prevent these timeouts.

## Why You Need Keepalives

When your SSH connection is idle (not executing commands or transferring files), it can be terminated:

- **Network devices may drop idle connections** - Firewalls and NAT gateways often have idle timeout policies (typically 5-15 minutes)
- **SSH servers may disconnect idle sessions** - Many servers have `ClientAliveInterval` configured to terminate inactive connections
- **Long-running operations may appear stalled** - Without activity, monitoring systems may flag the connection as dead

Keepalive messages solve this by sending periodic SSH protocol messages that keep the connection active.

## Configuring and Sending Keepalives

Configure keepalives after connecting and send them periodically when you're not actively using the connection:

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Credentials;

var session = new SshSession();
session.Connect("example.com", 22);

// Configure keepalive: send every 60 seconds, don't require server response
session.ConfigureKeepAlive(
    wantReply: false,
    interval: TimeSpan.FromSeconds(60)
);

session.Authenticate(SshCredential.FromPassword("user", "password"));

// When the connection is idle, send keepalives periodically
// For example, in a loop while waiting for something
while (waiting)
{
    Thread.Sleep(60_000); // Wait 60 seconds
    session.SendKeepAlive(); // Send keepalive to keep connection alive
}
```

### Parameters

- **wantReply**:
  - `false` - Send keepalive without expecting a reply (recommended, lower overhead)
  - `true` - Require the server to acknowledge keepalives (useful for detecting broken connections)

- **interval**:
  - The time between keepalive messages
  - Typical values: 30-60 seconds
  - Must be less than the smallest timeout in your network path

## Detecting Connection Failures

When `wantReply: true`, the server must acknowledge keepalives. This helps detect broken connections:

```c#
session.ConfigureKeepAlive(wantReply: true, interval: TimeSpan.FromSeconds(30));

try
{
    session.SendKeepAlive();
    Console.WriteLine("Connection is alive");
}
catch (SshException ex)
{
    Console.WriteLine("Connection appears to be broken!");
    // Reconnect or handle the failure
}
```

> **Note:** Using `wantReply: true` adds network round-trip overhead. Use it only when you need to actively monitor connection health.

## Best Practices

1. **Choose appropriate intervals**:
   - 30-60 seconds for typical use cases
   - Shorter intervals (15-30s) for restrictive networks
   - Avoid very short intervals (<15s) - they waste bandwidth

2. **Use wantReply: false by default**:
   - Lower overhead
   - Sufficient for most keepalive scenarios
   - Only use `wantReply: true` when actively monitoring connection health

3. **Only send keepalives when idle**:
   - No need to send keepalives while actively executing commands or transferring files
   - SSH activity naturally keeps the connection alive

4. **Coordinate with server timeouts**:
   - Set keepalive interval lower than server's `ClientAliveInterval`
   - Check network infrastructure timeouts (firewalls, load balancers)

## See Also

- `SshSession.ConfigureKeepAlive()` (SshSession.cs:325) - Configure keepalive settings
- `SshSession.SendKeepAlive()` (SshSession.cs:300) - Manually send a keepalive message
- [Session Timeouts](session-timeouts.md) - Understanding timeouts vs keepalives
- [Session Lifecycle](session-lifecycle.md) - When to send keepalives
- [Authentication](authentication.md) - Authenticate before using keepalives
- [Error Handling](error-handling.md) - Handle keepalive errors
