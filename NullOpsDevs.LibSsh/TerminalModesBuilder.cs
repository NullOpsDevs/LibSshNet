using JetBrains.Annotations;

namespace NullOpsDevs.LibSsh;

/// <summary>
/// Fluent builder for constructing terminal modes byte array for PTY requests.
/// Terminal modes are encoded as defined in RFC 4254, Section 8.
/// </summary>
[PublicAPI]
public class TerminalModesBuilder
{
    /// <summary>
    /// Gets an empty terminal modes array (just TTY_OP_END).
    /// This tells the server to use default terminal settings.
    /// </summary>
    public static readonly byte[] Empty = [0];

    private readonly List<byte> buffer = [];

    /// <summary>
    /// Sets a control character mode value.
    /// </summary>
    /// <param name="mode">The terminal mode (e.g., VINTR, VEOF).</param>
    /// <param name="value">The character value (e.g., 3 for Ctrl-C).</param>
    /// <returns>This builder for fluent chaining.</returns>
    public TerminalModesBuilder SetCharacter(TerminalMode mode, byte value)
    {
        buffer.Add((byte)mode);
        WriteUInt32(value);
        return this;
    }

    /// <summary>
    /// Sets a boolean flag mode (enabled = 1, disabled = 0).
    /// </summary>
    /// <param name="mode">The terminal mode flag (e.g., ECHO, ICANON, ISIG).</param>
    /// <param name="enabled">True to enable the flag, false to disable.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public TerminalModesBuilder SetFlag(TerminalMode mode, bool enabled)
    {
        buffer.Add((byte)mode);
        WriteUInt32(enabled ? 1u : 0u);
        return this;
    }

    /// <summary>
    /// Sets a terminal mode with a custom uint32 value.
    /// </summary>
    /// <param name="mode">The terminal mode.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public TerminalModesBuilder SetMode(TerminalMode mode, uint value)
    {
        buffer.Add((byte)mode);
        WriteUInt32(value);
        return this;
    }

    /// <summary>
    /// Sets both input and output baud rates.
    /// </summary>
    /// <param name="speed">The baud rate (e.g., 38400).</param>
    /// <returns>This builder for fluent chaining.</returns>
    public TerminalModesBuilder SetSpeed(uint speed)
    {
        // Set input speed
        buffer.Add((byte)TerminalMode.TTY_OP_ISPEED);
        WriteUInt32(speed);

        // Set output speed
        buffer.Add((byte)TerminalMode.TTY_OP_OSPEED);
        WriteUInt32(speed);

        return this;
    }

    /// <summary>
    /// Builds the terminal modes byte array, appending TTY_OP_END terminator.
    /// </summary>
    /// <returns>The encoded terminal modes ready for use with libssh2.</returns>
    public byte[] Build()
    {
        // Add TTY_OP_END terminator
        buffer.Add((byte)TerminalMode.TTY_OP_END);
        return [.. buffer];
    }

    /// <summary>
    /// Writes a uint32 value in network byte order (big-endian) as required by SSH protocol.
    /// </summary>
    private void WriteUInt32(uint value)
    {
        buffer.Add((byte)(value >> 24));
        buffer.Add((byte)(value >> 16));
        buffer.Add((byte)(value >> 8));
        buffer.Add((byte)value);
    }
}
