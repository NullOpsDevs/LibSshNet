using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh.Core;

/// <summary>
/// Represents SSH error codes from libssh2 and custom error codes.
/// </summary>
[PublicAPI]
[SuppressMessage("Design", "CA1069:Enums values should not be duplicated")]
public enum SshError
{
    /// <summary>
    /// No error occurred; the operation completed successfully.
    /// </summary>
    None = LibSshNative.LIBSSH2_ERROR_NONE,

    /// <summary>
    /// No socket has been established for the SSH session.
    /// </summary>
    SocketNone = LibSshNative.LIBSSH2_ERROR_SOCKET_NONE,

    /// <summary>
    /// Failed to receive the SSH protocol banner from the server.
    /// </summary>
    BannerRecv = LibSshNative.LIBSSH2_ERROR_BANNER_RECV,

    /// <summary>
    /// Failed to send the SSH protocol banner to the server.
    /// </summary>
    BannerSend = LibSshNative.LIBSSH2_ERROR_BANNER_SEND,

    /// <summary>
    /// No valid SSH banner was exchanged during the connection handshake.
    /// </summary>
    BannerNone = LibSshNative.LIBSSH2_ERROR_BANNER_NONE,

    /// <summary>
    /// Message authentication code (MAC) verification failed; data may be corrupted or tampered with.
    /// </summary>
    InvalidMac = LibSshNative.LIBSSH2_ERROR_INVALID_MAC,

    /// <summary>
    /// SSH key exchange (KEX) negotiation failed.
    /// </summary>
    KexFailure = LibSshNative.LIBSSH2_ERROR_KEX_FAILURE,

    /// <summary>
    /// Memory allocation failed.
    /// </summary>
    Alloc = LibSshNative.LIBSSH2_ERROR_ALLOC,

    /// <summary>
    /// Failed to send data over the socket.
    /// </summary>
    SocketSend = LibSshNative.LIBSSH2_ERROR_SOCKET_SEND,

    /// <summary>
    /// SSH key exchange process failed.
    /// </summary>
    KeyExchangeFailure = LibSshNative.LIBSSH2_ERROR_KEY_EXCHANGE_FAILURE,

    /// <summary>
    /// Operation timed out.
    /// </summary>
    Timeout = LibSshNative.LIBSSH2_ERROR_TIMEOUT,

    /// <summary>
    /// Failed to initialize the host key during key exchange.
    /// </summary>
    HostkeyInit = LibSshNative.LIBSSH2_ERROR_HOSTKEY_INIT,

    /// <summary>
    /// Failed to sign data with the host key.
    /// </summary>
    HostkeySign = LibSshNative.LIBSSH2_ERROR_HOSTKEY_SIGN,

    /// <summary>
    /// Failed to decrypt incoming data.
    /// </summary>
    Decrypt = LibSshNative.LIBSSH2_ERROR_DECRYPT,

    /// <summary>
    /// The socket was disconnected during operation.
    /// </summary>
    SocketDisconnect = LibSshNative.LIBSSH2_ERROR_SOCKET_DISCONNECT,

    /// <summary>
    /// SSH protocol error occurred.
    /// </summary>
    Proto = LibSshNative.LIBSSH2_ERROR_PROTO,

    /// <summary>
    /// The user's password has expired and must be changed.
    /// </summary>
    PasswordExpired = LibSshNative.LIBSSH2_ERROR_PASSWORD_EXPIRED,

    /// <summary>
    /// File operation failed.
    /// </summary>
    File = LibSshNative.LIBSSH2_ERROR_FILE,

    /// <summary>
    /// No suitable authentication method is available.
    /// </summary>
    MethodNone = LibSshNative.LIBSSH2_ERROR_METHOD_NONE,

    /// <summary>
    /// Authentication failed; invalid credentials or method not accepted by server.
    /// </summary>
    AuthenticationFailed = LibSshNative.LIBSSH2_ERROR_AUTHENTICATION_FAILED,

    /// <summary>
    /// Public key authentication failed; the provided public key was not verified by the server.
    /// </summary>
    PublickeyUnverified = LibSshNative.LIBSSH2_ERROR_PUBLICKEY_UNVERIFIED,

    /// <summary>
    /// SSH channel packets received out of order.
    /// </summary>
    ChannelOutoforder = LibSshNative.LIBSSH2_ERROR_CHANNEL_OUTOFORDER,

    /// <summary>
    /// General SSH channel failure.
    /// </summary>
    ChannelFailure = LibSshNative.LIBSSH2_ERROR_CHANNEL_FAILURE,

    /// <summary>
    /// SSH channel request was denied by the server.
    /// </summary>
    ChannelRequestDenied = LibSshNative.LIBSSH2_ERROR_CHANNEL_REQUEST_DENIED,

    /// <summary>
    /// Received data for an unknown or invalid SSH channel.
    /// </summary>
    ChannelUnknown = LibSshNative.LIBSSH2_ERROR_CHANNEL_UNKNOWN,

    /// <summary>
    /// SSH channel window size was exceeded.
    /// </summary>
    ChannelWindowExceeded = LibSshNative.LIBSSH2_ERROR_CHANNEL_WINDOW_EXCEEDED,

    /// <summary>
    /// SSH channel packet size was exceeded.
    /// </summary>
    ChannelPacketExceeded = LibSshNative.LIBSSH2_ERROR_CHANNEL_PACKET_EXCEEDED,

    /// <summary>
    /// SSH channel has been closed.
    /// </summary>
    ChannelClosed = LibSshNative.LIBSSH2_ERROR_CHANNEL_CLOSED,

