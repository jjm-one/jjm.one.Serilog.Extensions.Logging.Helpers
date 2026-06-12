using System.Reflection;
using Moq;
using Serilog;
using Serilog.Events;

namespace jjm.one.Serilog.Extensions.Logging.Helpers.Tests.FunctionLogging;

/// <summary>
///     Tests for the <see cref="FunctionLogging" /> class.
/// </summary>
public class FunctionLoggingTests
{
    #region private members

    private readonly Mock<ILogger> _logger;

    #endregion

    #region ctors

    /// <summary>
    ///     The default constructor of the <see cref="FunctionLoggingTests" /> class.
    /// </summary>
    public FunctionLoggingTests() => _logger = new Mock<ILogger>();

    #endregion

    #region LogFctCall tests

    /// <summary>
    ///     LogFctCall (auto-detect) logs at the default Debug level.
    /// </summary>
    [Fact]
    public void LogFctCallTest1()
    {
        _logger.Object.LogFctCall();

        _logger.Verify(x => x.Write(LogEventLevel.Debug,
                "Function called: {ClassName} -> {FctName}",
                nameof(FunctionLoggingTests), nameof(LogFctCallTest1)),
            Times.Once);
    }

    /// <summary>
    ///     LogFctCall (explicit types) logs at the default Debug level.
    /// </summary>
    [Fact]
    public void LogFctCallTest2()
    {
        _logger.Object.LogFctCall(GetType(), MethodBase.GetCurrentMethod());

        _logger.Verify(x => x.Write(LogEventLevel.Debug,
                "Function called: {ClassName} -> {FctName}",
                nameof(FunctionLoggingTests), nameof(LogFctCallTest2)),
            Times.Once);
    }

    /// <summary>
    ///     LogFctCall (auto-detect) respects a custom log level.
    /// </summary>
    [Theory]
    [InlineData(LogEventLevel.Verbose)]
    [InlineData(LogEventLevel.Information)]
    [InlineData(LogEventLevel.Warning)]
    [InlineData(LogEventLevel.Error)]
    [InlineData(LogEventLevel.Fatal)]
    public void LogFctCall_CustomLevel_AutoDetect(LogEventLevel level)
    {
        _logger.Object.LogFctCall(level);

        _logger.Verify(x => x.Write(level,
                "Function called: {ClassName} -> {FctName}",
                nameof(FunctionLoggingTests), nameof(LogFctCall_CustomLevel_AutoDetect)),
            Times.Once);
    }

    /// <summary>
    ///     LogFctCall (explicit types) respects a custom log level.
    /// </summary>
    [Theory]
    [InlineData(LogEventLevel.Verbose)]
    [InlineData(LogEventLevel.Information)]
    [InlineData(LogEventLevel.Warning)]
    [InlineData(LogEventLevel.Error)]
    [InlineData(LogEventLevel.Fatal)]
    public void LogFctCall_CustomLevel_Explicit(LogEventLevel level)
    {
        _logger.Object.LogFctCall(GetType(), MethodBase.GetCurrentMethod(), level);

        _logger.Verify(x => x.Write(level,
                "Function called: {ClassName} -> {FctName}",
                nameof(FunctionLoggingTests), nameof(LogFctCall_CustomLevel_Explicit)),
            Times.Once);
    }

    /// <summary>
    ///     LogFctCall (explicit types) logs null class/method names as null.
    /// </summary>
    [Fact]
    public void LogFctCall_NullClassAndMethod_LogsNulls()
    {
        _logger.Object.LogFctCall(null, null);

        _logger.Verify(x => x.Write(LogEventLevel.Debug,
                "Function called: {ClassName} -> {FctName}",
                (string?)null, (string?)null),
            Times.Once);
    }

    #endregion

    #region LogExcInFctCall tests

    /// <summary>
    ///     LogExcInFctCall (auto-detect) with no custom message uses an empty string.
    /// </summary>
    [Fact]
    public void LogExcInFctCallTest1()
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc);

        _logger.Verify(x => x.Write(LogEventLevel.Error,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                nameof(FunctionLoggingTests), nameof(LogExcInFctCallTest1),
                string.Empty),
            Times.Once);
    }

    /// <summary>
    ///     LogExcInFctCall (auto-detect) with a custom message prepends a newline.
    /// </summary>
    [Fact]
    public void LogExcInFctCallTest2()
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc, "TestMSG");

        _logger.Verify(x => x.Write(LogEventLevel.Error,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                nameof(FunctionLoggingTests), nameof(LogExcInFctCallTest2),
                "\nTestMSG"),
            Times.Once);
    }

    /// <summary>
    ///     LogExcInFctCall (explicit types) with no custom message uses an empty string.
    /// </summary>
    [Fact]
    public void LogExcInFctCallTest3()
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc, GetType(), MethodBase.GetCurrentMethod());

        _logger.Verify(x => x.Write(LogEventLevel.Error,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                nameof(FunctionLoggingTests), nameof(LogExcInFctCallTest3),
                string.Empty),
            Times.Once);
    }

    /// <summary>
    ///     LogExcInFctCall (explicit types) with a custom message prepends a newline.
    /// </summary>
    [Fact]
    public void LogExcInFctCallTest4()
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc, GetType(), MethodBase.GetCurrentMethod(), "TestMSG");

        _logger.Verify(x => x.Write(LogEventLevel.Error,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                nameof(FunctionLoggingTests), nameof(LogExcInFctCallTest4),
                "\nTestMSG"),
            Times.Once);
    }

    /// <summary>
    ///     LogExcInFctCall (auto-detect) respects a custom log level.
    /// </summary>
    [Theory]
    [InlineData(LogEventLevel.Verbose)]
    [InlineData(LogEventLevel.Debug)]
    [InlineData(LogEventLevel.Information)]
    [InlineData(LogEventLevel.Warning)]
    [InlineData(LogEventLevel.Fatal)]
    public void LogExcInFctCall_CustomLevel_AutoDetect(LogEventLevel level)
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc, level: level);

        _logger.Verify(x => x.Write(level,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                nameof(FunctionLoggingTests), nameof(LogExcInFctCall_CustomLevel_AutoDetect),
                string.Empty),
            Times.Once);
    }

    /// <summary>
    ///     LogExcInFctCall (explicit types) respects a custom log level.
    /// </summary>
    [Theory]
    [InlineData(LogEventLevel.Verbose)]
    [InlineData(LogEventLevel.Debug)]
    [InlineData(LogEventLevel.Information)]
    [InlineData(LogEventLevel.Warning)]
    [InlineData(LogEventLevel.Fatal)]
    public void LogExcInFctCall_CustomLevel_Explicit(LogEventLevel level)
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc, GetType(), MethodBase.GetCurrentMethod(), level: level);

        _logger.Verify(x => x.Write(level,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                nameof(FunctionLoggingTests), nameof(LogExcInFctCall_CustomLevel_Explicit),
                string.Empty),
            Times.Once);
    }

    /// <summary>
    ///     LogExcInFctCall (explicit types) logs null class/method names as null.
    /// </summary>
    [Fact]
    public void LogExcInFctCall_NullClassAndMethod_LogsNulls()
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc, null, null);

        _logger.Verify(x => x.Write(LogEventLevel.Error,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                (string?)null, (string?)null, string.Empty),
            Times.Once);
    }

    #endregion
}
