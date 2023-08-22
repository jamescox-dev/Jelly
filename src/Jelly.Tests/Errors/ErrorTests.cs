namespace Jelly.Errors.Test;

[TestFixture]
public class ErrorTests
{
    [TestCase("", "/")]
    [TestCase("/", "/")]
    [TestCase("//", "/")]
    [TestCase("///", "/")]
    [TestCase("error", "/error/")]
    [TestCase("/error", "/error/")]
    [TestCase("//error", "/error/")]
    [TestCase("error/", "/error/")]
    [TestCase("error//", "/error/")]
    [TestCase("/error/", "/error/")]
    [TestCase("//error//", "/error/")]
    [TestCase("error/test", "/error/test/")]
    [TestCase("error///test", "/error/test/")]
    [TestCase("/error/test/", "/error/test/")]
    public void ErrorTypesCanBeNormalized(string original, string expected)
    {
        var normalized = Error.NormalizeType(original);

        normalized.Should().Be(expected);
    }

    [TestCase("/unknown/", typeof(Error))]
    [TestCase("//error/parse", typeof(ParseError))]
    [TestCase("/Error/PARSE", typeof(ParseError))]
    [TestCase("/error/", typeof(Error))]
    [TestCase("/error/arg/", typeof(ArgError))]
    [TestCase("/error/arg/unexpected", typeof(UnexpectedArgError))]
    [TestCase("/error/arg/missing/", typeof(MissingArgError))]
    [TestCase("/error/eval/", typeof(EvalError))]
    [TestCase("/error/index/", typeof(IndexError))]
    [TestCase("/error/io/", typeof(IoError))]
    [TestCase("/error/key/", typeof(KeyError))]
    [TestCase("/error/name/", typeof(NameError))]
    [TestCase("/error/parse/", typeof(ParseError))]
    [TestCase("/error/parse/missing/end_token/", typeof(MissingEndTokenError))]
    [TestCase("/error/type/", typeof(TypeError))]
    [TestCase("/error/value/", typeof(ValueError))]
    public void TheCorrectSubclassCanBeCreatedBySpecifyingTheErrorType(string type, Type expectedType)
    {
        var error = Error.Create(type, "A test error message.");

        error.Type.Should().Be(Error.NormalizeType(type).ToLowerInvariant());
        error.Message.Should().Be("A test error message.");
        error.GetType().Should().Be(expectedType);
    }

    [Test]
    public void ABreakCanBeCreated()
    {
        var error = Error.Create("/break", string.Empty, "test".ToValue());

        error.Type.Should().Be(Error.NormalizeType("/break").ToLowerInvariant());
        error.Message.Should().Be("Unexpected 'break' outside of loop.");
        error.Value.Should().Be(Value.Empty);
        error.GetType().Should().Be(typeof(Break));
    }

    [Test]
    public void AContinueCanBeCreated()
    {
         var error = Error.Create("/continue", string.Empty, "test".ToValue());

        error.Type.Should().Be(Error.NormalizeType("/continue").ToLowerInvariant());
        error.Message.Should().Be("Unexpected 'continue' outside of loop.");
        error.Value.Should().Be(Value.Empty);
        error.GetType().Should().Be(typeof(Continue));
    }

    [Test]
    public void AReturnCanBeCreated()
    {
        var error = Error.Create("/return", string.Empty, "test".ToValue());

        error.Type.Should().Be(Error.NormalizeType("/return").ToLowerInvariant());
        error.Message.Should().Be("Unexpected 'return' outside of def.");
        error.Value.Should().Be("test".ToValue());
        error.GetType().Should().Be(typeof(Return));
    }

    [TestCase("/error/", "/error/", true)]
    [TestCase("/error/", "/ERROR/", true)]
    [TestCase("/error/parse", "/error/", true)]
    [TestCase("/error/parse/missing/end_token", "", true)]
    [TestCase("/error/parse/missing/end_token", "/", true)]
    [TestCase("/error/parse/missing/end_token", "/error", true)]
    [TestCase("/error/parse/missing/end_token", "/error/parse", true)]
    [TestCase("/error/arg", "/error/parse", false)]
    [TestCase("/", "/error/parse", false)]
    public void AnErrorIsOfATypeIfStartsWithTheSamePath(string type, string typeQuery, bool expected)
    {
        var error = Error.Create(type, "N/A");

        var isType = error.IsType(typeQuery);

        isType.Should().Be(expected);
    }

    [TestCase(typeof(Exception), "I'm an exception", "/wobbly/exception/")]
    [TestCase(typeof(ArgumentException), "Bad argument!", "/wobbly/argument_exception/")]
    [TestCase(typeof(NotImplementedException), "I am really implemented!", "/wobbly/not_implemented_exception/")]
    public void AnNativeClrErrorCanBeCaughtAnConvertedToAJellyError(
        Type clrExceptionType, string message, string expectedJellyType)
    {
        var action = () => Error.RethrowUnhandledClrExceptionsAsJellyErrors(() =>
            throw (Exception)Activator.CreateInstance(clrExceptionType, message)!);

        action.Invoking(a => a()).Should().Throw<Error>().WithMessage(message).Where(e => e.Type == expectedJellyType);
    }

    [Test]
    public void AnJellyErrorIsNotCaughtAnnConvertedToAJellyError()
    {
        var action = () => Error.RethrowUnhandledClrExceptionsAsJellyErrors(() =>
            throw new Error("/error/", "Boo!"));

        action.Invoking(a => a()).Should().Throw<Error>().WithMessage("Boo!").Where(e => e.Type == "/error/");
    }
}