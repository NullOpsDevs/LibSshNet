using System.Runtime.InteropServices;

namespace NullOpsDevs.LibSsh.Platform;

/// <summary>
/// Platform-specific file stat structure for FreeBSD.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct StatFreebsd
{
    /// <summary>Device ID containing the file.</summary>
    public ulong st_dev;

    /// <summary>Inode number.</summary>
    public ulong st_ino;

    /// <summary>Number of hard links.</summary>
    public ulong st_nlink;

    /// <summary>File mode (type and permissions).</summary>
    public ushort st_mode;

    /// <summary>Padding for alignment.</summary>
    public short st_padding0;

    /// <summary>User ID of the file owner.</summary>
    public uint st_uid;

    /// <summary>Group ID of the file owner.</summary>
    public uint st_gid;

    /// <summary>Padding for alignment.</summary>
    public int st_padding1;

    /// <summary>Device ID if this is a special file.</summary>
    public ulong st_rdev;

    /// <summary>Access time in seconds since epoch.</summary>
    public long st_atim_sec;

    /// <summary>Access time nanoseconds component.</summary>
    public long st_atim_nsec;

    /// <summary>Modification time in seconds since epoch.</summary>
    public long st_mtim_sec;

    /// <summary>Modification time nanoseconds component.</summary>
    public long st_mtim_nsec;

    /// <summary>Status change time in seconds since epoch.</summary>
    public long st_ctim_sec;

    /// <summary>Status change time nanoseconds component.</summary>
    public long st_ctim_nsec;

    /// <summary>File creation time in seconds since epoch.</summary>
    public long st_birthtim_sec;

    /// <summary>File creation time nanoseconds component.</summary>
    public long st_birthtim_nsec;

    /// <summary>Total size in bytes.</summary>
    public long st_size;

    /// <summary>Number of 512-byte blocks allocated.</summary>
    public long st_blocks;

    /// <summary>Optimal block size for I/O.</summary>
    public int st_blksize;

    /// <summary>User-defined flags for file.</summary>
    public uint st_flags;

    /// <summary>File generation number.</summary>
    public ulong st_gen;

    /// <summary>Reserved for future use.</summary>
    public fixed long st_spare[10];
}