using NullOpsDevs.LibSsh.Exceptions;
using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh.Extensions;

internal static class LibSshExtensions
{
    public static void ThrowIfNotSuccessful(this int @return, string? message = null, Action? also = null)
    {
        if (@return >= 0)
            return;
        
        also?.Invoke();
        throw new SshException(message ?? "Unhandled exception", (SshError)@return);
    }

    public static SshException AsSshException(this Exception exception)
    {
        return new SshException(exception.Message, SshError.InnerException, exception);
    }
}