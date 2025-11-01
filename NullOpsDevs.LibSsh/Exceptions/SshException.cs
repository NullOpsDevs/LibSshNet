﻿using System.Runtime.InteropServices;
using NullOpsDevs.LibSsh.Core;
using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh.Exceptions;

/// <summary>
/// Exception thrown when an SSH operation fails.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
/// <param name="error">The SSH error code associated with this exception.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or null if no inner exception is specified.</param>
public class SshException(string message, SshError error, Exception? innerException = null) : Exception(message, innerException)
{
    /// <summary>
    /// Gets the SSH error code associated with this exception.
    /// </summary>
    public SshError Error { get; } = error;

    public static unsafe SshException FromLastSessionError(_LIBSSH2_SESSION* session)
    {
        sbyte* errorMsg = null;
        var errorMsgLen = 0;
        var errorCode = LibSshNative.libssh2_session_last_error(session, &errorMsg, &errorMsgLen, 0);
        
        var errorText = errorMsg != null 
            ? Marshal.PtrToStringAnsi((IntPtr)errorMsg, errorMsgLen) 
            : "Unknown error";
        
        return new SshException(
            $"Failed to create SCP channel: [{(SshError)errorCode:G}] {errorText}", 
            (SshError) errorCode);
    }
}
