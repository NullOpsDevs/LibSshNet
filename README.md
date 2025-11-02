# NullOpsDevs.LibSsh

A modern, cross-platform .NET library providing managed bindings for libssh2, enabling SSH operations including remote command execution, SCP file transfers, and advanced terminal (PTY) features.

[![NuGet](https://img.shields.io/badge/nuget-1.0.0-blue.svg)](https://www.nuget.org/packages/NullOpsDevs.LibSsh/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)

## Features

| Category                | Feature                                  | Method(s)                                            | Supported |
|-------------------------|------------------------------------------|------------------------------------------------------|-----------|
| **Authentication**      |                                          |                                                      |           |
|                         | Password authentication                  | `Authenticate(PasswordCredential)`                   | ✅         |
|                         | Public key authentication                | `Authenticate(PublicKeyCredential)`                  | ✅         |
|                         | SSH agent authentication                 | `Authenticate(SshAgentCredential)`                   | ✅         |
|                         | Host-based authentication                | `Authenticate(HostBasedCredential)`                  | ✅         |
|                         | Keyboard-interactive authentication      | -                                                    | ❌         |
| **Session Management**  |                                          |                                                      |           |
|                         | Connection                               | `Connect`, `ConnectAsync`                            | ✅         |
|                         | Host key retrieval                       | `GetHostKey`                                         | ✅         |
|                         | Host key verification                    | `GetHostKeyHash`                                     | ✅         |
|                         | Session timeout configuration            | `SetSessionTimeout`, `DisableSessionTimeout`         | ✅         |
|                         | Keepalive configuration                  | `ConfigureKeepAlive`, `SendKeepAlive`                | ✅         |
|                         | Method preference configuration          | `SetMethodPreferences`                               | ✅         |
|                         | Secure default algorithms                | `SetSecureMethodPreferences`                         | ✅         |
|                         | Negotiated method inspection             | `GetNegotiatedMethod`                                | ✅         |
| **File Transfer (SCP)** |                                          |                                                      |           |
|                         | File upload                              | `WriteFile`, `WriteFileAsync`                        | ✅         |
|                         | File download                            | `ReadFile`, `ReadFileAsync`                          | ✅         |
| **Command Execution**   |                                          |                                                      |           |
|                         | One-shot command execution               | `ExecuteCommand`, `ExecuteCommandAsync`              | ✅         |
|                         | Exit code retrieval                      | `SshCommandResult.ExitCode`                          | ✅         |
|                         | Exit signal retrieval                    | `SshCommandResult.ExitSignal`                        | ✅         |
|                         | stdout/stderr separation                 | `SshCommandResult.Stdout`, `SshCommandResult.Stderr` | ✅         |
| **Terminal (PTY)**      |                                          |                                                      |           |
|                         | PTY allocation                           | `CommandExecutionOptions.RequestPty`                 | ✅         |
|                         | Terminal type selection                  | `CommandExecutionOptions.TerminalType`               | ✅         |
|                         | Terminal modes                           | `CommandExecutionOptions.TerminalModes`              | ✅         |
|                         | Window size configuration                | `CommandExecutionOptions.TerminalWidth/Height`       | ✅         |
|                         | Interactive shell mode                   | -                                                    | ❌         |
| **Error Handling**      |                                          |                                                      |           |
|                         | Typed exceptions                         | `SshException`                                       | ✅         |
|                         | Detailed error messages                  | `SshException.Message`                               | ✅         |
|                         | 60+ error code mappings                  | `SshError` enum                                      | ✅         |
| **Advanced Features**   |                                          |                                                      |           |
|                         | Host key type detection                  | `SshHostKey.Type`                                    | ✅         |
|                         | Microsoft.Extensions.Logging integration | Constructor `ILogger` parameter                      | ✅         |
|                         | Cross-platform native binaries           | Bundled in NuGet package                             | ✅         |
| **Thread Safety**       | `SshSession` is *NOT* thread-safe.       | -                                                    | ❌         |


## Installation

Install via NuGet Package Manager:

```bash
dotnet add package NullOpsDevs.LibSsh
```

Or via Package Manager Console:

```powershell
Install-Package NullOpsDevs.LibSsh
```

### Supported Platforms

- .NET 9.0+
- Windows (x64)
- Linux (x64, ARM64)
- macOS (x64, ARM64/Apple Silicon)

## Quick Start

For a quick start, I'd suggest looking into the [NullOpsDevs.LibSsh.Test](NullOpsDevs.LibSsh.Test) project.

## Building from Source

### Prerequisites

- .NET 9.0 SDK or later
- Git

### Build Steps

```bash
# Clone the repository
git clone https://github.com/NullOpsDevs/LibSshNet.git
cd LibSshNet

# Restore dependencies and build
dotnet restore
dotnet build

# Run tests
cd NullOpsDevs.LibSsh.Test
docker compose up -d
dotnet run
```

## Architecture

The library consists of three layers:

1. **Managed Layer**: Clean, idiomatic C# API with async/await support (not true async)
2. **Interop Layer**: P/Invoke bindings to libssh2 native library
3. **Native Layer**: Pre-compiled libssh2 binaries for all supported platforms

All native dependencies are bundled in the NuGet package for zero-configuration deployment.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Acknowledgments

This library uses [libssh2](https://www.libssh2.org/), a client-side C library implementing the SSH2 protocol.

libssh2 is licensed under [BSD License](https://libssh2.org/license.html).
