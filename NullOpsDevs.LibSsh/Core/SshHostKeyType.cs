using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh.Core;

public enum SshHostKeyType
{
    Unknown = LibSshNative.LIBSSH2_HOSTKEY_TYPE_UNKNOWN,
    Rsa = LibSshNative.LIBSSH2_HOSTKEY_TYPE_RSA,
    
    [Obsolete("Deprecated, see https://libssh2.org/libssh2_session_hostkey.html")]
    Dss = LibSshNative.LIBSSH2_HOSTKEY_TYPE_DSS,
    
    Ecdsa256 = LibSshNative.LIBSSH2_HOSTKEY_TYPE_ECDSA_256,
    Ecdsa384 = LibSshNative.LIBSSH2_HOSTKEY_TYPE_ECDSA_384,
    Ecdsa521 = LibSshNative.LIBSSH2_HOSTKEY_TYPE_ECDSA_521,
    Ed25519 = LibSshNative.LIBSSH2_HOSTKEY_TYPE_ED25519
}