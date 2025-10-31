using System.Runtime.InteropServices;
using System.Text;

namespace NullOpsDevs.LibSsh;

internal readonly ref struct NativeBuffer(IntPtr pointer, int length) : IDisposable
{
    public IntPtr Pointer { get; } = pointer;
    public int Length { get; } = length;
    public unsafe Span<byte> Span => new(Pointer.ToPointer(), Length);
    public unsafe T* AsPointer<T>() where T : unmanaged => (T*)Pointer.ToPointer();
    
    public string AsString() => Marshal.PtrToStringUTF8(Pointer) ?? string.Empty;
    
    public void Clear() => Span.Clear();
    
    /// <inheritdoc />
    public void Dispose() => Marshal.FreeHGlobal(Pointer);

    public static NativeBuffer Allocate(int length)
    {
        var buf = Marshal.AllocHGlobal(length);
        return new NativeBuffer(buf, length);
    }
    
    public static NativeBuffer Allocate(string value)
    {
        var buf = Allocate(Encoding.UTF8.GetByteCount(value));
        Encoding.UTF8.GetBytes(value, buf.Span);

        return buf;
    }
}