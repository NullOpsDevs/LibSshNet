namespace NullOpsDevs.LibSsh.Exceptions;

public class SshException(string message, SshError error, Exception? innerException = null) : Exception(message, innerException)
{
    public SshError Error { get; } = error;
}
