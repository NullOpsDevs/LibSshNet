# Authentication

You can authenticate to a server using one of the following methods.

## Authenticate via username and password

```c#
sshSession.Authenticate(SshCredential.FromPassword("user", "password"));
```

## Host-based authentication

```c#
sshSession.Authenticate(SshCredential.FromHostBased(
    "username",
    "~/.ssh/id_rsa.pub",
    "~/.ssh/id_rsa",
    "optional_passphrase",
    "hostname",
    "local username"));
```

## Key-based authentication (file)

```c#
sshSession.Authenticate(SshCredential.FromPublicKeyFile(
    "username",
    "~/.ssh/id_rsa.pub",
    "~/.ssh/id_rsa",
    "optional_passphrase"));
```

## Key-based authentication (memory)

```c#
sshSession.Authenticate(SshCredential.FromPublicKeyMemory(
    "username",
    new byte[]{ /* public key bytes */ },
    new byte[]{ /* private key bytes */},
    "optional_passphrase"));
```

## Key-based authentication (agent)

```c#
sshSession.Authenticate(SshCredential.FromAgent("username"));
```
