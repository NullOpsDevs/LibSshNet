using System.Runtime.InteropServices;

namespace NullOpsDevs.LibSsh;

/// <summary>
/// Platform-specific file stat structure for macOS (Darwin).
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct StatDarwin
{
    /// <summary>Device ID containing the file.</summary>
    public int st_dev;

    /// <summary>File mode (type and permissions).</summary>
    public ushort st_mode;

    /// <summary>Number of hard links.</summary>
    public ushort st_nlink;

    /// <summary>Inode number.</summary>
    public ulong st_ino;

    /// <summary>User ID of the file owner.</summary>
    public uint st_uid;

    /// <summary>Group ID of the file owner.</summary>
    public uint st_gid;

    /// <summary>Device ID if this is a special file.</summary>
    public int st_rdev;

    /// <summary>Access time in seconds since epoch.</summary>
    public long st_atimespec_sec;

    /// <summary>Access time nanoseconds component.</summary>
    public long st_atimespec_nsec;

    /// <summary>Modification time in seconds since epoch.</summary>
    public long st_mtimespec_sec;

    /// <summary>Modification time nanoseconds component.</summary>
    public long st_mtimespec_nsec;

    /// <summary>Status change time in seconds since epoch.</summary>
    public long st_ctimespec_sec;

    /// <summary>Status change time nanoseconds component.</summary>
    public long st_ctimespec_nsec;

    /// <summary>File creation time in seconds since epoch.</summary>
    public long st_birthtimespec_sec;

    /// <summary>File creation time nanoseconds component.</summary>
    public long st_birthtimespec_nsec;

    /// <summary>Total size in bytes.</summary>
    public long st_size;

    /// <summary>Number of 512-byte blocks allocated.</summary>
    public long st_blocks;

    /// <summary>Optimal block size for I/O.</summary>
    public int st_blksize;

    /// <summary>User-defined flags for file.</summary>
    public uint st_flags;

    /// <summary>File generation number.</summary>
    public uint st_gen;

    /// <summary>Reserved for future use.</summary>
    public int st_lspare;

    /// <summary>Reserved for future use.</summary>
    public fixed long st_qspare[2];
}