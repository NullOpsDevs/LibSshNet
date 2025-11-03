# File Transfer with SCP

SCP (Secure Copy Protocol) allows you to securely transfer files between your local system and a remote SSH server. NullOpsDevs.LibSsh provides simple methods for both uploading and downloading files using the SCP protocol.

## Downloading Files

To download a file from the remote server to your local system:

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Credentials;

var session = new SshSession();
session.Connect("example.com", 22);
session.Authenticate(SshCredential.FromPassword("user", "password"));

// Download a file
using var fileStream = File.Create("local-file.txt");
bool success = session.ReadFile("/remote/path/file.txt", fileStream);

if (success)
{
    Console.WriteLine("File downloaded successfully!");
}
```

### Download with Custom Buffer Size

You can specify a custom buffer size for better performance with large files:

```c#
using var fileStream = File.Create("large-file.bin");

// Use 64KB buffer instead of default 32KB
bool success = session.ReadFile(
    path: "/remote/path/large-file.bin",
    destination: fileStream,
    bufferSize: 65536
);
```

### Async Download

For non-blocking file downloads:

```c#
using var fileStream = File.Create("local-file.txt");

bool success = await session.ReadFileAsync(
    "/remote/path/file.txt",
    fileStream,
    cancellationToken: cancellationToken
);
```

## Uploading Files

To upload a file from your local system to the remote server:

```c#
using var fileStream = File.OpenRead("local-file.txt");

bool success = session.WriteFile(
    path: "/remote/path/file.txt",
    source: fileStream
);

if (success)
{
    Console.WriteLine("File uploaded successfully!");
}
```

### Upload with Unix Permissions

You can specify Unix file permissions when uploading:

```c#
using var fileStream = File.OpenRead("script.sh");

// Upload with rwxr-xr-x permissions (755 in octal, 493 in decimal)
bool success = session.WriteFile(
    path: "/remote/path/script.sh",
    source: fileStream,
    mode: 493  // 0755 in octal
);
```

Common Unix permission modes:

| Permissions | Octal | Decimal | Description |
|-------------|-------|---------|-------------|
| `rw-r--r--` | 0644 | 420 | Default file (owner read/write, others read) |
| `rwxr-xr-x` | 0755 | 493 | Executable file (owner full, others read/execute) |
| `rw-------` | 0600 | 384 | Private file (owner read/write only) |
| `rwxrwxrwx` | 0777 | 511 | Full permissions (not recommended) |

### Upload with Custom Buffer Size

```c#
using var fileStream = File.OpenRead("large-file.bin");

// Use 128KB buffer for better performance
bool success = session.WriteFile(
    path: "/remote/path/large-file.bin",
    source: fileStream,
    mode: 420,  // rw-r--r--
    bufferSize: 131072
);
```

### Async Upload

For non-blocking file uploads:

```c#
using var fileStream = File.OpenRead("local-file.txt");

bool success = await session.WriteFileAsync(
    path: "/remote/path/file.txt",
    source: fileStream,
    mode: 420,
    cancellationToken: cancellationToken
);
```

## Complete Example

Here's a complete example showing both upload and download:

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Credentials;

var session = new SshSession();

try
{
    // Connect and authenticate
    session.Connect("example.com", 22);
    session.Authenticate(SshCredential.FromPublicKeyFile(
        "username",
        "~/.ssh/id_ed25519.pub",
        "~/.ssh/id_ed25519"
    ));

    // Upload a file
    Console.WriteLine("Uploading file...");
    using (var uploadStream = File.OpenRead("local-data.txt"))
    {
        bool uploaded = session.WriteFile(
            "/home/user/data.txt",
            uploadStream,
            mode: 420  // rw-r--r--
        );

        if (uploaded)
            Console.WriteLine("Upload successful!");
        else
            Console.WriteLine("Upload failed!");
    }

    // Download a file
    Console.WriteLine("Downloading file...");
    using (var downloadStream = File.Create("downloaded-backup.tar.gz"))
    {
        bool downloaded = session.ReadFile(
            "/home/user/backup.tar.gz",
            downloadStream
        );

        if (downloaded)
            Console.WriteLine("Download successful!");
        else
            Console.WriteLine("Download failed!");
    }
}
finally
{
    session.Dispose();
}
```

## Important Notes

1. **Stream Management**:
   - The `ReadFile()` and `WriteFile()` methods do NOT close the streams
   - You are responsible for disposing streams (use `using` statements)

2. **Stream Requirements**:
   - Upload streams must be readable and seekable
   - Download streams must be writable
   - File size is determined from stream length for uploads

3. **Return Value**:
   - Both methods return `true` if the entire file was transferred successfully
   - Returns `false` if the transfer was incomplete

4. **Session State**:
   - The session must be in `LoggedIn` state (authenticated) before transferring files

5. **File Paths**:
   - Use absolute paths on the remote server
   - Path interpretation depends on the user's home directory and permissions

## Error Handling

Always handle potential errors when transferring files:

```c#
try
{
    using var fileStream = File.OpenRead("local-file.txt");
    bool success = session.WriteFile("/remote/path/file.txt", fileStream);

    if (!success)
    {
        Console.WriteLine("File transfer incomplete!");
    }
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"Local file not found: {ex.Message}");
}
catch (SshException ex)
{
    Console.WriteLine($"SSH error during transfer: {ex.Message}");
}
catch (IOException ex)
{
    Console.WriteLine($"I/O error: {ex.Message}");
}
```

## Performance Tips

1. **Buffer Size**:
   - Default buffer size is 32KB
   - For large files, consider 64KB or 128KB buffers
   - Don't use excessively large buffers (>1MB) - diminishing returns

2. **Network Conditions**:
   - Larger buffers help on high-latency networks
   - Smaller buffers may be better on unstable connections

3. **File Size**:
   - SCP is efficient for single file transfers
   - For multiple small files, consider archiving them first (tar/zip)

4. **Async Methods**:
   - Use async methods to avoid blocking the UI thread
   - Helpful for responsive applications during large transfers

## See Also

- `SshSession.ReadFile()` (SshSession.cs:555) - Download files via SCP
- `SshSession.WriteFile()` (SshSession.cs:636) - Upload files via SCP
- `SshSession.ReadFileAsync()` (SshSession.cs:614) - Async download
- `SshSession.WriteFileAsync()` (SshSession.cs:707) - Async upload
- [Authentication](authentication.md) - Authenticate before file transfers
- [Session Timeouts](session-timeouts.md) - Set timeouts for large file transfers
- [Session Lifecycle](session-lifecycle.md) - Understanding session states
- [Error Handling](error-handling.md) - Handle file transfer errors