    /// <summary>
    /// End-of-file (EOF) has already been sent on this SSH channel.
    /// </summary>
    ChannelEofSent = LibSshNative.LIBSSH2_ERROR_CHANNEL_EOF_SENT,

    /// <summary>
    /// SCP protocol error occurred during file transfer.
    /// </summary>
    ScpProtocol = LibSshNative.LIBSSH2_ERROR_SCP_PROTOCOL,

    /// <summary>
    /// Compression or decompression error (zlib).
    /// </summary>
    Zlib = LibSshNative.LIBSSH2_ERROR_ZLIB,

    /// <summary>
    /// Socket operation timed out.
    /// </summary>
    SocketTimeout = LibSshNative.LIBSSH2_ERROR_SOCKET_TIMEOUT,

    /// <summary>
    /// SFTP protocol error occurred.
    /// </summary>
    SftpProtocol = LibSshNative.LIBSSH2_ERROR_SFTP_PROTOCOL,

    /// <summary>
    /// Request was denied by the server.
    /// </summary>
    RequestDenied = LibSshNative.LIBSSH2_ERROR_REQUEST_DENIED,

    /// <summary>
    /// The requested method or algorithm is not supported by the server.
    /// </summary>
    MethodNotSupported = LibSshNative.LIBSSH2_ERROR_METHOD_NOT_SUPPORTED,

    /// <summary>
    /// Invalid argument or parameter provided.
    /// </summary>
    Inval = LibSshNative.LIBSSH2_ERROR_INVAL,

    /// <summary>
    /// Invalid polling type specified for non-blocking operations.
    /// </summary>
    InvalidPollType = LibSshNative.LIBSSH2_ERROR_INVALID_POLL_TYPE,

    /// <summary>
    /// Public key subsystem protocol error.
    /// </summary>
    PublickeyProtocol = LibSshNative.LIBSSH2_ERROR_PUBLICKEY_PROTOCOL,

    /// <summary>
    /// Operation would block; try again later (non-blocking mode only).
    /// </summary>
    Eagain = LibSshNative.LIBSSH2_ERROR_EAGAIN,

    /// <summary>
    /// The provided buffer is too small to hold the requested data.
    /// </summary>
    BufferTooSmall = LibSshNative.LIBSSH2_ERROR_BUFFER_TOO_SMALL,

    /// <summary>
    /// Incorrect usage of the libssh2 API; check function parameters and state requirements.
    /// </summary>
    BadUse = LibSshNative.LIBSSH2_ERROR_BAD_USE,

    /// <summary>
    /// Data compression failed.
    /// </summary>
    Compress = LibSshNative.LIBSSH2_ERROR_COMPRESS,

    /// <summary>
    /// An out-of-boundary access was attempted.
    /// </summary>
    OutOfBoundary = LibSshNative.LIBSSH2_ERROR_OUT_OF_BOUNDARY,

    /// <summary>
    /// SSH agent protocol error occurred.
    /// </summary>
    AgentProtocol = LibSshNative.LIBSSH2_ERROR_AGENT_PROTOCOL,

    /// <summary>
    /// Failed to receive data over the socket.
    /// </summary>
    SocketRecv = LibSshNative.LIBSSH2_ERROR_SOCKET_RECV,

    /// <summary>
    /// Failed to encrypt outgoing data.
    /// </summary>
    Encrypt = LibSshNative.LIBSSH2_ERROR_ENCRYPT,

    /// <summary>
    /// Invalid or bad socket descriptor.
    /// </summary>
    BadSocket = LibSshNative.LIBSSH2_ERROR_BAD_SOCKET,

    /// <summary>
    /// Known hosts file operation failed.
    /// </summary>
    KnownHosts = LibSshNative.LIBSSH2_ERROR_KNOWN_HOSTS,

    /// <summary>
    /// SSH channel window is full; cannot send more data until window is adjusted.
    /// </summary>
    ChannelWindowFull = LibSshNative.LIBSSH2_ERROR_CHANNEL_WINDOW_FULL,

    /// <summary>
    /// Authentication using a key file failed.
    /// </summary>
    KeyfileAuthFailed = LibSshNative.LIBSSH2_ERROR_KEYFILE_AUTH_FAILED,

    /// <summary>
    /// Random number generation failed.
    /// </summary>
    Randgen = LibSshNative.LIBSSH2_ERROR_RANDGEN,

    /// <summary>
    /// Expected user authentication banner was not received from the server.
    /// </summary>
    MissingUserauthBanner = LibSshNative.LIBSSH2_ERROR_MISSING_USERAUTH_BANNER,

    /// <summary>
    /// The requested algorithm is not supported.
    /// </summary>
    AlgoUnsupported = LibSshNative.LIBSSH2_ERROR_ALGO_UNSUPPORTED,

    /// <summary>
    /// Message authentication code (MAC) operation failed.
    /// </summary>
    MacFailure = LibSshNative.LIBSSH2_ERROR_MAC_FAILURE,

    /// <summary>
    /// Hash function initialization failed.
    /// </summary>
    HashInit = LibSshNative.LIBSSH2_ERROR_HASH_INIT,

    /// <summary>
    /// Hash calculation failed.
    /// </summary>
    HashCalc = LibSshNative.LIBSSH2_ERROR_HASH_CALC,

    /// <summary>
    /// Custom error: Failed to initialize the SSH session.
    /// </summary>
    FailedToInitializeSession = int.MaxValue - 2,

    /// <summary>
    /// Custom error: An inner exception was thrown; check the exception's InnerException property for details.
    /// </summary>
    InnerException = int.MaxValue - 1,

    /// <summary>
    /// Custom error: Library was used incorrectly by the developer; review API usage and session state.
    /// </summary>
    DevWrongUse = int.MaxValue
}
