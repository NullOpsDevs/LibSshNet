# Algorithm and Method Preferences

SSH uses various cryptographic algorithms for key exchange, encryption, message authentication, and compression. NullOpsDevs.LibSsh allows you to configure which algorithms to prefer and in what order during the SSH handshake.

## Why Configure Algorithms?

By default, libssh2 negotiates algorithms automatically. However, you might want to customize preferences to:
- **Enforce strong security standards** - Reject weak or deprecated algorithms
- **Ensure compatibility** - Support specific server requirements
- **Meet compliance requirements** - Satisfy security policies or regulations
- **Troubleshoot connection issues** - Debug algorithm negotiation failures

## Quick Start: Secure Defaults

The easiest way to use strong algorithms is to apply the secure defaults:

```c#
using NullOpsDevs.LibSsh;

var session = new SshSession();

// Apply secure algorithm preferences before connecting
session.SetSecureMethodPreferences();

session.Connect("example.com", 22);
```

This configures:
- **Key Exchange**: Curve25519, ECDH with NIST curves, DH group exchange
- **Host Keys**: Ed25519, ECDSA, RSA-SHA2 (no DSA)
- **Ciphers**: ChaCha20-Poly1305, AES-GCM, AES-CTR
- **MACs**: HMAC-SHA2-256/512 with encrypt-then-MAC
- **Compression**: None

## SSH Method Types

SSH uses different algorithm types for different purposes:

| Method Type | Purpose | Examples |
|-------------|---------|----------|
| `Kex` | Key exchange | curve25519-sha256, diffie-hellman-group14-sha256 |
| `HostKey` | Server authentication | ssh-ed25519, rsa-sha2-256, ecdsa-sha2-nistp256 |
| `CryptCs` | Encryption (Client→Server) | aes256-ctr, chacha20-poly1305@openssh.com |
| `CryptSc` | Encryption (Server→Client) | aes256-ctr, chacha20-poly1305@openssh.com |
| `MacCs` | Message auth (Client→Server) | hmac-sha2-256, hmac-sha2-512 |
| `MacSc` | Message auth (Server→Client) | hmac-sha2-256, hmac-sha2-512 |
| `CompCs` | Compression (Client→Server) | none, zlib |
| `CompSc` | Compression (Server→Client) | none, zlib |

## Setting Custom Preferences

Configure specific algorithms for any method type before connecting:

```c#
using NullOpsDevs.LibSsh.Core;

var session = new SshSession();

// Only accept Ed25519 and ECDSA host keys (no RSA or DSA)
session.SetMethodPreferences(
    SshMethod.HostKey,
    "ssh-ed25519,ecdsa-sha2-nistp521,ecdsa-sha2-nistp384,ecdsa-sha2-nistp256"
);

// Use only ChaCha20 or AES-GCM ciphers
session.SetMethodPreferences(
    SshMethod.CryptCs,
    "chacha20-poly1305@openssh.com,aes256-gcm@openssh.com,aes128-gcm@openssh.com"
);

session.SetMethodPreferences(
    SshMethod.CryptSc,
    "chacha20-poly1305@openssh.com,aes256-gcm@openssh.com,aes128-gcm@openssh.com"
);

session.Connect("example.com", 22);
```

### Preference Format

Algorithm preferences are specified as **comma-separated lists** in priority order:
- First algorithm is most preferred
- Last algorithm is least preferred
- Server and client negotiate the first mutually-supported algorithm

```c#
// Most preferred → Least preferred
session.SetMethodPreferences(
    SshMethod.Kex,
    "curve25519-sha256,ecdh-sha2-nistp521,ecdh-sha2-nistp256,diffie-hellman-group14-sha256"
);
```

## Checking Negotiated Algorithms

After connecting, verify which algorithms were actually negotiated:

