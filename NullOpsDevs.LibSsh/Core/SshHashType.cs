using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh.Core;

[PublicAPI]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum SshHashType
{
    MD5 = LibSshNative.LIBSSH2_HOSTKEY_HASH_MD5,
    
    SHA1 = LibSshNative.LIBSSH2_HOSTKEY_HASH_SHA1,
    
    SHA256 = LibSshNative.LIBSSH2_HOSTKEY_HASH_SHA256
}
