using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using NullOpsDevs.LibSsh.Generated;

namespace NullOpsDevs.LibSsh.Core;

/// <summary>
/// Specifies the hash algorithm to use for computing SSH host key fingerprints.
/// </summary>
[PublicAPI]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum SshHashType
{
    /// <summary>
    /// MD5 hash algorithm (128-bit). Note: MD5 is cryptographically weak and should only be used for compatibility with legacy systems.
    /// </summary>
    MD5 = LibSshNative.LIBSSH2_HOSTKEY_HASH_MD5,

    /// <summary>
    /// SHA-1 hash algorithm (160-bit). Note: SHA-1 is considered weak and SHA-256 is recommended for new implementations.
    /// </summary>
    SHA1 = LibSshNative.LIBSSH2_HOSTKEY_HASH_SHA1,

    /// <summary>
    /// SHA-256 hash algorithm (256-bit). This is the recommended hash algorithm for host key verification.
    /// </summary>
    SHA256 = LibSshNative.LIBSSH2_HOSTKEY_HASH_SHA256
}
