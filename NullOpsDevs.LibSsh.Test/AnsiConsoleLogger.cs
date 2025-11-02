using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace NullOpsDevs.LibSsh.Test;

public class AnsiConsoleLogger : ILogger
{
    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId _, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var logLevelColor = logLevel switch
        {
            LogLevel.Trace => "dim",
            LogLevel.Debug => "dim",
            LogLevel.Information => "white",
            LogLevel.Warning => "yellow",
            LogLevel.Error => "red",
            LogLevel.Critical => "red",
            _ => "white"
        };
        
        var formatted = formatter(state, exception);
        AnsiConsole.MarkupLine($"[{logLevelColor}]{logLevel:G}[/] {Markup.Escape(formatted)}");
    }
}