```c#
var session = new SshSession();
session.SetSecureMethodPreferences();
session.Connect("example.com", 22);

// Check what was negotiated
string? kexAlgorithm = session.GetNegotiatedMethod(SshMethod.Kex);
string? hostKeyAlgorithm = session.GetNegotiatedMethod(SshMethod.HostKey);
string? cipherCs = session.GetNegotiatedMethod(SshMethod.CryptCs);
string? macCs = session.GetNegotiatedMethod(SshMethod.MacCs);

Console.WriteLine($"Key Exchange: {kexAlgorithm}");
Console.WriteLine($"Host Key: {hostKeyAlgorithm}");
Console.WriteLine($"Cipher (Client→Server): {cipherCs}");
Console.WriteLine($"MAC (Client→Server): {macCs}");
```

Output example:
```
Key Exchange: curve25519-sha256
Host Key: ssh-ed25519
Cipher (Client→Server): chacha20-poly1305@openssh.com
MAC (Client→Server): hmac-sha2-256-etm@openssh.com
```

## Common Algorithm Configurations

### Maximum Security (Restrictive)

```c#
var session = new SshSession();

// Only the strongest algorithms
session.SetMethodPreferences(SshMethod.Kex, "curve25519-sha256");
session.SetMethodPreferences(SshMethod.HostKey, "ssh-ed25519");
session.SetMethodPreferences(SshMethod.CryptCs, "chacha20-poly1305@openssh.com");
session.SetMethodPreferences(SshMethod.CryptSc, "chacha20-poly1305@openssh.com");
session.SetMethodPreferences(SshMethod.MacCs, "hmac-sha2-256-etm@openssh.com");
session.SetMethodPreferences(SshMethod.MacSc, "hmac-sha2-256-etm@openssh.com");
session.SetMethodPreferences(SshMethod.CompCs, "none");
session.SetMethodPreferences(SshMethod.CompSc, "none");

session.Connect("example.com", 22);
```

### Legacy Compatibility

```c#
var session = new SshSession();

// Support older servers (includes weaker algorithms)
session.SetMethodPreferences(
    SshMethod.HostKey,
    "ssh-ed25519,ecdsa-sha2-nistp256,rsa-sha2-512,rsa-sha2-256,ssh-rsa"
);

session.SetMethodPreferences(
    SshMethod.CryptCs,
    "chacha20-poly1305@openssh.com,aes256-ctr,aes192-ctr,aes128-ctr,aes256-cbc"
);

session.Connect("legacy-server.com", 22);
```

### PCI-DSS Compliance Example

```c#
var session = new SshSession();

// Example compliance configuration (verify with your requirements)
session.SetMethodPreferences(
    SshMethod.Kex,
    "curve25519-sha256,ecdh-sha2-nistp521,ecdh-sha2-nistp384,diffie-hellman-group16-sha512"
);

session.SetMethodPreferences(
    SshMethod.HostKey,
    "ssh-ed25519,ecdsa-sha2-nistp521,ecdsa-sha2-nistp384,rsa-sha2-512"
);

session.SetMethodPreferences(
    SshMethod.CryptCs,
    "chacha20-poly1305@openssh.com,aes256-gcm@openssh.com,aes256-ctr"
);

session.SetMethodPreferences(SshMethod.CryptSc, "chacha20-poly1305@openssh.com,aes256-gcm@openssh.com,aes256-ctr");
session.SetMethodPreferences(SshMethod.MacCs, "hmac-sha2-256-etm@openssh.com,hmac-sha2-512-etm@openssh.com");
session.SetMethodPreferences(SshMethod.MacSc, "hmac-sha2-256-etm@openssh.com,hmac-sha2-512-etm@openssh.com");
session.SetMethodPreferences(SshMethod.CompCs, "none");
session.SetMethodPreferences(SshMethod.CompSc, "none");

session.Connect("example.com", 22);
```

## Algorithm Recommendations

### Key Exchange (Kex)

