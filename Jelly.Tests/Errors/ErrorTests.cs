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
    [TestCase("/error/eval/", typeof(EvalError))]
    [TestCase("/error/name/", typeof(NameError))]
    [TestCase("/error/parse/", typeof(ParseError))]
    [TestCase("/error/parse/missing/end_token/", typeof(MissingEndTokenError))]
    [TestCase("/error/type/", typeof(TypeError))]
    [TestCase("/error/value/", typeof(ValueError))]
    public void TheCorrectSubclassCanBeCreatedBySpecifiyingTheErrorType(string type, Type expectedType)
    {
        var error = Error.Create(type, "A test error message.");

        error.Type.Should().Be(Error.NormalizeType(type).ToLowerInvariant());
        error.Message.Should().Be("A test error message.");
        error.GetType().Should().Be(expectedType);
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
}