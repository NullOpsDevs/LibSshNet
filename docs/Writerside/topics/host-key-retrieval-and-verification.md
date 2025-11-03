# Host Key Retrieval and Verification

Host key verification is a critical security mechanism in SSH that prevents man-in-the-middle (MITM) attacks. When you connect to an SSH server, the server presents its host key to prove its identity. You should verify this key matches what you expect before proceeding with authentication.

## Why Host Key Verification Matters

Without host key verification, an attacker could intercept your connection and impersonate the legitimate server. This would allow them to:
- Capture your authentication credentials
- Monitor all data transmitted during the session
- Manipulate commands and responses

Traditional SSH clients (like OpenSSH) maintain a `known_hosts` file that stores fingerprints of previously-seen servers and warn users when keys change. NullOpsDevs.LibSsh provides the APIs to implement similar verification in your applications.

## Basic Host Key Retrieval

After connecting to a server (but before authenticating), you can retrieve the server's host key:

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Core;

var session = new SshSession();
session.Connect("example.com", 22);

// Retrieve the host key
SshHostKey hostKey = session.GetHostKey();

Console.WriteLine($"Host key type: {hostKey.Type}");
Console.WriteLine($"Host key length: {hostKey.Key.Length} bytes");
```

The `SshHostKey` structure contains:
- `Key` - The raw host key data as a byte array
- `Type` - The key algorithm type (see [Host Key Types](#host-key-types))

## Host Key Types

The server's host key can use one of several cryptographic algorithms:

| Type | Description | Recommended |
|------|-------------|-------------|
| `Ed25519` | Ed25519 elliptic curve signature algorithm | ✅ **Recommended** for new deployments |
| `Ecdsa256` | ECDSA with NIST P-256 curve | ✅ Good choice |
| `Ecdsa384` | ECDSA with NIST P-384 curve | ✅ Good choice |
| `Ecdsa521` | ECDSA with NIST P-521 curve | ✅ Good choice |
| `Rsa` | RSA public key algorithm | ⚠️ Acceptable with sufficient key size (2048+ bits) |
| `Dss` | DSA algorithm (deprecated) | ❌ **Avoid** - deprecated and insecure |

```c#
SshHostKey hostKey = session.GetHostKey();

switch (hostKey.Type)
{
    case SshHostKeyType.Ed25519:
        Console.WriteLine("Server uses Ed25519 (recommended)");
        break;
    case SshHostKeyType.Rsa:
        Console.WriteLine("Server uses RSA (ensure key is 2048+ bits)");
        break;
    case SshHostKeyType.Dss:
        Console.WriteLine("WARNING: Server uses deprecated DSA keys!");
        break;
    // ... handle other types
}
```

## Computing Host Key Fingerprints

To make host keys human-readable and easier to verify, you can compute cryptographic fingerprints (hashes) of the key:

```c#
var session = new SshSession();
session.Connect("example.com", 22);

// Get fingerprints using different hash algorithms
byte[] sha256Hash = session.GetHostKeyHash(SshHashType.SHA256);
byte[] sha1Hash = session.GetHostKeyHash(SshHashType.SHA1);
byte[] md5Hash = session.GetHostKeyHash(SshHashType.MD5);

Console.WriteLine($"SHA256: {ConvertToFingerprint(sha256Hash, "SHA256")}");
Console.WriteLine($"SHA1:   {ConvertToFingerprint(sha1Hash, "SHA1")}");
Console.WriteLine($"MD5:    {ConvertToFingerprint(md5Hash, "MD5")}");

// Helper method to format fingerprints
static string ConvertToFingerprint(byte[] hash, string algorithm)
{
    string hex = BitConverter.ToString(hash).Replace("-", ":");
    return $"{algorithm}:{hex}";
}
```

### Hash Algorithm Comparison

| Algorithm | Output Size | Security | Use Case |
|-----------|-------------|----------|----------|
| `SHA256` | 32 bytes (256 bits) | ✅ Strong | **Recommended** for new implementations |
| `SHA1` | 20 bytes (160 bits) | ⚠️ Weak | Legacy compatibility only |
| `MD5` | 16 bytes (128 bits) | ❌ Broken | Legacy compatibility only |

> **Best Practice:** Always use SHA-256 for new implementations. MD5 and SHA-1 are cryptographically weak but may be needed for compatibility with older systems.

## Retrieving Negotiated Algorithms

After connection, you can inspect which algorithms were negotiated for various SSH protocol operations:

```c#
var session = new SshSession();
session.Connect("example.com", 22);