| Algorithm | Security | Compatibility | Recommendation |
|-----------|----------|---------------|----------------|
| `curve25519-sha256` | ✅ Excellent | ✅ Modern servers | **Recommended** |
| `ecdh-sha2-nistp521` | ✅ Good | ✅ Widely supported | Good choice |
| `ecdh-sha2-nistp384` | ✅ Good | ✅ Widely supported | Good choice |
| `ecdh-sha2-nistp256` | ✅ Good | ✅ Widely supported | Good choice |
| `diffie-hellman-group-exchange-sha256` | ✅ Good | ⚠️ Some servers | Acceptable |
| `diffie-hellman-group16-sha512` | ✅ Good | ⚠️ Some servers | Acceptable |
| `diffie-hellman-group14-sha256` | ⚠️ Adequate | ✅ Good | Fallback only |
| `diffie-hellman-group14-sha1` | ❌ Weak | ✅ Good | **Avoid** |
| `diffie-hellman-group1-sha1` | ❌ Broken | ✅ Legacy | **Never use** |

### Host Key Algorithms

| Algorithm | Security | Compatibility | Recommendation |
|-----------|----------|---------------|----------------|
| `ssh-ed25519` | ✅ Excellent | ✅ Modern servers | **Recommended** |
| `ecdsa-sha2-nistp521` | ✅ Good | ✅ Widely supported | Good choice |
| `ecdsa-sha2-nistp384` | ✅ Good | ✅ Widely supported | Good choice |
| `ecdsa-sha2-nistp256` | ✅ Good | ✅ Widely supported | Good choice |
| `rsa-sha2-512` | ✅ Good | ✅ Widely supported | Acceptable |
| `rsa-sha2-256` | ✅ Good | ✅ Widely supported | Acceptable |
| `ssh-rsa` (SHA-1) | ❌ Weak | ✅ Legacy | **Avoid** |
| `ssh-dss` | ❌ Broken | ✅ Legacy | **Never use** |

### Ciphers

| Algorithm | Security | Performance | Recommendation |
|-----------|----------|-------------|----------------|
| `chacha20-poly1305@openssh.com` | ✅ Excellent | ✅ Fast | **Recommended** |
| `aes256-gcm@openssh.com` | ✅ Excellent | ✅ Fast (with AES-NI) | **Recommended** |
| `aes128-gcm@openssh.com` | ✅ Good | ✅ Fast (with AES-NI) | Good choice |
| `aes256-ctr` | ✅ Good | ✅ Good | Acceptable |
| `aes192-ctr` | ✅ Good | ✅ Good | Acceptable |
| `aes128-ctr` | ✅ Good | ✅ Good | Acceptable |
| `aes256-cbc` | ⚠️ Adequate | ⚠️ Slower | Fallback only |
| `3des-cbc` | ❌ Weak | ❌ Very slow | **Avoid** |

### MACs

| Algorithm | Security | Recommendation |
|-----------|----------|----------------|
| `hmac-sha2-256-etm@openssh.com` | ✅ Excellent | **Recommended** |
| `hmac-sha2-512-etm@openssh.com` | ✅ Excellent | **Recommended** |
| `hmac-sha2-256` | ✅ Good | Good choice |
| `hmac-sha2-512` | ✅ Good | Good choice |
| `hmac-sha1-etm@openssh.com` | ⚠️ Adequate | Fallback only |
| `hmac-sha1` | ❌ Weak | **Avoid** |
| `hmac-md5` | ❌ Broken | **Never use** |

### Compression

| Algorithm | Use Case | Recommendation |
|-----------|----------|----------------|
| `none` | Always | **Recommended** |
| `zlib` | Slow networks only | Rarely needed |
| `zlib@openssh.com` | Slow networks only | Rarely needed |

> **Note**: Compression is generally not recommended for SSH. It adds CPU overhead and can expose information through compression side-channels (CRIME attack).

## Troubleshooting Algorithm Negotiation

### Connection fails with "no matching algorithms"

```c#
try
{
    session.Connect("example.com", 22);
}
catch (SshException ex) when (ex.Error == SshError.KexFailure)
{
    Console.WriteLine("Key exchange failed - no common algorithms");
    Console.WriteLine("Try adding more algorithms to your preferences");
}
```

**Solution**: Add more algorithms to support older servers:
```c#
session.SetMethodPreferences(
    SshMethod.Kex,
    "curve25519-sha256,ecdh-sha2-nistp256,diffie-hellman-group14-sha256"
);
```

### Want to verify security of negotiated algorithms

