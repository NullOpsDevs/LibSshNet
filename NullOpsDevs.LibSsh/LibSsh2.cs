using JetBrains.Annotations;
using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh;

[PublicAPI]
public static class LibSsh2
{
    public static string Version => $"{LibSshNative.LIBSSH2_VERSION_MAJOR}.{LibSshNative.LIBSSH2_VERSION_MINOR}.{LibSshNative.LIBSSH2_VERSION_PATCH}";

    public static readonly Lock GlobalLock = new();

    public static Action<string>? GlobalLogger;

    public static void Log(string message)
    {
        GlobalLogger?.Invoke(message);
    }
}
