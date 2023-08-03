namespace Jelly.Evaluator.Tests;

[TestFixture]
public class AssignmentEvaluatorTests : EvaluatorTestsBase
{
    [Test]
    public void TheCorrectVariableIsSetInTheScope()
    {
        var assignment = Node.Assignment("name", Node.Literal("Bob".ToValue()));

        Evaluate(assignment);

        Environment.GlobalScope.GetVariable("name").Should().Be("Bob".ToValue());
    }

    [Test]
    public void TheResultOfTheAssignmentIsTheValueOfTheAssignment()
    {
        var assignment = Node.Assignment("name", Node.Literal("Bob".ToValue()));

        var result = Evaluate(assignment);

        result.Should().Be("Bob".ToValue());
    }

    [Test]
    public void AssignmentNodeWithAListIndexerEvaluatesItsExpressionAndUsesItAsAnIndexIntoTheListValueReturnedFromTheVariableAndCreatesANewListWithTheIndexedItemSetToTheValueAndThatValueIsReturned()
    {
        var assignment = Node.Assignment(0, 0, "list", Node.Literal("x".ToValue()), Node.ListIndexer(0, 0, Node.Literal(3)));

        var result = Evaluate(assignment);

        result.Should().Be("x".ToValue());
        Environment.GlobalScope.GetVariable("list").Should().Be(new ListValue(1.ToValue(), 2.ToValue(), "x".ToValue()));
    }

    [Test]
    public void AssignmentNodeWithADictIndexerEvaluatesItsExpressionAndUsesItAsAnIndexIntoTheListValueReturnedFromTheVariableAndCreatesANewListWithTheIndexedItemSetToTheValueAndThatValueIsReturned()
    {
        var assignment = Node.Assignment(0, 0, "dict", Node.Literal("y".ToValue()), Node.DictIndexer(0, 0, Node.Literal("b")));

        var result = Evaluate(assignment);

        result.Should().Be("y".ToValue());
        Environment.GlobalScope.GetVariable("dict").Should().Be(new DictValue("a".ToValue(), 1.ToValue(), "b".ToValue(), "y".ToValue()));
    }

    [Test]
    public void AssignmentNodeIndexersCanBeChained()
    {
        var assignment = Node.Assignment(0, 0, "nest", Node.Literal("result!".ToValue()),
            Node.DictIndexer(0, 0, Node.Literal("b")),
            Node.ListIndexer(0, 0, Node.Literal(2)),
            Node.DictIndexer(0, 0, Node.Literal("z")));

        var result = Evaluate(assignment);

        result.Should().Be("result!".ToValue());
        Environment.GlobalScope.GetVariable("nest").Should().Be(new DictValue(
            "a".ToValue(), 1.ToValue(),
            "b".ToValue(), new ListValue(
                0.ToValue(),
                new DictValue("z".ToValue(), "result!".ToValue()))));
    }

    public override void Setup()
    {
        base.Setup();
        Environment.GlobalScope.DefineVariable("name", "Jeff".ToValue());
        Environment.GlobalScope.DefineVariable("list", new ListValue(1.ToValue(), 2.ToValue(), 3.ToValue()));
        Environment.GlobalScope.DefineVariable("dict", new DictValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue()));
        Environment.GlobalScope.DefineVariable("nest", new DictValue(
            "a".ToValue(), 1.ToValue(),
            "b".ToValue(), new ListValue(
                0.ToValue(),
                new DictValue("z".ToValue()))));
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new AssignmentEvaluator();
    }
}