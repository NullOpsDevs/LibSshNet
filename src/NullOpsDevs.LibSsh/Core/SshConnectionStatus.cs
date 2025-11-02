namespace NullOpsDevs.LibSsh.Core;

/// <summary>
/// Represents the connection status of an SSH session.
/// </summary>
public enum SshConnectionStatus
{
    /// <summary>
    /// The session is not connected to any server.
    /// </summary>
    Disconnected,

    /// <summary>
    /// The session has established a connection to the server but is not yet authenticated.
    /// </summary>
    Connected,

    /// <summary>
    /// The session is connected and successfully authenticated (logged in).
    /// </summary>
    LoggedIn,
    
    /// <summary>
    /// Session has been disposed.
    /// </summary>
    Disposed
}
