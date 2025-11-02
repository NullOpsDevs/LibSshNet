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
using NullOpsDevs.LibSsh.Core;
using NullOpsDevs.LibSsh.Credentials;

var ssh = new SshSession();
ssh.Connect("localhost", 2222);
ssh.Authenticate(SshCredential.FromPassword("user", "12345"));

Console.WriteLine(ssh.ExecuteCommand("ls").Stdout);
```
