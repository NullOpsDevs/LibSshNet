# NullOpsDevs.LibSsh

A modern, cross-platform .NET library providing managed bindings for libssh2, enabling SSH operations including remote command execution, SCP file transfers, and advanced terminal (PTY) features.

[![NuGet](https://img.shields.io/badge/nuget-1.0.0-blue.svg)](https://www.nuget.org/packages/NullOpsDevs.LibSsh/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)

## Features

| Feature        | Subfeature                   | Supported |
|----------------|------------------------------|-----------|
| Authentication |                              |           |
|                | Password                     | ✅         |
|                | Agent                        | ✅         |
|                | Host based                   | ✅         |
|                | Private key                  | ✅         |
| SCP            |                              |           |
|                | File upload                  | ✅         |
|                | File download                | ✅         |
| Commands       |                              |           |
|                | One-shot command execution   | ✅         |
|                | PTY                          | ✅         |
|                | Terminal type/mode selection | ✅         |
|                | Shell mode                   | ❌         |


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
