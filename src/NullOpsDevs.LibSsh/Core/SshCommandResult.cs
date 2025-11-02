namespace NullOpsDevs.LibSsh.Core;

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

    /// <summary>
    /// Gets the exit code from the command execution.
    /// A value of 0 typically indicates success, while non-zero values indicate an error.
    /// This value may be null if the exit code could not be retrieved.
    /// </summary>
    public int? ExitCode { get; init; }

    /// <summary>
    /// Gets the exit signal from the command execution if the command was terminated by a signal.
    /// This value may be null if no signal was received or if the signal could not be retrieved.
    /// Common signals include "TERM", "KILL", "HUP", etc.
    /// </summary>
    public string? ExitSignal { get; init; }
}