using System.Runtime.InteropServices;

namespace NullOpsDevs.LibSsh.Platform;

/// <summary>
/// Platform-specific file stat structure for MinGW64 (Windows).
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct StatMingw64
{
    /// <summary>Device ID containing the file.</summary>
    public uint st_dev;

    /// <summary>Inode number.</summary>
    public ushort st_ino;

    /// <summary>File mode (type and permissions).</summary>
    public ushort st_mode;

    /// <summary>Number of hard links.</summary>
    public short st_nlink;

    /// <summary>User ID of the file owner.</summary>
    public short st_uid;

    /// <summary>Group ID of the file owner.</summary>
    public short st_gid;

    /// <summary>Device ID if this is a special file.</summary>
    public uint st_rdev;

    /// <summary>Total size in bytes.</summary>
    public long st_size;

    /// <summary>Time of last access (Unix timestamp).</summary>
    public long st_atime;

    /// <summary>Time of last modification (Unix timestamp).</summary>
    public long st_mtime;

    /// <summary>Time of last status change (Unix timestamp).</summary>
    public long st_ctime;
}