using NullOpsDevs.LibSsh.Core;
using NullOpsDevs.LibSsh.Exceptions;

namespace NullOpsDevs.LibSsh.Extensions;

/// <summary>
/// Internal extension methods for libssh2 operations.
/// </summary>
internal static class LibSshExtensions
{
    /// <summary>
    /// Throws an <see cref="SshException"/> if the libssh2 return code indicates failure (negative value).
    /// </summary>
    /// <param name="return">The libssh2 function return code.</param>
    /// <param name="session">Libssh2 session.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <param name="also">Optional action to execute before throwing the exception (e.g., cleanup).</param>
    /// <exception cref="SshException">Thrown when the return code is negative (indicates error).</exception>
    internal static unsafe void ThrowIfNotSuccessful(this int @return, SshSession session,
        string? message = null, Action? also = null)
    {
        if (@return >= 0)
            return;

        also?.Invoke();

        if (session.SessionPtr != null)
        {
            if (message != null)
                throw SshException.FromLastSessionError(session.SessionPtr, message);
            
            throw SshException.FromLastSessionError(session.SessionPtr);
        }

        throw new SshException(message ?? "Unknown error", (SshError)@return);
    }

    /// <summary>
    /// Converts a standard exception to an <see cref="SshException"/> with the InnerException error code.
    /// </summary>
    /// <param name="exception">The exception to convert.</param>
    /// <returns>An SshException wrapping the original exception.</returns>
    internal static SshException AsSshException(this Exception exception)
    {
        return new SshException(exception.Message, SshError.InnerException, exception);
    }
}