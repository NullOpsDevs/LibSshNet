# Authentication

After establishing a connection to an SSH server, you must authenticate before you can execute commands or transfer files. `NullOpsDevs.LibSsh` supports multiple authentication methods to suit different security requirements and deployment scenarios.

## Overview

Authentication is performed by calling the `Authenticate()` method on your `SshSession` instance with an appropriate `SshCredential` object. The session must be in the `Connected` state before authenticating.

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Credentials;

var session = new SshSession();
session.Connect("example.com", 22);

// Authenticate using your chosen method
bool success = session.Authenticate(credential);

if (success)
{
    // Session is now in LoggedIn state
    // You can execute commands, transfer files, etc.
}
```

## Password Authentication

Password authentication is the simplest method but is generally less secure than key-based authentication. It's suitable for development environments or when key-based authentication isn't available.

```c#
var credential = SshCredential.FromPassword("username", "password");
session.Authenticate(credential);
```

<warning>
<b>Security Note:</b> Password authentication transmits credentials over an encrypted channel, but passwords can be compromised through various attacks. Consider using key-based authentication for production environments.
</warning>

## Public Key Authentication (File-based)

Public key authentication is the recommended method for most use cases. It uses asymmetric cryptography where you authenticate with a private key that corresponds to a public key registered on the server.

### Basic Usage

```c#
var credential = SshCredential.FromPublicKeyFile(
    username: "username",
    publicKeyPath: "~/.ssh/id_rsa.pub",
    privateKeyPath: "~/.ssh/id_rsa",
    passphrase: null  // or provide passphrase if key is encrypted
);

session.Authenticate(credential);
```

### With Encrypted Private Key

```c#
var credential = SshCredential.FromPublicKeyFile(
    username: "username",
    publicKeyPath: "~/.ssh/id_ed25519.pub",
    privateKeyPath: "~/.ssh/id_ed25519",
    passphrase: "my-secure-passphrase"
);

session.Authenticate(credential);
```

> **Best Practice:** Use Ed25519 keys (`ssh-keygen -t ed25519`) for new deployments. They offer better security and performance than RSA keys.

## Public Key Authentication (Memory-based)

When you need to load keys from sources other than the filesystem (e.g., databases, configuration systems, or encrypted stores), you can use memory-based authentication.

```c#
byte[] publicKeyData = LoadPublicKeyFromSecureStore();
byte[] privateKeyData = LoadPrivateKeyFromSecureStore();

var credential = SshCredential.FromPublicKeyMemory(
    username: "username",
    publicKeyData: publicKeyData,
    privateKeyData: privateKeyData,
    passphrase: null  // or provide passphrase if key is encrypted
);

session.Authenticate(credential);
```

> **Note:** The key data should be in OpenSSH format (PEM). Both public and private key data must be provided as UTF-8 encoded byte arrays.

## SSH Agent Authentication

SSH agent authentication delegates key management to an SSH agent (ssh-agent on Linux/macOS, Pageant on Windows). This method is convenient when you have multiple keys or want to avoid storing private keys in your application.

```c#
var credential = SshCredential.FromAgent("username");
session.Authenticate(credential);
```

The SSH agent will:
1. Try each available identity in the agent
2. Return success when a valid key is found
3. Return failure if no keys authenticate successfully

> **Platform Note:** Ensure an SSH agent is running before using this method:
> - **Linux/macOS:** `eval $(ssh-agent)` and `ssh-add ~/.ssh/id_rsa`
> - **Windows:** Use Pageant or Windows OpenSSH Authentication Agent

## Host-based Authentication

Host-based authentication allows a trusted client host to authenticate users without requiring individual credentials. This method is rarely used in modern deployments and is typically restricted to tightly controlled environments.

```c#
var credential = SshCredential.FromHostBased(
    username: "username",
    publicKeyPath: "/etc/ssh/ssh_host_rsa_key.pub",
    privateKeyPath: "/etc/ssh/ssh_host_rsa_key",
    passphrase: null,
    hostname: "client-hostname.example.com",
    localUsername: "local-username"  // optional, defaults to username
);
session.Authenticate(credential);
```

> **Warning:** Host-based authentication requires server-side configuration (in `/etc/ssh/sshd_config` and `~/.shosts` or `/etc/ssh/shosts.equiv`) and is considered less secure than user-based authentication methods. Use with caution.

## Error Handling

Authentication failures can occur for various reasons. Always check the return value and handle failures appropriately:

```c#
try
{
    var credential = SshCredential.FromPassword("username", "wrong-password");
    bool success = session.Authenticate(credential);

    if (!success)
    {
        Console.WriteLine("Authentication failed. Check your credentials.");
        return;
    }
}
catch (SshException ex)
{
    Console.WriteLine($"SSH error during authentication: {ex.Message}");
}
```

Common authentication failures:
- **Invalid credentials:** Wrong password or key not authorized on server
- **Key format issues:** Unsupported key type or corrupted key file
- **Permission denied:** Server configuration doesn't allow the authentication method
- **Agent not available:** SSH agent not running when using agent authentication

## See Also

- [Host Key Retrieval and Verification](host-key-retrieval-and-verification.md) - Verify server identity before authenticating
- [Session Lifecycle](session-lifecycle.md) - Understanding session states during authentication
- [Command Execution](command-execution.md) - Execute commands after authenticating
- [File Transfer with SCP](scp.md) - Transfer files after authenticating
- [Error Handling](error-handling.md) - Handle authentication errors
- [Quickstart](quickstart.md) - Complete connection and authentication examples
