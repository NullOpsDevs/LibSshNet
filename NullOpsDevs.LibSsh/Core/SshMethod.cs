using JetBrains.Annotations;
using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh.Core;

[PublicAPI]
public enum SshMethod
{
    Kex = LibSshNative.LIBSSH2_METHOD_KEX,
    HostKey = LibSshNative.LIBSSH2_METHOD_HOSTKEY,
    CryptCs = LibSshNative.LIBSSH2_METHOD_CRYPT_CS,
    CryptSc = LibSshNative.LIBSSH2_METHOD_CRYPT_SC,
    MacCs = LibSshNative.LIBSSH2_METHOD_MAC_CS,
    MacSc = LibSshNative.LIBSSH2_METHOD_MAC_SC,
    CompCs = LibSshNative.LIBSSH2_METHOD_COMP_CS,
    CompSc = LibSshNative.LIBSSH2_METHOD_COMP_SC,
    LangCs = LibSshNative.LIBSSH2_METHOD_LANG_CS,
    LangSc = LibSshNative.LIBSSH2_METHOD_LANG_SC,
    SignAlgo = LibSshNative.LIBSSH2_METHOD_SIGN_ALGO
}