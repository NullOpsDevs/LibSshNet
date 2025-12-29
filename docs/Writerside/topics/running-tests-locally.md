# Running Tests Locally

This guide walks you through setting up and running the NullOpsDevs.LibSsh test suite on your local machine.

## Prerequisites

Before running tests, ensure you have the following installed:

- **.NET 8.0 or 9.0 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop)
  - Windows: Docker Desktop with WSL 2 backend
  - Linux: Docker Engine and Docker Compose
  - macOS: Docker Desktop

## Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/NullOpsDevs/LibSshNet.git
cd LibSshNet
```

### 2. Start the SSH Test Server

The test suite requires a Docker-based SSH server to be running:

```bash
cd src/NullOpsDevs.LibSsh.Test
docker compose up -d
```

Wait for the container to be healthy (usually takes 5-10 seconds):

```bash
docker compose ps
```

You should see:

```
NAME                  IMAGE                      STATUS
libssh-test-server    libssh-test-server:latest  Up (healthy)
```

### 3. Run the Tests

From the test project directory, simply run:

```bash
dotnet run
```

The test runner will:
1. Load the native libssh2 library
2. Wait for Docker containers to be ready
3. Run all test categories
4. Display a summary of results

## Test Categories

The test suite includes the following categories:

### Authentication Tests
- Retrieve negotiated methods
- Host key retrieval
- Host key hash retrieval
- Password authentication
- Public key authentication (with and without passphrase)
- Public key from memory
- SSH agent authentication

### Command Execution Tests
- Basic command execution
- Command with exit codes
- Command with stderr output
- Commands with PTY allocation
- Streaming command stdout/stderr
- Streaming to file
- Async streaming
- Incremental output streaming

### File Transfer Tests
- SCP upload (small, medium, large files)
- SCP download (small, medium, large files)
- File transfer error handling

### Terminal Features Tests
- PTY allocation
- Terminal modes configuration
- Window size configuration

### Error Handling Tests
- Authentication failures
- Connection timeouts
- Invalid commands
- File transfer errors

### Edge Case Tests
- Multiple sequential operations
- Timeout handling
- Large output handling
- Connection with deprecated methods
- Parallel sessions (15 concurrent connections)

## Running Stress Tests

For performance testing with 2,000 parallel connections:

<tabs>
<tab id="stress-linux" title="Linux/macOS">
<code-block lang="bash">
CRAZY_LOAD_TEST=1 dotnet run
</code-block>
</tab>
<tab id="stress-windows" title="Windows (PowerShell)">
<code-block lang="powershell">
$env:CRAZY_LOAD_TEST="1"
dotnet run
</code-block>
</tab>
<tab id="stress-windows-cmd" title="Windows (CMD)">
<code-block lang="batch">
set CRAZY_LOAD_TEST=1
dotnet run
</code-block>
</tab>
</tabs>

> **Note**: Stress tests require sufficient system resources. Recommended: 8+ CPU cores, 16GB+ RAM.

## Test Server Configuration

The Docker-based SSH test server is configured with:

- **Port**: 2222 (mapped to host)
- **Username**: `user`
- **Password**: `12345`
- **SSH Keys**: Located in `docker/test-keys/`
- **Test Files**: Pre-generated in `/test-files/` inside the container
  - `small.txt` - Small text file
  - `medium.bin` - 1MB binary file
  - `large.dat` - 10MB binary file

### Server Limits

The test server is configured to handle high connection loads:

- **MaxStartups**: 1000:30:2000
- **MaxSessions**: 1000
- **File descriptors**: 65536
- **Max processes**: 32768

## Troubleshooting

### Docker Container Not Starting

Check Docker daemon status:

```bash
docker ps
```

View container logs:

```bash
docker compose logs ssh-server
```

Rebuild the container if needed:

```bash
docker compose down
docker compose build --no-cache
docker compose up -d
```

### Connection Refused Errors

Ensure the SSH server is listening on port 2222:

```bash
docker compose exec ssh-server nc -z localhost 2222
```

If the connection fails, restart the container:

```bash
docker compose restart
```

### Tests Timing Out

The test runner waits up to 60 seconds for containers to be ready. If tests still time out:

1. Check Docker resource allocation (CPU/memory)
2. Verify no firewall is blocking port 2222
3. Manually test SSH connection:

```bash
ssh -p 2222 user@localhost
# Password: 12345
```

### Native Library Load Failures

If you see "Failed to load native library":

1. Ensure the NullOpsDevs.LibSsh package is properly restored
2. Check that your platform is supported (Windows x64, Linux x64/ARM64, macOS x64/ARM64)
3. Try cleaning and rebuilding:

```bash
dotnet clean
dotnet restore
dotnet build
dotnet run
```

The native libraries are automatically copied to the output directory (`bin/Debug/net9.0/native/`) during build from the NuGet package.

### Port 2222 Already in Use

If port 2222 is already in use:

```bash
# Find what's using the port
lsof -i :2222  # macOS/Linux
netstat -ano | findstr :2222  # Windows

# Stop the conflicting service or change the port in docker-compose.yml
```

## Cleaning Up

### Stop the Test Server

```bash
docker compose down
```

### Remove Test Container and Images

```bash
docker compose down --rmi all
```

### Full Cleanup (including volumes)

```bash
docker compose down --rmi all --volumes
```

## CI/CD Integration

The test suite is designed to run in CI/CD environments. See `.github/workflows/` for GitHub Actions examples.

### Environment Variables

- `CRAZY_LOAD_TEST` - Set to `1` to enable stress testing with 2,000 parallel connections

### Example GitHub Actions Workflow

```yaml
- name: Start SSH Test Server
  run: |
    cd src/NullOpsDevs.LibSsh.Test
    docker compose up -d

- name: Run Tests
  run: |
    cd src/NullOpsDevs.LibSsh.Test
    dotnet run

- name: Cleanup
  if: always()
  run: |
    cd src/NullOpsDevs.LibSsh.Test
    docker compose down
```

## Test Output

Successful test runs will display colorful output using Spectre.Console:

```
  _     _ _   ____ ____  _   _ ____    _____         _
 | |   (_) |_/ ___/ ___|| | | |___ \  |_   _|__  ___| |_ ___
 | |   | | __\___ \___ \| |_| | __) |   | |/ _ \/ __| __/ __|
 | |___| | |_ ___) |__) |  _  |/ __/    | |  __/\__ \ |_\__ \
 |_____|_|\__|____/____/|_| |_|_____|   |_|\___||___/\__|___/

✓ Passed: 25
✗ Failed: 0
- Skipped: 0
```

## See Also

- [Performance and Reliability](performance-and-reliability.md) - Stress test results and memory profiling
- [Session Lifecycle](session-lifecycle.md) - Understanding SSH session states
- [Error Handling](error-handling.md) - Handling SSH errors in your applications
