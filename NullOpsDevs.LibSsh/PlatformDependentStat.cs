using System.Runtime.InteropServices;
using NullOpsDevs.LibSsh.Exceptions;

namespace NullOpsDevs.LibSsh;

/// <summary>
/// Platform-agnostic wrapper for file stat structures across different operating systems.
/// </summary>
public readonly struct PlatformInDependentStat
{
    /// <summary>
    /// Gets the size of the file in bytes.
    /// </summary>
    public long FileSize { get; init; }

    /// <summary>
    /// Creates a <see cref="PlatformInDependentStat"/> instance from a platform-specific stat structure.
    /// </summary>
    /// <param name="structure">Pointer to the platform-specific stat structure.</param>
    /// <returns>A platform-agnostic <see cref="PlatformInDependentStat"/> instance.</returns>
    /// <exception cref="SshException">Thrown when an internal exception occurs during conversion.</exception>
    /// <exception cref="NotSupportedException">Thrown when the current operating system is not supported.</exception>
    public static unsafe PlatformInDependentStat From(void* structure)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return CreateFromUnixStruct(structure);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return CreateFromWindowsMingwStruct(structure);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return CreateFromMacOsStruct(structure);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                return CreateFromFreeBsdStruct(structure);
        }
        catch (Exception ex)
        {
            throw new SshException("Internal exception occured", SshError.InnerException, ex);
        }
        
        throw new NotSupportedException("Your OS is not supported by this library.");
    }

    /// <summary>
    /// Creates a <see cref="PlatformInDependentStat"/> from a Windows MinGW64 stat structure.
    /// </summary>
    /// <param name="structure">Pointer to the Windows MinGW64 stat structure.</param>
    /// <returns>A <see cref="PlatformInDependentStat"/> instance with data from the Windows structure.</returns>
    private static unsafe PlatformInDependentStat CreateFromWindowsMingwStruct(void* structure)
    {
        var stat = (StatMingw64*)structure;
        
        return new PlatformInDependentStat
        {
            FileSize = stat->st_size
        };
    }

    /// <summary>
    /// Creates a <see cref="PlatformInDependentStat"/> from a Linux x64 stat structure.
    /// </summary>
    /// <param name="structure">Pointer to the Linux x64 stat structure.</param>
    /// <returns>A <see cref="PlatformInDependentStat"/> instance with data from the Linux structure.</returns>
    private static unsafe PlatformInDependentStat CreateFromUnixStruct(void* structure)
    {
        var stat = (StatLinuxX64*)structure;
        
        return new PlatformInDependentStat
        {
            FileSize = stat->st_size
        };
    }

    /// <summary>
    /// Creates a <see cref="PlatformInDependentStat"/> from a macOS (Darwin) stat structure.
    /// </summary>
    /// <param name="structure">Pointer to the macOS stat structure.</param>
    /// <returns>A <see cref="PlatformInDependentStat"/> instance with data from the macOS structure.</returns>
    private static unsafe PlatformInDependentStat CreateFromMacOsStruct(void* structure)
    {
        var stat = (StatDarwin*)structure;
        
        return new PlatformInDependentStat
        {
            FileSize = stat->st_size
        };
    }

    /// <summary>
    /// Creates a <see cref="PlatformInDependentStat"/> from a FreeBSD stat structure.
    /// </summary>
    /// <param name="structure">Pointer to the FreeBSD stat structure.</param>
    /// <returns>A <see cref="PlatformInDependentStat"/> instance with data from the FreeBSD structure.</returns>
    private static unsafe PlatformInDependentStat CreateFromFreeBsdStruct(void* structure)
    {
        var stat = (StatFreebsd*)structure;
        
        return new PlatformInDependentStat
        {
            FileSize = stat->st_size
        };
    }
}