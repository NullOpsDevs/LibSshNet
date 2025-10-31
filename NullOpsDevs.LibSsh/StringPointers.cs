using System.Runtime.InteropServices;

namespace NullOpsDevs.LibSsh;

internal class StringPointers
{
    public static readonly unsafe sbyte* Session = (sbyte*)Marshal.StringToHGlobalAnsi("session");
    public static readonly unsafe sbyte* Exec = (sbyte*)Marshal.StringToHGlobalAnsi("exec");
}