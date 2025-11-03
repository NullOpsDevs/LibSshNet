using System.Reflection;
using System.Runtime.InteropServices;
using NullOpsDevs.LibSsh.Generated;
using Spectre.Console;

namespace NullOpsDevs.LibSsh.Test;

public static class NativePreloader
{
    private static IntPtr _libraryHandle;
    private static string? _libraryPath;

    public static bool Preload()
    {
        // Get the directory where the test executable is located
        var executableDir = AppContext.BaseDirectory;
        var nativeDir = Path.Combine(executableDir, "native");

        if (!Directory.Exists(nativeDir))
        {
            AnsiConsole.MarkupLine($"[red]Folder 'native' was not found at: {nativeDir}[/]");
            return false;
        }

        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        var isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        var processArchitecture = RuntimeInformation.ProcessArchitecture;
        var isx64 = processArchitecture == Architecture.X64;
        var isArm64 = processArchitecture == Architecture.Arm64;

        if (isWindows && isx64)
            _libraryPath = Path.Combine(nativeDir, "libssh2-win-x64", "libssh2.dll");
        else if (isLinux && isx64)
            _libraryPath = Path.Combine(nativeDir, "libssh2-linux-x64", "libssh2.so");
        else if (isLinux && isArm64)
            _libraryPath = Path.Combine(nativeDir, "libssh2-linux-arm64", "libssh2.so");
        else if (isMac && isx64)
            _libraryPath = Path.Combine(nativeDir, "libssh2-osx-x64", "libssh2.dylib");
        else if (isMac && isArm64)
            _libraryPath = Path.Combine(nativeDir, "libssh2-osx-arm64", "libssh2.dylib");

        if (_libraryPath == null)
        {
            AnsiConsole.MarkupLine("[red]Unsupported platform/architecture combination.[/]");
            return false;
        }

        if (!File.Exists(_libraryPath))
        {
            AnsiConsole.MarkupLine($"[red]Native library not found at: {_libraryPath}[/]");
            return false;
        }

        try
        {
            // Load the library
            _libraryHandle = NativeLibrary.Load(_libraryPath);

            // Set up the DllImport resolver for the LibSSH2 assembly
            var libssh2Assembly = typeof(LibSshNative).Assembly;
            NativeLibrary.SetDllImportResolver(libssh2Assembly, DllImportResolver);

            AnsiConsole.MarkupLine($"[green]Successfully loaded native library from: {_libraryPath}[/]");
            return _libraryHandle != IntPtr.Zero;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to load native library: {ex.Message}[/]");
            return false;
        }
    }

    private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        // If the library being imported is "libssh2", return our pre-loaded handle
        if (libraryName == "libssh2" && _libraryHandle != IntPtr.Zero)
        {
            return _libraryHandle;
        }

        // Otherwise, use default resolution
        return IntPtr.Zero;
    }
}