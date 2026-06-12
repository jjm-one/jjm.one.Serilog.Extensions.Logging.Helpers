using System.Reflection;
using Moq;
using Serilog;
using Serilog.Events;

namespace jjm.one.Serilog.Extensions.Logging.Helpers.Tests.FunctionLogging;

/// <summary>
///     Tests for <see cref="FunctionLogging" />.
/// </summary>
public class FunctionLoggingTests
{
    #region private members

    private readonly Mock<ILogger> _logger;

    #endregion

    #region ctors

    public FunctionLoggingTests()
    {
        _logger = new Mock<ILogger>();
        // Enable logging for all levels by default so positive tests don't need individual setup.
        _logger.Setup(x => x.IsEnabled(It.IsAny<LogEventLevel>())).Returns(true);
    }

    #endregion

    #region LogFctCall — auto-detect overload

    [Fact]
    public void LogFctCall_AutoDetect_DefaultLevel_LogsDebug()
    {
        _logger.Object.LogFctCall();

        _logger.Verify(x => x.Write(
                LogEventLevel.Debug,
                "Function called: {ClassName} -> {FctName}",
                nameof(FunctionLoggingTests),
                nameof(LogFctCall_AutoDetect_DefaultLevel_LogsDebug)),
            Times.Once);
    }

