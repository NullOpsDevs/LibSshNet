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
    None = LibSshNative.LIBSSH2_ERROR_NONE,
    SocketNone = LibSshNative.LIBSSH2_ERROR_SOCKET_NONE,
    BannerRecv = LibSshNative.LIBSSH2_ERROR_BANNER_RECV,
    BannerSend = LibSshNative.LIBSSH2_ERROR_BANNER_SEND,
    BannerNone = LibSshNative.LIBSSH2_ERROR_BANNER_NONE,
    InvalidMac = LibSshNative.LIBSSH2_ERROR_INVALID_MAC,
    KexFailure = LibSshNative.LIBSSH2_ERROR_KEX_FAILURE,
    Alloc = LibSshNative.LIBSSH2_ERROR_ALLOC,
    SocketSend = LibSshNative.LIBSSH2_ERROR_SOCKET_SEND,
    KeyExchangeFailure = LibSshNative.LIBSSH2_ERROR_KEY_EXCHANGE_FAILURE,
    Timeout = LibSshNative.LIBSSH2_ERROR_TIMEOUT,
    HostkeyInit = LibSshNative.LIBSSH2_ERROR_HOSTKEY_INIT,
    HostkeySign = LibSshNative.LIBSSH2_ERROR_HOSTKEY_SIGN,
    Decrypt = LibSshNative.LIBSSH2_ERROR_DECRYPT,
    SocketDisconnect = LibSshNative.LIBSSH2_ERROR_SOCKET_DISCONNECT,
    Proto = LibSshNative.LIBSSH2_ERROR_PROTO,
    PasswordExpired = LibSshNative.LIBSSH2_ERROR_PASSWORD_EXPIRED,
    File = LibSshNative.LIBSSH2_ERROR_FILE,
    MethodNone = LibSshNative.LIBSSH2_ERROR_METHOD_NONE,
    AuthenticationFailed = LibSshNative.LIBSSH2_ERROR_AUTHENTICATION_FAILED,
    PublickeyUnverified = LibSshNative.LIBSSH2_ERROR_PUBLICKEY_UNVERIFIED,
    ChannelOutoforder = LibSshNative.LIBSSH2_ERROR_CHANNEL_OUTOFORDER,
    ChannelFailure = LibSshNative.LIBSSH2_ERROR_CHANNEL_FAILURE,
    ChannelRequestDenied = LibSshNative.LIBSSH2_ERROR_CHANNEL_REQUEST_DENIED,
    ChannelUnknown = LibSshNative.LIBSSH2_ERROR_CHANNEL_UNKNOWN,
    ChannelWindowExceeded = LibSshNative.LIBSSH2_ERROR_CHANNEL_WINDOW_EXCEEDED,
    ChannelPacketExceeded = LibSshNative.LIBSSH2_ERROR_CHANNEL_PACKET_EXCEEDED,
    ChannelClosed = LibSshNative.LIBSSH2_ERROR_CHANNEL_CLOSED,
    ChannelEofSent = LibSshNative.LIBSSH2_ERROR_CHANNEL_EOF_SENT,
    ScpProtocol = LibSshNative.LIBSSH2_ERROR_SCP_PROTOCOL,
    Zlib = LibSshNative.LIBSSH2_ERROR_ZLIB,
    SocketTimeout = LibSshNative.LIBSSH2_ERROR_SOCKET_TIMEOUT,
    SftpProtocol = LibSshNative.LIBSSH2_ERROR_SFTP_PROTOCOL,
    RequestDenied = LibSshNative.LIBSSH2_ERROR_REQUEST_DENIED,
    MethodNotSupported = LibSshNative.LIBSSH2_ERROR_METHOD_NOT_SUPPORTED,
    Inval = LibSshNative.LIBSSH2_ERROR_INVAL,
    InvalidPollType = LibSshNative.LIBSSH2_ERROR_INVALID_POLL_TYPE,
    PublickeyProtocol = LibSshNative.LIBSSH2_ERROR_PUBLICKEY_PROTOCOL,
    Eagain = LibSshNative.LIBSSH2_ERROR_EAGAIN,
    BufferTooSmall = LibSshNative.LIBSSH2_ERROR_BUFFER_TOO_SMALL,
    BadUse = LibSshNative.LIBSSH2_ERROR_BAD_USE,
    Compress = LibSshNative.LIBSSH2_ERROR_COMPRESS,
    OutOfBoundary = LibSshNative.LIBSSH2_ERROR_OUT_OF_BOUNDARY,
    AgentProtocol = LibSshNative.LIBSSH2_ERROR_AGENT_PROTOCOL,
    SocketRecv = LibSshNative.LIBSSH2_ERROR_SOCKET_RECV,
    Encrypt = LibSshNative.LIBSSH2_ERROR_ENCRYPT,
    BadSocket = LibSshNative.LIBSSH2_ERROR_BAD_SOCKET,
    KnownHosts = LibSshNative.LIBSSH2_ERROR_KNOWN_HOSTS,
    ChannelWindowFull = LibSshNative.LIBSSH2_ERROR_CHANNEL_WINDOW_FULL,
    KeyfileAuthFailed = LibSshNative.LIBSSH2_ERROR_KEYFILE_AUTH_FAILED,
    Randgen = LibSshNative.LIBSSH2_ERROR_RANDGEN,
    MissingUserauthBanner = LibSshNative.LIBSSH2_ERROR_MISSING_USERAUTH_BANNER,
    AlgoUnsupported = LibSshNative.LIBSSH2_ERROR_ALGO_UNSUPPORTED,
    MacFailure = LibSshNative.LIBSSH2_ERROR_MAC_FAILURE,
    HashInit = LibSshNative.LIBSSH2_ERROR_HASH_INIT,
    HashCalc = LibSshNative.LIBSSH2_ERROR_HASH_CALC,
    
    FailedToInitializeSession = int.MaxValue - 2,
    InnerException = int.MaxValue - 1,
    DevWrongUse = int.MaxValue
}
