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
    public static readonly Lock GlobalLock = new();

    /// <summary>
    /// Gets or sets the global logger action for libssh2 operations.
    /// </summary>
    public static Action<string>? GlobalLogger;

    /// <summary>
    /// Logs a message using the configured <see cref="GlobalLogger"/> if available.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void Log(string message)
    {
        GlobalLogger?.Invoke(message);
    }
}
