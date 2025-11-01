using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NullOpsDevs.LibSsh;

/// <summary>
/// Represents a disposable wrapper around unmanaged memory allocated from the global heap.
/// </summary>
/// <param name="pointer">The pointer to the allocated memory.</param>
/// <param name="length">The length of the allocated memory in bytes.</param>
internal readonly ref struct NativeBuffer(IntPtr pointer, int length) : IDisposable
{
    /// <summary>
    /// Gets the pointer to the allocated native memory.
    /// </summary>
    public IntPtr Pointer { get; } = pointer;

    /// <summary>
    /// Gets the length of the allocated memory in bytes.
    /// </summary>
    public int Length { get; } = length;
    
    /// <summary>
    /// Returns the allocated memory as a UTF-8 encoded string.
    /// </summary>
    public string AsString() => Marshal.PtrToStringUTF8(Pointer) ?? string.Empty;

    /// <summary>
    /// Gets a span view of the allocated memory as bytes.
    /// </summary>
    public unsafe Span<byte> Span => new(Pointer.ToPointer(), Length);

    /// <summary>
    /// Gets an untyped pointer to the allocated memory.
    /// </summary>
    /// <returns>A void pointer to the start of the allocated memory buffer.</returns>
    public unsafe void* AsPointer() => Pointer.ToPointer();
    
    /// <summary>
    /// Gets a typed pointer to the allocated memory.
    /// </summary>
    /// <typeparam name="T">The unmanaged type to cast the pointer to.</typeparam>
    /// <returns>A pointer to the allocated memory cast as the specified type.</returns>
    public unsafe T* AsPointer<T>() where T : unmanaged => (T*)Pointer.ToPointer();

    /// <inheritdoc />
    public void Dispose() => Marshal.FreeHGlobal(Pointer);

    /// <summary>
    /// Allocates a native buffer of the specified length.
    /// </summary>
    /// <param name="length">The number of bytes to allocate.</param>
    /// <returns>A new NativeBuffer with allocated memory.</returns>
    public static unsafe NativeBuffer Allocate(int length)
    {
        var buf = Marshal.AllocHGlobal(length);
        Unsafe.InitBlockUnaligned(buf.ToPointer(), 0, (uint)length);
        return new NativeBuffer(buf, length);
    }

    /// <summary>
    /// Allocates a native buffer and copies a UTF-8 encoded string into it.
    /// </summary>
    /// <param name="value">The string to encode and copy.</param>
    /// <returns>A new NativeBuffer containing the UTF-8 encoded string.</returns>
    public static NativeBuffer Allocate(string value)
    {
        var buf = Allocate(Encoding.UTF8.GetByteCount(value) + 1);
        Encoding.UTF8.GetBytes(value, buf.Span);
        buf.Span[^1] = 0;

        return buf;
    }

    /// <summary>
    /// Allocates a native buffer and copies the byte array data into it.
    /// </summary>
    /// <param name="data">The byte array to copy into native memory.</param>
    /// <returns>A new NativeBuffer containing a copy of the data.</returns>
    public static NativeBuffer Allocate(byte[] data)
    {
        var buf = Allocate(data.Length);
        data.CopyTo(buf.Span);

        return buf;
    }
}