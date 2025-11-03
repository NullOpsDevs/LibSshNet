# Quickstart

### 1. Install NuGet package

<tabs>
    <tab id="dotnet-CLI-install" title="dotnet CLI">
        <code-block lang="shell">
            dotnet add NullOpsDevs.LibSsh
        </code-block>
    </tab>
    <tab id="csproj-install" title=".csproj">
        <code-block lang="xml">
            &lt;ItemGroup&gt;
              &lt;PackageReference Include=&quot;NullOpsDevs.LibSsh&quot; Version=&quot;&lt;...&gt;&quot; /&gt;
            &lt;/ItemGroup&gt;
        </code-block>
    </tab>
</tabs>

### 2. Connect to the server and execute some commands!

```c#
using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Credentials;

var ssh = new SshSession();
ssh.Connect("localhost", 2222);
ssh.Authenticate(SshCredential.FromPassword("user", "12345"));

Console.WriteLine(ssh.ExecuteCommand("ls").Stdout);
```

## Next Steps

- [Session Lifecycle](session-lifecycle.md) - Understand how sessions progress through states
- [Authentication](authentication.md) - Learn about all authentication methods
- [Command Execution](command-execution.md) - Execute commands with PTY support
- [File Transfer with SCP](scp.md) - Upload and download files
- [Error Handling](error-handling.md) - Handle SSH errors properly
- [Algorithm and Method Preferences](algorithm-preferences.md) - Configure security settings
