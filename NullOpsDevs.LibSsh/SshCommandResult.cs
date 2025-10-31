namespace NullOpsDevs.LibSsh;

public readonly struct SshCommandResult
{
    public static SshCommandResult Unsuccessful => new() { Successful = false };
    
    public bool Successful { get; init; }
    
    public string? Stdout { get; init; }
    public string? Stderr { get; init; }
}