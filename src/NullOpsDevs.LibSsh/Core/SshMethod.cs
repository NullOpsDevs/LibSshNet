using JetBrains.Annotations;
using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh.Core;

/// <summary>
/// Specifies the SSH protocol method types that can be negotiated between client and server.
/// CS = Client-to-Server, SC = Server-to-Client.
/// </summary>
[PublicAPI]
public enum SshMethod
{
    /// <summary>
    /// Key exchange method (e.g., diffie-hellman-group14-sha256, curve25519-sha256).
    /// </summary>
    Kex = LibSshNative.LIBSSH2_METHOD_KEX,

    /// <summary>
    /// Host key algorithm (e.g., ssh-ed25519, rsa-sha2-512, ecdsa-sha2-nistp256).
    /// </summary>
    HostKey = LibSshNative.LIBSSH2_METHOD_HOSTKEY,

    /// <summary>
    /// Encryption cipher for client-to-server traffic (e.g., aes256-ctr, chacha20-poly1305@openssh.com).
    /// </summary>
    CryptCs = LibSshNative.LIBSSH2_METHOD_CRYPT_CS,

    /// <summary>
    /// Encryption cipher for server-to-client traffic (e.g., aes256-ctr, chacha20-poly1305@openssh.com).
    /// </summary>
    CryptSc = LibSshNative.LIBSSH2_METHOD_CRYPT_SC,

    /// <summary>
    /// Message authentication code (MAC) algorithm for client-to-server traffic (e.g., hmac-sha2-256).
    /// </summary>
    MacCs = LibSshNative.LIBSSH2_METHOD_MAC_CS,

    /// <summary>
    /// Message authentication code (MAC) algorithm for server-to-client traffic (e.g., hmac-sha2-256).
    /// </summary>
    MacSc = LibSshNative.LIBSSH2_METHOD_MAC_SC,

    /// <summary>
    /// Compression method for client-to-server traffic (e.g., none, zlib@openssh.com).
    /// </summary>
    CompCs = LibSshNative.LIBSSH2_METHOD_COMP_CS,

    /// <summary>
    /// Compression method for server-to-client traffic (e.g., none, zlib@openssh.com).
    /// </summary>
    CompSc = LibSshNative.LIBSSH2_METHOD_COMP_SC,

    /// <summary>
    /// Language preference for client-to-server communication (rarely used).
    /// </summary>
    LangCs = LibSshNative.LIBSSH2_METHOD_LANG_CS,

    /// <summary>
    /// Language preference for server-to-client communication (rarely used).
    /// </summary>
    LangSc = LibSshNative.LIBSSH2_METHOD_LANG_SC,

    /// <summary>
    /// Signature algorithm used for authentication (e.g., rsa-sha2-512, rsa-sha2-256).
    /// </summary>
    SignAlgo = LibSshNative.LIBSSH2_METHOD_SIGN_ALGO
}