```c#
var session = new SshSession();
session.SetSecureMethodPreferences();
session.Connect("example.com", 22);

// Verify only strong algorithms were negotiated
var kex = session.GetNegotiatedMethod(SshMethod.Kex);
var hostKey = session.GetNegotiatedMethod(SshMethod.HostKey);

if (kex?.Contains("sha1") == true || hostKey == "ssh-rsa")
{
    Console.WriteLine("WARNING: Weak algorithms detected!");
    session.Dispose();
    return;
}

Console.WriteLine("Strong algorithms confirmed ✓");
```

## Best Practices

1. **Always set preferences before connecting**:
   ```c#
   session.SetSecureMethodPreferences(); // Must be before Connect()
   session.Connect("example.com", 22);
   ```

2. **Use secure defaults unless you have specific needs**:
   ```c#
   // Start with this
   session.SetSecureMethodPreferences();

   // Only customize if necessary
   ```

3. **Log negotiated algorithms in production**:
   ```c#
   logger.LogInformation($"Connected with KEX: {session.GetNegotiatedMethod(SshMethod.Kex)}");
   logger.LogInformation($"Host key: {session.GetNegotiatedMethod(SshMethod.HostKey)}");
   ```

4. **Test with your target servers**:
   - Different servers support different algorithms
   - Test your configuration before deploying

5. **Prefer modern algorithms**:
   - Ed25519 over RSA for host keys
   - Curve25519 over DH for key exchange
   - ChaCha20-Poly1305 or AES-GCM for ciphers

6. **Disable weak algorithms**:
   - Never include SHA-1 based algorithms in production
   - Avoid CBC-mode ciphers
   - Never use DSA keys

## Complete Example

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Core;
using NullOpsDevs.LibSsh.Credentials;
using NullOpsDevs.LibSsh.Exceptions;

public class SecureConnection
{
    public void ConnectWithStrongAlgorithms(string host, int port)
    {
        var session = new SshSession();

        try
        {
            // Apply secure algorithm preferences
            session.SetSecureMethodPreferences();

            Console.WriteLine("Connecting with secure algorithms...");
            session.Connect(host, port);

            // Verify what was negotiated
            Console.WriteLine("\nNegotiated Algorithms:");
            Console.WriteLine($"  Key Exchange: {session.GetNegotiatedMethod(SshMethod.Kex)}");
            Console.WriteLine($"  Host Key: {session.GetNegotiatedMethod(SshMethod.HostKey)}");
            Console.WriteLine($"  Cipher (C→S): {session.GetNegotiatedMethod(SshMethod.CryptCs)}");
            Console.WriteLine($"  Cipher (S→C): {session.GetNegotiatedMethod(SshMethod.CryptSc)}");
            Console.WriteLine($"  MAC (C→S): {session.GetNegotiatedMethod(SshMethod.MacCs)}");
            Console.WriteLine($"  MAC (S→C): {session.GetNegotiatedMethod(SshMethod.MacSc)}");

            // Authenticate and use the connection
            var credential = SshCredential.FromPublicKeyFile(
                "username",
                "~/.ssh/id_ed25519.pub",
                "~/.ssh/id_ed25519"
            );

            if (session.Authenticate(credential))
            {
                Console.WriteLine("\nAuthentication successful!");
                var result = session.ExecuteCommand("whoami");
                Console.WriteLine($"Logged in as: {result.Stdout.Trim()}");
            }
        }
        catch (SshException ex)
        {
            Console.WriteLine($"SSH error: {ex.Message}");
        }
        finally
        {
            session.Dispose();
        }
    }
}
```

## See Also

- `SshSession.SetMethodPreferences()` (SshSession.cs:215) - Set custom algorithm preferences
- `SshSession.SetSecureMethodPreferences()` (SshSession.cs:240) - Apply secure defaults
- `SshSession.GetNegotiatedMethod()` (SshSession.cs:189) - Check negotiated algorithms
- `SshMethod` enum (SshSession.cs) - Available method types
- [Host Key Retrieval and Verification](host-key-retrieval-and-verification.md) - Verify server identity
- [Error Handling](error-handling.md) - Handle algorithm negotiation errors
