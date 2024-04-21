namespace Jelly.Tests.Commands.ArgParsers;

[TestFixture]
public class StandardArgParserTests
{
    [Test]
    public void AParserConfiguredWithNoArgumentsReturnsAnEmptyDictionaryWhenPassedNoArguments()
    {
        var parser = new StandardArgParser();

        var parsedArgs = parser.Parse("test1", ListValue.EmptyList);

        parsedArgs.Should().Be(DictValue.EmptyDictionary);
    }

    [Test]
    public void AParserConfiguredWithNoArgumentsThrowsAnExceptionWhenPassedAnyArguments()
    {
        var parser = new StandardArgParser();

        parser.Invoking(p => p.Parse("test2", new ListValue(Node.Literal("a"))))
            .Should().Throw<UnexpectedArgError>()
            .WithMessage("test2 takes 0 arguments but 1 was given.");
    }

    [Test]
    public void AParserConfiguredWithOneRequiredArgumentsThrowsAnExceptionWhenNotPassedAnyArguments()
    {
        var parser = new StandardArgParser(new Arg("first"));

        parser.Invoking(p => p.Parse("test3", ListValue.EmptyList))
            .Should().Throw<MissingArgError>()
            .WithMessage("test3 missing 1 required argument:  first.");
    }

    [Test]
    public void AParserConfiguredWithTwoRequiredArgumentsThrowsAnExceptionWhenNotPassedAnyArguments()
    {
        var parser = new StandardArgParser(new Arg("first"), new Arg("second"));

        parser.Invoking(p => p.Parse("test4", ListValue.EmptyList))
            .Should().Throw<MissingArgError>()
            .WithMessage("test4 missing 2 required arguments:  first, and second.");
    }

    [Test]
    public void AParserConfiguredWithTwoRequiredArgumentsThrowsAnExceptionWhenPassedOnlyOneArguments()
    {
        var parser = new StandardArgParser(new Arg("first"), new Arg("second"));

        parser.Invoking(p => p.Parse("test5", new ListValue(Node.Literal("a"))))
            .Should().Throw<MissingArgError>()
            .WithMessage("test5 missing 1 required argument:  second.");
    }

    [Test]
    public void AParserConfiguredWithTwoRequiredArgumentsAndPassedTwoArgumentsReturnsADictionaryWithAnEntryForEachArgument()
    {
        var parser = new StandardArgParser(new Arg("a"), new Arg("b"));

        var args = parser.Parse("test", new ListValue(Node.Literal("1st"), Node.Literal("2nd")));

        args.Should().Be(new DictValue(
            "a".ToValue(), Node.Literal("1st"),
            "b".ToValue(), Node.Literal("2nd")
        ));
    }

    [Test]
    public void AParserConfiguredWithTwoOptionalArgumentsAndPassedNoArgumentsReturnsADictionaryWithAnEntryForEachOptionalArgumentSetToThereDefaultValues()
    {
        var parser = new StandardArgParser(new OptArg("optional"), new OptArg("also_optional", "DEFAULT".ToValue()));

        var args = parser.Parse("test_optionals", new ListValue());

        args.Should().Be(new DictValue(
            "optional".ToValue(), Value.Empty,
            "also_optional".ToValue(), "DEFAULT".ToValue()
        ));
    }

    [Test]
    public void AParserConfiguredWithTwoOptionalArgumentsAndPassedOneArgumentsReturnsADictionaryWithAnEntryForTheFirstArgumentValueAndAnEntryForTheSecondArgumentSetToItsDefaultValue()
    {
        var parser = new StandardArgParser(new OptArg("opt1"), new OptArg("opt2", "that".ToValue()));

        var args = parser.Parse("takes_options", new ListValue("this".ToValue()));

        args.Should().Be(new DictValue(
            "opt1".ToValue(), "this".ToValue(),
            "opt2".ToValue(), "that".ToValue()
        ));
    }

    [Test]
    public void AParserConfiguredWithOneOptionalArgumentsAndPassedMoreThanOneArgumentsThrowsAnExceptionWhenPassedAnyArguments()
    {
        var parser = new StandardArgParser(new OptArg("opt1"));

        parser.Invoking(p => p.Parse("command", new ListValue(Node.Literal("a"), Node.Literal("b"))))
            .Should().Throw<UnexpectedArgError>()
            .WithMessage("command takes from 0 to 1 argument but 2 were given.");
    }

    [Test]
    public void AParserConfiguredWithARestArgumentsReceivesAllArgumentValuesAsAListUnderTheNameOfTheRestArgument()
    {
        var parser = new StandardArgParser(new RestArg("and_the_rest..."));
        var passedArgs = new ListValue(true.ToValue(), 2.ToValue(), "three".ToValue());

        var args = parser.Parse("test_the_rest", passedArgs);

        args.Should().Be(new DictValue(
            "and_the_rest...".ToValue(), passedArgs
        ));
    }

    [Test]
    public void AParserConfiguredWithARestArgumentOnlyProcessesArgumentsAfterTheRequiredAndOptionalArgumentsHaveBeenConsumed()
    {
        var parser = new StandardArgParser(new Arg("req"), new OptArg("opt"), new RestArg("rest"));
        var passedArgs = new ListValue(true.ToValue(), 2.ToValue(), "three".ToValue());

        var args = parser.Parse("test_the_rest", passedArgs);

        args.Should().Be(new DictValue(
            "req".ToValue(), true.ToValue(),
            "opt".ToValue(), 2.ToValue(),
            "rest".ToValue(), new ListValue("three".ToValue())
        ));
    }
}