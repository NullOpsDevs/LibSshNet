namespace NullOpsDevs.LibSsh;

/// <summary>
/// Represents the result of an SSH command execution.
/// </summary>
public readonly struct SshCommandResult
{
    /// <summary>
    /// Gets a result indicating an unsuccessful command execution.
    /// </summary>
    public static SshCommandResult Unsuccessful => new() { Successful = false };

    /// <summary>
    /// Gets a value indicating whether the command execution was successful.
    /// </summary>
    public bool Successful { get; init; }

    /// <summary>
    /// Gets the standard output (stdout) from the command execution.
    /// </summary>
    public string? Stdout { get; init; }

    /// <summary>
    /// Gets the standard error (stderr) from the command execution.
    /// </summary>
    public string? Stderr { get; init; }
}