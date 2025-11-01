using System.Runtime.InteropServices;

namespace NullOpsDevs.LibSsh.Platform;

/// <summary>
/// Platform-specific file stat structure for Linux x64.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct StatLinuxX64
{
    /// <summary>Device ID containing the file.</summary>
    public ulong st_dev;

    /// <summary>Inode number.</summary>
    public ulong st_ino;

    /// <summary>Number of hard links.</summary>
    public ulong st_nlink;

    /// <summary>File mode (type and permissions).</summary>
    public uint st_mode;

    /// <summary>User ID of the file owner.</summary>
    public uint st_uid;

    /// <summary>Group ID of the file owner.</summary>
    public uint st_gid;

    /// <summary>Padding for alignment.</summary>
    public uint __pad0;

    /// <summary>Device ID if this is a special file.</summary>
    public ulong st_rdev;

    /// <summary>Total size in bytes.</summary>
    public long st_size;

    /// <summary>Optimal block size for I/O.</summary>
    public long st_blksize;

    /// <summary>Number of 512-byte blocks allocated.</summary>
    public long st_blocks;

    /// <summary>Access time in seconds since epoch.</summary>
    public long st_atime;

    /// <summary>Access time nanoseconds component.</summary>
    public long st_atime_nsec;

    /// <summary>Modification time in seconds since epoch.</summary>
    public long st_mtime;

    /// <summary>Modification time nanoseconds component.</summary>
    public long st_mtime_nsec;

    /// <summary>Status change time in seconds since epoch.</summary>
    public long st_ctime;

    /// <summary>Status change time nanoseconds component.</summary>
    public long st_ctime_nsec;

    /// <summary>Reserved for future use.</summary>
    public fixed long __unused[3];
}