// Check negotiated algorithms
string? kexAlgorithm = session.GetNegotiatedMethod(SshMethod.Kex);
string? hostKeyAlgorithm = session.GetNegotiatedMethod(SshMethod.HostKey);
string? cipherCs = session.GetNegotiatedMethod(SshMethod.CryptCs);
string? macCs = session.GetNegotiatedMethod(SshMethod.MacCs);

Console.WriteLine($"Key Exchange: {kexAlgorithm}");
Console.WriteLine($"Host Key: {hostKeyAlgorithm}");
Console.WriteLine($"Cipher (Client→Server): {cipherCs}");
Console.WriteLine($"MAC (Client→Server): {macCs}");
```

This is useful for:
- Debugging connection issues
- Auditing security configurations
- Ensuring strong algorithms are being used

## Advanced: Configuring Accepted Host Key Types

You can restrict which host key types your client will accept by setting method preferences before connecting:

```c#
var session = new SshSession();

// Only accept Ed25519 and ECDSA host keys (reject RSA and DSA)
session.SetMethodPreferences(
    SshMethod.HostKey,
    "ssh-ed25519,ecdsa-sha2-nistp521,ecdsa-sha2-nistp384,ecdsa-sha2-nistp256"
);

session.Connect("example.com", 22);
```

Or use the secure defaults which already prefer modern algorithms:

```c#
var session = new SshSession();

// Apply secure algorithm preferences for all methods
session.SetSecureMethodPreferences();

session.Connect("example.com", 22);
```

The secure defaults prefer:
- **Host Keys:** Ed25519 → ECDSA (521/384/256) → RSA-SHA2 (no DSA or legacy RSA)
- **Key Exchange:** Curve25519 → ECDH with NIST curves → DH group exchange
- **Ciphers:** ChaCha20-Poly1305 → AES-GCM → AES-CTR
- **MACs:** HMAC-SHA2 with encrypt-then-MAC

See `SshSession.cs:240-249` for the complete list of secure defaults.

## Complete Secure Connection Example

Here's a complete example that combines host key verification with authentication:

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Core;
using NullOpsDevs.LibSsh.Credentials;

var session = new SshSession();

try
{
    // 1. Apply secure algorithm preferences
    session.SetSecureMethodPreferences();

    // 2. Connect to the server
    session.Connect("example.com", 22);

    // 3. Verify host key
    byte[] hostKeyHash = session.GetHostKeyHash(SshHashType.SHA256);
    string fingerprint = Convert.ToBase64String(hostKeyHash);

    Console.WriteLine($"Server fingerprint: SHA256:{fingerprint}");
    Console.WriteLine("Verify this matches your known fingerprint!");

    // In production, compare against a known-good value
    // if (fingerprint != expectedFingerprint) { throw ... }

    // 4. Authenticate
    var credential = SshCredential.FromPublicKeyFile(
        "username",
        "~/.ssh/id_ed25519.pub",
        "~/.ssh/id_ed25519"
    );

    if (!session.Authenticate(credential))
    {
        Console.WriteLine("Authentication failed!");
        return;
    }

    // 5. Execute commands securely
    var result = session.ExecuteCommand("whoami");
    Console.WriteLine($"Logged in as: {result.Stdout.Trim()}");
}
finally
{
    session.Dispose();
}
```

## Security Recommendations

1. **Always verify host keys** before authenticating - never blindly trust server identities
2. **Use SHA-256 fingerprints** for new implementations (avoid MD5 and SHA-1)
3. **Implement key pinning** for production systems (pre-configure known fingerprints)
4. **Monitor for key changes** - a changed host key may indicate an attack or server redeployment
5. **Use secure algorithm preferences** to reject weak cryptographic algorithms
6. **Log verification failures** for security auditing and incident response
7. **Educate users** about the importance of verifying fingerprints when prompted

## See Also

- [Authentication](authentication.md) - Learn about authentication methods after host key verification
- [Algorithm and Method Preferences](algorithm-preferences.md) - Configure accepted algorithms
- [Session Lifecycle](session-lifecycle.md) - When to verify host keys
- [Error Handling](error-handling.md) - Handle host key verification errors
- [Quickstart](quickstart.md) - Complete connection examples
- `SshSession.GetHostKey()` (SshSession.cs:156) - Retrieve the raw host key
- `SshSession.GetHostKeyHash()` (SshSession.cs:347) - Compute host key fingerprints
- `SshSession.SetSecureMethodPreferences()` (SshSession.cs:240) - Configure secure algorithm defaults
