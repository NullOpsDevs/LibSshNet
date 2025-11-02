using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh.Core;

/// <summary>
/// Specifies the type of SSH host key algorithm used by the server.
/// </summary>
public enum SshHostKeyType
{
    /// <summary>
    /// Unknown or unsupported host key type.
    /// </summary>
    Unknown = LibSshNative.LIBSSH2_HOSTKEY_TYPE_UNKNOWN,

    /// <summary>
    /// RSA (Rivest-Shamir-Adleman) public key algorithm.
    /// </summary>
    Rsa = LibSshNative.LIBSSH2_HOSTKEY_TYPE_RSA,

    /// <summary>
    /// DSS (Digital Signature Standard) / DSA public key algorithm.
    /// </summary>
    [Obsolete("Deprecated, see https://libssh2.org/libssh2_session_hostkey.html")]
    Dss = LibSshNative.LIBSSH2_HOSTKEY_TYPE_DSS,

    /// <summary>
    /// ECDSA (Elliptic Curve Digital Signature Algorithm) with NIST P-256 curve.
    /// </summary>
    Ecdsa256 = LibSshNative.LIBSSH2_HOSTKEY_TYPE_ECDSA_256,

    /// <summary>
    /// ECDSA (Elliptic Curve Digital Signature Algorithm) with NIST P-384 curve.
    /// </summary>
    Ecdsa384 = LibSshNative.LIBSSH2_HOSTKEY_TYPE_ECDSA_384,

    /// <summary>
    /// ECDSA (Elliptic Curve Digital Signature Algorithm) with NIST P-521 curve.
    /// </summary>
    Ecdsa521 = LibSshNative.LIBSSH2_HOSTKEY_TYPE_ECDSA_521,

    /// <summary>
    /// Ed25519 elliptic curve signature algorithm (recommended for new deployments).
    /// </summary>
    Ed25519 = LibSshNative.LIBSSH2_HOSTKEY_TYPE_ED25519
}