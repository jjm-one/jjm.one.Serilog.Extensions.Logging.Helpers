using System.Reflection;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Events;

namespace jjm.one.Serilog.Extensions.Logging.Helpers;

/// <summary>
///     Static class for all function logging helper extensions.
/// </summary>
public static class FunctionLogging
{
    /// <summary>
    ///     Log a function/method call. The class and method names are resolved at compile time via
    ///     caller-info attributes — zero allocation, no stack walk.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="level">The logging level. Default is <see cref="LogEventLevel.Debug" />.</param>
    /// <param name="memberName">Compiler-supplied calling member name.</param>
    /// <param name="sourceFilePath">Compiler-supplied calling source file path.</param>
    public static void LogFctCall(
        this ILogger logger,
        LogEventLevel level = LogEventLevel.Debug,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "")
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (!logger.IsEnabled(level))
        {
            return;
        }

        logger.Write(level, "Function called: {ClassName} -> {FctName}",
            Path.GetFileNameWithoutExtension(sourceFilePath), memberName);
    }

    /// <summary>
    ///     Log a function/method call.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="classType">The <see cref="Type" /> of the calling class.</param>
    /// <param name="methodType">The <see cref="MethodBase" /> of the calling function/method.</param>
    /// <param name="level">The logging level. Default is <see cref="LogEventLevel.Debug" />.</param>
    public static void LogFctCall(
        this ILogger logger,
        Type? classType,
        MethodBase? methodType,
        LogEventLevel level = LogEventLevel.Debug)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (!logger.IsEnabled(level))
        {
            return;
        }

        logger.Write(level, "Function called: {ClassName} -> {FctName}",
            classType?.Name, methodType?.Name);
    }

    /// <summary>
    ///     Log an exception within a function/method call. The class and method names are resolved at
    ///     compile time via caller-info attributes — zero allocation, no stack walk.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="exc">The exception.</param>
    /// <param name="msg">An optional context message appended to the log entry.</param>
    /// <param name="level">The logging level. Default is <see cref="LogEventLevel.Error" />.</param>
    /// <param name="memberName">Compiler-supplied calling member name.</param>
    /// <param name="sourceFilePath">Compiler-supplied calling source file path.</param>
    public static void LogExcInFctCall(
        this ILogger logger,
        Exception exc,
        string? msg = null,
        LogEventLevel level = LogEventLevel.Error,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "")
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (exc is null)
        {
            throw new ArgumentNullException(nameof(exc));
        }

        if (!logger.IsEnabled(level))
        {
            return;
        }

        var customMsg = string.IsNullOrWhiteSpace(msg) ? string.Empty : "\n" + msg;

        logger.Write(level, exc, "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
            Path.GetFileNameWithoutExtension(sourceFilePath), memberName, customMsg);
    }

    /// <summary>
    ///     Log an exception within a function/method call.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="exc">The exception.</param>
    /// <param name="classType">The <see cref="Type" /> of the calling class.</param>
    /// <param name="methodType">The <see cref="MethodBase" /> of the calling function/method.</param>
    /// <param name="msg">An optional context message appended to the log entry.</param>
    /// <param name="level">The logging level. Default is <see cref="LogEventLevel.Error" />.</param>
    public static void LogExcInFctCall(
        this ILogger logger,
        Exception exc,
        Type? classType,
        MethodBase? methodType,
        string? msg = null,
        LogEventLevel level = LogEventLevel.Error)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (exc is null)
        {
            throw new ArgumentNullException(nameof(exc));
        }

        if (!logger.IsEnabled(level))
        {
            return;
        }

        var customMsg = string.IsNullOrWhiteSpace(msg) ? string.Empty : "\n" + msg;

        logger.Write(level, exc, "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
            classType?.Name, methodType?.Name, customMsg);
    }
}
