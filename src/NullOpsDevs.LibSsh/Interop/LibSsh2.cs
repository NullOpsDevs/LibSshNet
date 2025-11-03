using JetBrains.Annotations;
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

    /// <summary>
    /// Performs global deinitialization of the libssh2 library and frees all internal memory.
    /// </summary>
    /// <remarks>
    /// <para><strong>WARNING: Only call this method at application exit. Never interact with the library again after calling this method.</strong></para>
    /// <para>This is a global shutdown operation that frees all internal libssh2 resources including cryptographic library state.</para>
    /// <para>After calling this method, the library is in an uninitialized state and cannot be used again without restarting the application.</para>
    /// <para>Do not call this while any <see cref="SshSession"/> instances are still in use or may be created in the future.</para>
    /// </remarks>
    public static void Exit()
    {
        LibSshNative.libssh2_exit();
    }
}
