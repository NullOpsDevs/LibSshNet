using JetBrains.Annotations;
using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh.Core;

/// <summary>
/// Specifies the trace levels for libssh2 debugging output.
/// These values can be combined using bitwise OR to enable multiple trace categories.
/// </summary>
[Flags]
[PublicAPI]
public enum SshTraceLevel
{
    /// <summary>
    /// No tracing enabled.
    /// </summary>
    None = 0,

    /// <summary>
    /// Trace transport layer operations.
    /// </summary>
    Transport = LibSshNative.LIBSSH2_TRACE_TRANS,

    /// <summary>
    /// Trace key exchange operations.
    /// </summary>
    KeyExchange = LibSshNative.LIBSSH2_TRACE_KEX,

    /// <summary>
    /// Trace authentication operations.
    /// </summary>
    Authentication = LibSshNative.LIBSSH2_TRACE_AUTH,

    /// <summary>
    /// Trace connection layer operations.
    /// </summary>
    Connection = LibSshNative.LIBSSH2_TRACE_CONN,

    /// <summary>
    /// Trace SCP operations.
    /// </summary>
    Scp = LibSshNative.LIBSSH2_TRACE_SCP,

    /// <summary>
    /// Trace SFTP operations.
    /// </summary>
    Sftp = LibSshNative.LIBSSH2_TRACE_SFTP,

    /// <summary>
    /// Trace error messages.
    /// </summary>
    Error = LibSshNative.LIBSSH2_TRACE_ERROR,

    /// <summary>
    /// Trace public key operations.
    /// </summary>
    PublicKey = LibSshNative.LIBSSH2_TRACE_PUBLICKEY,

    /// <summary>
    /// Trace socket operations.
    /// </summary>
    Socket = LibSshNative.LIBSSH2_TRACE_SOCKET,

    /// <summary>
    /// Enable all trace levels.
    /// </summary>
    All = Transport | KeyExchange | Authentication | Connection | Scp | Sftp | Error | PublicKey | Socket
}