    [Theory]
    [InlineData(LogEventLevel.Verbose)]
    [InlineData(LogEventLevel.Information)]
    [InlineData(LogEventLevel.Warning)]
    [InlineData(LogEventLevel.Error)]
    [InlineData(LogEventLevel.Fatal)]
    public void LogFctCall_AutoDetect_CustomLevel_LogsAtThatLevel(LogEventLevel level)
    {
        _logger.Object.LogFctCall(level);

        _logger.Verify(x => x.Write(
                level,
                "Function called: {ClassName} -> {FctName}",
                It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public void LogFctCall_AutoDetect_LoggingDisabled_DoesNotLog()
    {
        var logger = new Mock<ILogger>();
        logger.Setup(x => x.IsEnabled(LogEventLevel.Debug)).Returns(false);

        logger.Object.LogFctCall();

        logger.Verify(x => x.Write(
                It.IsAny<LogEventLevel>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public void LogFctCall_AutoDetect_NullLogger_ThrowsArgumentNullException()
    {
        ILogger? nullLogger = null;
        Assert.Throws<ArgumentNullException>(() => nullLogger!.LogFctCall());
    }

    #endregion

    #region LogFctCall — explicit type/method overload

    [Fact]
    public void LogFctCall_Explicit_DefaultLevel_LogsDebug()
    {
        _logger.Object.LogFctCall(GetType(), MethodBase.GetCurrentMethod());

        _logger.Verify(x => x.Write(
                LogEventLevel.Debug,
                "Function called: {ClassName} -> {FctName}",
                nameof(FunctionLoggingTests),
                nameof(LogFctCall_Explicit_DefaultLevel_LogsDebug)),
            Times.Once);
    }

    [Theory]
    [InlineData(LogEventLevel.Verbose)]
    [InlineData(LogEventLevel.Information)]
    [InlineData(LogEventLevel.Warning)]
    [InlineData(LogEventLevel.Error)]
    [InlineData(LogEventLevel.Fatal)]
    public void LogFctCall_Explicit_CustomLevel_LogsAtThatLevel(LogEventLevel level)
    {
        _logger.Object.LogFctCall(GetType(), MethodBase.GetCurrentMethod(), level);

        _logger.Verify(x => x.Write(
                level,
                "Function called: {ClassName} -> {FctName}",
                It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public void LogFctCall_Explicit_LoggingDisabled_DoesNotLog()
    {
        var logger = new Mock<ILogger>();
        logger.Setup(x => x.IsEnabled(LogEventLevel.Debug)).Returns(false);

        logger.Object.LogFctCall(GetType(), MethodBase.GetCurrentMethod());

        logger.Verify(x => x.Write(
                It.IsAny<LogEventLevel>(), It.IsAny<string>(),
                It.IsAny<string?>(), It.IsAny<string?>()),
            Times.Never);
    }

    [Fact]
    public void LogFctCall_Explicit_NullLogger_ThrowsArgumentNullException()
    {
        ILogger? nullLogger = null;
        Assert.Throws<ArgumentNullException>(() =>
            nullLogger!.LogFctCall(GetType(), MethodBase.GetCurrentMethod()));
    }

    [Fact]
    public void LogFctCall_Explicit_NullClassType_LogsNullClassName()
    {
        _logger.Object.LogFctCall(null, MethodBase.GetCurrentMethod());

        _logger.Verify(x => x.Write(
                LogEventLevel.Debug,
                "Function called: {ClassName} -> {FctName}",
                (string?)null,
                nameof(LogFctCall_Explicit_NullClassType_LogsNullClassName)),
            Times.Once);
    }

    [Fact]
    public void LogFctCall_Explicit_NullMethodType_LogsNullMethodName()
    {
        _logger.Object.LogFctCall(GetType(), null);

        _logger.Verify(x => x.Write(
                LogEventLevel.Debug,
                "Function called: {ClassName} -> {FctName}",
                nameof(FunctionLoggingTests),
                (string?)null),
            Times.Once);
    }

    #endregion

    #region LogExcInFctCall — auto-detect overload

    [Fact]
    public void LogExcInFctCall_AutoDetect_NoMsg_LogsErrorWithoutCustomMsg()
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc);

        _logger.Verify(x => x.Write(
                LogEventLevel.Error,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                nameof(FunctionLoggingTests),
                nameof(LogExcInFctCall_AutoDetect_NoMsg_LogsErrorWithoutCustomMsg),
                string.Empty),
            Times.Once);
    }

    [Fact]
    public void LogExcInFctCall_AutoDetect_WithMsg_AppendsNewlineAndMsg()
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc, "TestMSG");

        _logger.Verify(x => x.Write(
                LogEventLevel.Error,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                nameof(FunctionLoggingTests),
                nameof(LogExcInFctCall_AutoDetect_WithMsg_AppendsNewlineAndMsg),
                "\nTestMSG"),
            Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void LogExcInFctCall_AutoDetect_NullOrWhitespaceMsg_LogsWithoutCustomMsg(string? msg)
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc, msg);

        _logger.Verify(x => x.Write(
                LogEventLevel.Error,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                nameof(FunctionLoggingTests),
                nameof(LogExcInFctCall_AutoDetect_NullOrWhitespaceMsg_LogsWithoutCustomMsg),
                string.Empty),
            Times.Once);
    }

    [Theory]
    [InlineData(LogEventLevel.Verbose)]
    [InlineData(LogEventLevel.Debug)]
    [InlineData(LogEventLevel.Information)]
    [InlineData(LogEventLevel.Warning)]
    [InlineData(LogEventLevel.Fatal)]
    public void LogExcInFctCall_AutoDetect_CustomLevel_LogsAtThatLevel(LogEventLevel level)
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc, null, level);

        _logger.Verify(x => x.Write(
                level,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public void LogExcInFctCall_AutoDetect_LoggingDisabled_DoesNotLog()
    {
        var logger = new Mock<ILogger>();
        logger.Setup(x => x.IsEnabled(LogEventLevel.Error)).Returns(false);
        var exc = new Exception("Test");

        logger.Object.LogExcInFctCall(exc);

        logger.Verify(x => x.Write(
                It.IsAny<LogEventLevel>(), It.IsAny<Exception?>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public void LogExcInFctCall_AutoDetect_NullLogger_ThrowsArgumentNullException()
    {
        ILogger? nullLogger = null;
        Assert.Throws<ArgumentNullException>(() => nullLogger!.LogExcInFctCall(new Exception()));
    }

    [Fact]
    public void LogExcInFctCall_AutoDetect_NullException_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => _logger.Object.LogExcInFctCall(null!));

    #endregion

    #region LogExcInFctCall — explicit type/method overload

    [Fact]
    public void LogExcInFctCall_Explicit_NoMsg_LogsErrorWithoutCustomMsg()
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc, GetType(), MethodBase.GetCurrentMethod());

        _logger.Verify(x => x.Write(
                LogEventLevel.Error,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                nameof(FunctionLoggingTests),
                nameof(LogExcInFctCall_Explicit_NoMsg_LogsErrorWithoutCustomMsg),
                string.Empty),
            Times.Once);
    }

    [Fact]
    public void LogExcInFctCall_Explicit_WithMsg_AppendsNewlineAndMsg()
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc, GetType(), MethodBase.GetCurrentMethod(), "TestMSG");

        _logger.Verify(x => x.Write(
                LogEventLevel.Error,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                nameof(FunctionLoggingTests),
                nameof(LogExcInFctCall_Explicit_WithMsg_AppendsNewlineAndMsg),
                "\nTestMSG"),
            Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void LogExcInFctCall_Explicit_NullOrWhitespaceMsg_LogsWithoutCustomMsg(string? msg)
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc, GetType(), MethodBase.GetCurrentMethod(), msg);

        _logger.Verify(x => x.Write(
                LogEventLevel.Error,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                nameof(FunctionLoggingTests),
                nameof(LogExcInFctCall_Explicit_NullOrWhitespaceMsg_LogsWithoutCustomMsg),
                string.Empty),
            Times.Once);
    }

    [Theory]
    [InlineData(LogEventLevel.Verbose)]
    [InlineData(LogEventLevel.Debug)]
    [InlineData(LogEventLevel.Information)]
    [InlineData(LogEventLevel.Warning)]
    [InlineData(LogEventLevel.Fatal)]
    public void LogExcInFctCall_Explicit_CustomLevel_LogsAtThatLevel(LogEventLevel level)
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc, GetType(), MethodBase.GetCurrentMethod(), null, level);

        _logger.Verify(x => x.Write(
                level,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public void LogExcInFctCall_Explicit_LoggingDisabled_DoesNotLog()
    {
        var logger = new Mock<ILogger>();
        logger.Setup(x => x.IsEnabled(LogEventLevel.Error)).Returns(false);
        var exc = new Exception("Test");

        logger.Object.LogExcInFctCall(exc, GetType(), MethodBase.GetCurrentMethod());

        logger.Verify(x => x.Write(
                It.IsAny<LogEventLevel>(), It.IsAny<Exception?>(), It.IsAny<string>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public void LogExcInFctCall_Explicit_NullLogger_ThrowsArgumentNullException()
    {
        ILogger? nullLogger = null;
        Assert.Throws<ArgumentNullException>(() =>
            nullLogger!.LogExcInFctCall(new Exception(), GetType(), MethodBase.GetCurrentMethod()));
    }

    [Fact]
    public void LogExcInFctCall_Explicit_NullException_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _logger.Object.LogExcInFctCall(null!, GetType(), MethodBase.GetCurrentMethod()));
    }

    [Fact]
    public void LogExcInFctCall_Explicit_NullClassType_LogsNullClassName()
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc, null, MethodBase.GetCurrentMethod());

        _logger.Verify(x => x.Write(
                LogEventLevel.Error,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                (string?)null,
                nameof(LogExcInFctCall_Explicit_NullClassType_LogsNullClassName),
                string.Empty),
            Times.Once);
    }

    [Fact]
    public void LogExcInFctCall_Explicit_NullMethodType_LogsNullMethodName()
    {
        var exc = new Exception("Test");

        _logger.Object.LogExcInFctCall(exc, GetType(), null);

        _logger.Verify(x => x.Write(
                LogEventLevel.Error,
                It.Is<Exception>(e => e == exc),
                "Exception thrown in: {ClassName} -> {FctName}{CustomMsg}",
                nameof(FunctionLoggingTests),
                (string?)null,
                string.Empty),
            Times.Once);
    }

    #endregion
}
