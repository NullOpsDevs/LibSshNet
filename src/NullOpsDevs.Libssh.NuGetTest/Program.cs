using NullOpsDevs.LibSsh;
using NullOpsDevs.LibSsh.Credentials;

var ssh = new SshSession();
ssh.Connect("localhost", 2222);
ssh.Authenticate(SshCredential.FromPassword("user", "12345"));

Console.WriteLine(ssh.ExecuteCommand("ls").Stdout);
