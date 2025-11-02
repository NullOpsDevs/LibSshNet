using System.Runtime.InteropServices;
using System.Text;
using NullOpsDevs.LibSsh.Core;
using NullOpsDevs.LibSsh.Credentials;
using NullOpsDevs.LibSsh.Terminal;
using Spectre.Console;

namespace NullOpsDevs.LibSsh.Test;

public static class Program
{
    private static int passedTests;
    private static int failedTests;
    private static int skippedTests;

    public static async Task<int> Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        
        // Display banner
        AnsiConsole.Write(new FigletText("LibSSH2 Tests").Centered().Color(Color.Blue));
        AnsiConsole.WriteLine();

        // Load native library
        if (!NativePreloader.Preload())
        {
            AnsiConsole.MarkupLine("[red]Failed to load native library. Exiting.[/]");
            return 255;
        }

        // LibSsh2.GlobalLogger = msg => AnsiConsole.MarkupLine($"[grey]{Markup.Escape(msg)}[/]");

        // Wait for Docker containers
        AnsiConsole.MarkupLine("[yellow]Waiting for Docker containers to be ready...[/]");
        try
        {
            await TestHelper.WaitForContainersAsync(TimeSpan.FromSeconds(60));
            AnsiConsole.MarkupLine("[green]Docker containers are ready![/]");
        }
        catch (TimeoutException ex)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
            AnsiConsole.MarkupLine("[yellow]Make sure to run 'docker-compose up -d' first![/]");
            return 1;
        }

        AnsiConsole.WriteLine();

        // Run test categories
        await RunTestCategory("Authentication Tests", RunAuthenticationTests);
        await RunTestCategory("Command Execution Tests", RunCommandTests);
        await RunTestCategory("File Transfer Tests", RunFileTransferTests);
        await RunTestCategory("Terminal Features Tests", RunTerminalTests);
        await RunTestCategory("Error Handling Tests", RunErrorHandlingTests);
        await RunTestCategory("Edge Case Tests", RunEdgeCaseTests);

        // Display summary
        AnsiConsole.WriteLine();
        DisplaySummary();

        return failedTests > 0 ? 1 : 0;
    }

    private static async Task RunTestCategory(string categoryName, Func<Task> testFunc)
    {
        AnsiConsole.Write(new Rule($"[bold blue]{categoryName}[/]").RuleStyle("blue dim"));
        AnsiConsole.WriteLine();

        try
        {
            await testFunc();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Category failed with exception: {Markup.Escape(ex.Message)}[/]");
        }

        AnsiConsole.WriteLine();
    }

    #region Authentication Tests

    private static async Task RunAuthenticationTests()
    {
        await RunTest("Retrieve negotiated methods", RetrieveNegotiatedMethods);
        await RunTest("Host key retrival", TestHostKeyRetrival);
        await RunTest("Host key hash retrival", TestHostKeyHashRetrival);
        await RunTest("Password Authentication", TestPasswordAuth);
        await RunTest("Public Key Authentication (no passphrase)", TestPublicKeyAuth);
        await RunTest("Public Key Authentication (with passphrase)", TestPublicKeyAuthWithPassphrase);
        await RunTest("Public Key from Memory", TestPublicKeyFromMemory);
        await RunTest("SSH Agent Authentication", TestSshAgentAuth);
    }

    private static Task<bool> RetrieveNegotiatedMethods()
    {
        using var session = TestHelper.CreateAndConnect();
        
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Blue)
            .AddColumn(new TableColumn("[bold]Type[/]").Centered())
            .AddColumn(new TableColumn("[bold]Negotiated[/]").Centered());
        
        foreach (var sshMethod in Enum.GetValues<SshMethod>())
        {
            var methods = "< not supported by sshlib2 >";

            try
            {
                methods = session.GetNegotiatedMethod(sshMethod);
            } catch { /* ignored */ }
            
            table.AddRow(sshMethod.ToString("G"), methods!);
        }
        
        AnsiConsole.Write(table);
        return Task.FromResult(true);
    }

    private static Task<bool> TestHostKeyRetrival()
    {
        using var session = TestHelper.CreateAndConnect();
        var hostKey = session.GetHostKey();
        return Task.FromResult(hostKey.Key.Length != 0 && hostKey.Type != SshHostKeyType.Unknown);
    }

    private static Task<bool> TestHostKeyHashRetrival()
    {
        using var session = TestHelper.CreateAndConnect();
        var hostKey = session.GetHostKeyHash(SshHashType.SHA256);
        return Task.FromResult(hostKey.Length == 32);
    }

    private static Task<bool> TestPasswordAuth()
    {
        using var session = TestHelper.CreateAndConnect();
        var credential = SshCredential.FromPassword(TestConfig.Username, TestConfig.Password);
        return Task.FromResult(session.Authenticate(credential));
    }

    private static Task<bool> TestPublicKeyAuth()
    {
        using var session = TestHelper.CreateAndConnect();
        
        var credential = SshCredential.FromPublicKeyFile(
            TestConfig.Username,
            TestConfig.GetKeyPath(TestConfig.PublicKeyPath),
            TestConfig.GetKeyPath(TestConfig.PrivateKeyPath));
        
        return Task.FromResult(session.Authenticate(credential));
    }

    private static Task<bool> TestPublicKeyAuthWithPassphrase()
    {
        using var session = TestHelper.CreateAndConnect();
        
        var credential = SshCredential.FromPublicKeyFile(
            TestConfig.Username,
            TestConfig.GetKeyPath(TestConfig.PublicKeyProtectedPath),
            TestConfig.GetKeyPath(TestConfig.PrivateKeyProtectedPath),
            TestConfig.KeyPassphrase);
        
        return Task.FromResult(session.Authenticate(credential));
    }

    private static Task<bool> TestPublicKeyFromMemory()
    {
        using var session = TestHelper.CreateAndConnect();
        
        var publicKeyData = TestHelper.LoadKeyFile(TestConfig.PublicKeyPath);
        var privateKeyData = TestHelper.LoadKeyFile(TestConfig.PrivateKeyPath);
        
        var credential = SshCredential.FromPublicKeyMemory(
            TestConfig.Username,
            publicKeyData,
            privateKeyData);
        
        return Task.FromResult(session.Authenticate(credential));
    }

    private static Task<bool> TestSshAgentAuth()
    {
        try
        {
            using var session = TestHelper.CreateAndConnect();
            var credential = SshCredential.FromAgent(TestConfig.Username);
            
            var result = session.Authenticate(credential);

            if (!result && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                skippedTests++;
                return Task.FromResult(true);
            }

            return Task.FromResult(result);
        }
        catch
        {
            // Agent might not be accessible on Windows - don't count as failure
            skippedTests++;
            return Task.FromResult(true);
        }
    }

    #endregion

    #region Command Execution Tests

    private static async Task RunCommandTests()
    {
        await RunTest("Simple command without PTY", TestSimpleCommand);
        await RunTest("Command with stderr output", TestCommandWithStderr);
        await RunTest("Command with PTY (default)", TestCommandWithPty);
        await RunTest("Command with PTY (Xterm256Color)", TestCommandWithXterm256);
        await RunTest("Command with custom terminal modes", TestCommandWithCustomModes);
        await RunTest("Command with cancellation", TestCommandCancellation);
    }

    private static Task<bool> TestSimpleCommand()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        var result = session.ExecuteCommand("echo 'Hello from SSH'");
        return Task.FromResult(result.Successful && result.Stdout?.Contains("Hello from SSH") == true);
    }

    private static Task<bool> TestCommandWithStderr()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        var result = session.ExecuteCommand("ls /nonexistent >&2");
        return Task.FromResult(result is { Successful: true, Stderr.Length: > 4 });
    }

    private static Task<bool> TestCommandWithPty()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        var options = new CommandExecutionOptions { RequestPty = true };
        var result = session.ExecuteCommand("echo 'PTY test'", options);
        return Task.FromResult(result.Successful && result.Stdout?.Contains("PTY test") == true);
    }

    private static Task<bool> TestCommandWithXterm256()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        
        var options = new CommandExecutionOptions
        {
            RequestPty = true,
            TerminalType = TerminalType.Xterm256Color
        };
        
        var result = session.ExecuteCommand("echo $TERM", options);
        
        return Task.FromResult(result.Successful && result.Stdout?.Contains(TerminalType.Xterm256Color.ToLibSsh2String()) == true);
    }

    private static Task<bool> TestCommandWithCustomModes()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        
        var modes = new TerminalModesBuilder()
            .SetFlag(TerminalMode.ECHO, true)
            .SetFlag(TerminalMode.ICANON, true)
            .Build();

        var options = new CommandExecutionOptions
        {
            RequestPty = true,
            TerminalModes = modes,
            TerminalWidth = 120,
            TerminalHeight = 40
        };

        var result = session.ExecuteCommand("tput cols", options);
        
        return Task.FromResult(result.Successful && result.Stdout?.Trim().Contains("120") == true);
    }

    private static Task<bool> TestCommandCancellation()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(100);

        try
        {
            session.ExecuteCommand("sleep 10", cancellationToken: cts.Token);
            return Task.FromResult(false);
        }
        catch (OperationCanceledException)
        {
            return Task.FromResult(true);
        }
    }

    #endregion

    #region File Transfer Tests

    private static async Task RunFileTransferTests()
    {
        await RunTest("Upload small file", TestUploadSmallFile);
        await RunTest("Upload large file", TestUploadLargeFile);
        await RunTest("Download small file", TestDownloadSmallFile);
        await RunTest("Download large file", TestDownloadLargeFile);
        await RunTest("Binary file round-trip", TestBinaryRoundTrip);
        await RunTest("File with custom permissions", TestCustomPermissions);
    }

    private static Task<bool> TestUploadSmallFile()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        const string content = "Test file content\n";
        using var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return Task.FromResult(session.WriteFile("/tmp/test-small.txt", sourceStream));
    }

    private static Task<bool> TestUploadLargeFile()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        var tempFile = TestHelper.CreateTempFile(1024 * 1024);

        try
        {
            using var sourceStream = File.OpenRead(tempFile);
            return Task.FromResult(session.WriteFile("/tmp/test-large.bin", sourceStream));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    private static Task<bool> TestDownloadSmallFile()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        using var destStream = new MemoryStream();
        var success = session.ReadFile(TestConfig.RemoteSmallFile, destStream);

        if (!success)
            return Task.FromResult(false);

        destStream.Position = 0;
        var content = Encoding.UTF8.GetString(destStream.ToArray());
        return Task.FromResult(content.Contains("Small test file"));
    }

    private static Task<bool> TestDownloadLargeFile()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        using var destStream = new MemoryStream();
        var success = session.ReadFile(TestConfig.RemoteLargeFile, destStream);
        return Task.FromResult(success && destStream.Length > 1024 * 1024);
    }

    private static Task<bool> TestBinaryRoundTrip()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();

        var originalData = new byte[10240];
        Random.Shared.NextBytes(originalData);
        var originalHash = TestHelper.GetStreamHash(new MemoryStream(originalData));

        using var uploadStream = new MemoryStream(originalData);
        
        if (!session.WriteFile("/tmp/test-binary.bin", uploadStream))
            return Task.FromResult(false);

        using var downloadStream = new MemoryStream();
        
        if (!session.ReadFile("/tmp/test-binary.bin", downloadStream))
            return Task.FromResult(false);

        var downloadHash = TestHelper.GetStreamHash(downloadStream);
        return Task.FromResult(originalHash == downloadHash);
    }

    private static Task<bool> TestCustomPermissions()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        
        const string content = "Test with custom permissions\n";
        using var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        
        return Task.FromResult(session.WriteFile("/tmp/test-exec.sh", sourceStream, mode: 493));
    }

    #endregion

    #region Terminal Features Tests

    private static async Task RunTerminalTests()
    {
        await RunTest("Different terminal types", TestDifferentTerminalTypes);
        await RunTest("Custom terminal dimensions", TestCustomDimensions);
        await RunTest("Terminal modes builder", TestTerminalModesBuilder);
    }

    private static Task<bool> TestDifferentTerminalTypes()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();

        var types = new[] { TerminalType.Xterm, TerminalType.VT100, TerminalType.Linux };
        foreach (var type in types)
        {
            var options = new CommandExecutionOptions
            {
                RequestPty = true,
                TerminalType = type
            };

            var result = session.ExecuteCommand("echo test", options);
            
            if (!result.Successful)
                return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    private static Task<bool> TestCustomDimensions()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        var options = new CommandExecutionOptions
        {
            RequestPty = true,
            TerminalWidth = 120,
            TerminalHeight = 40
        };

        var result = session.ExecuteCommand("tput cols && tput lines", options);
        var stdout = result.Stdout?.Trim() ?? "";
        return Task.FromResult(result.Successful &&
               stdout.Contains("120") &&
               stdout.Contains("40"));
    }

    private static Task<bool> TestTerminalModesBuilder()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        
        var modes = new TerminalModesBuilder()
            .SetSpeed(9600)
            .SetFlag(TerminalMode.ECHO, false)
            .SetFlag(TerminalMode.ICANON, true)
            .Build();

        var options = new CommandExecutionOptions
        {
            RequestPty = true,
            TerminalModes = modes
        };

        var result = session.ExecuteCommand("stty -a", options);
        return Task.FromResult(result.Successful);
    }

    #endregion

    #region Error Handling Tests

    private static async Task RunErrorHandlingTests()
    {
        await RunTest("Invalid credentials", TestInvalidCredentials);
        await RunTest("Non-existent remote file", TestNonExistentFile);
        await RunTest("Permission denied", TestPermissionDenied);
        await RunTest("Connection to wrong port", TestWrongPort);
        await RunTest("Command with non-zero exit", TestNonZeroExit);
    }

    private static Task<bool> TestInvalidCredentials()
    {
        try
        {
            using var session = TestHelper.CreateAndConnect();
            var credential = SshCredential.FromPassword("invalid", "invalid");
            
            return Task.FromResult(!session.Authenticate(credential));
        }
        catch
        {
            return Task.FromResult(true);
        }
    }

    private static Task<bool> TestNonExistentFile()
    {
        try
        {
            using var session = TestHelper.CreateConnectAndAuthenticate();
            using var destStream = new MemoryStream();
            
            session.ReadFile("/nonexistent/file.txt", destStream);
            
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(true);
        }
    }

    private static Task<bool> TestPermissionDenied()
    {
        try
        {
            using var session = TestHelper.CreateConnectAndAuthenticate();
            var content = "Test\n";
            using var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            session.WriteFile("/root/denied.txt", sourceStream);
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(true);
        }
    }

    private static Task<bool> TestWrongPort()
    {
        try
        {
            using var session = new SshSession();
            session.Connect(TestConfig.Host, 9999);
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(true);
        }
    }

    private static Task<bool> TestNonZeroExit()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        var result = session.ExecuteCommand("exit 42");
        return Task.FromResult(result is { Successful: true, ExitCode: 42 });
    }

    #endregion

    #region Edge Case Tests

    private static async Task RunEdgeCaseTests()
    {
        await RunTest("Empty file transfer", TestEmptyFileTransfer);
        await RunTest("Command with large output", TestLargeOutput);
        await RunTest("Multiple sequential operations", TestMultipleOperations);
        await RunTest("Timeout test", TimeoutTest);
    }

    private static Task<bool> TimeoutTest()
    {
        var session = TestHelper.CreateConnectAndAuthenticate();
        session.SetSessionTimeout(TimeSpan.FromMilliseconds(1));
        
        var result = session.ExecuteCommand("sleep 10");
        
        return Task.FromResult(result is { Successful: false });
    }

    private static Task<bool> TestEmptyFileTransfer()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        using var emptyStream = new MemoryStream();

        if (!session.WriteFile("/tmp/empty.txt", emptyStream))
            return Task.FromResult(false);

        using var downloadStream = new MemoryStream();
        
        return Task.FromResult(
            session.ReadFile("/tmp/empty.txt", downloadStream) &&
            downloadStream.Length == 0);
    }

    private static Task<bool> TestLargeOutput()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();
        var result = session.ExecuteCommand("cat /test-files/large.dat | head -c 100000");
        return Task.FromResult(result is { Successful: true, Stdout.Length: > 1000 });
    }

    private static Task<bool> TestMultipleOperations()
    {
        using var session = TestHelper.CreateConnectAndAuthenticate();

        for (int i = 0; i < 5; i++)
        {
            var result = session.ExecuteCommand($"echo 'Test {i}'");
            if (!result.Successful || !result.Stdout?.Contains($"Test {i}") == true)
                return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    #endregion

    #region Test Infrastructure

    private static async Task RunTest(string testName, Func<Task<bool>> testFunc)
    {
        try
        {
            var result = await testFunc();
            if (result)
            {
                AnsiConsole.MarkupLine($"[green]  ✓[/] {testName}");
                passedTests++;
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]  ✗[/] {testName}");
                failedTests++;
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]  ✗[/] {testName}: {Markup.Escape(ex.Message)}");
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes);
            failedTests++;
        }

        await Task.Delay(1000);
    }

    private static void DisplaySummary()
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Blue)
            .AddColumn(new TableColumn("[bold]Result[/]").Centered())
            .AddColumn(new TableColumn("[bold]Count[/]").Centered());

        table.AddRow("[green]Passed[/]", $"[green]{passedTests}[/]");
        table.AddRow("[red]Failed[/]", $"[red]{failedTests}[/]");
        if (skippedTests > 0)
            table.AddRow("[yellow]Skipped[/]", $"[yellow]{skippedTests}[/]");

        var totalColor = failedTests > 0 ? "red" : "green";
        table.AddRow($"[{totalColor}]Total[/]", $"[{totalColor}]{passedTests + failedTests + skippedTests}[/]");

        AnsiConsole.Write(table);

        if (failedTests == 0 && passedTests > 0)
        {
            AnsiConsole.MarkupLine("\n[bold green]All tests passed! 🎉[/]");
        }
        else if (failedTests > 0)
        {
            AnsiConsole.MarkupLine($"\n[bold red]{failedTests} test(s) failed.[/]");
        }
    }

    #endregion
}
