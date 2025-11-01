using System.Runtime.InteropServices;

namespace NullOpsDevs.LibSsh.Interop;

/// <summary>
/// Provides cached native string pointers for commonly used libssh2 string constants.
/// </summary>
internal static class StringPointers
{
    /// <summary>
    /// Pointer to the "session" string used for SSH channel types.
    /// </summary>
    public static readonly unsafe sbyte* Session = (sbyte*)Marshal.StringToHGlobalAnsi("session");

    /// <summary>
    /// Pointer to the "exec" string used for command execution channel requests.
    /// </summary>
    public static readonly unsafe sbyte* Exec = (sbyte*)Marshal.StringToHGlobalAnsi("exec");
}