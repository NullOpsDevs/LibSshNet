using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh.Interop;

/// <summary>
/// Provides global utilities and configuration for the libssh2 library.
/// </summary>
[PublicAPI]
public static class LibSsh2
{
    /// <summary>
    /// Gets the version of the libssh2 library.
    /// </summary>
    public static string Version => $"{LibSshNative.LIBSSH2_VERSION_MAJOR}.{LibSshNative.LIBSSH2_VERSION_MINOR}.{LibSshNative.LIBSSH2_VERSION_PATCH}";

    /// <summary>
    /// Gets the global lock used for thread-safe library initialization.
    /// </summary>
#if NET9_0_OR_GREATER
    public static readonly Lock GlobalLock = new();
#else
    public static readonly object GlobalLock = new();
#endif
}
