namespace Jelly.Interpreter.Tests;

[TestFixture]
public class InterpreterTests
{
    [Test]
    public void TheInterpreterCallsTheConfiguredInterpreterForTheGivenNodeTypeAndTheInterpreterPassedToEvaluateIsPassedToTheRetrievedInterpretersEvaluateMethod()
    {
        var anotherInterpreter = new TestInterpreter();
        var interpreter = new Interpreter();
        var test1Interpreter = new TestInterpreter();
        var test2Interpreter = new TestInterpreter();
        interpreter.AddInterpreter("type1", test1Interpreter);
        interpreter.AddInterpreter("type2", test2Interpreter);
        var test1Node = new DictionaryValue(new KeyValuePair<Value, Value>[] {
            new(new StringValue("type"), new StringValue("type1")),
            new(new StringValue("message"), new StringValue("hi")),
        });
        var test2Node = new DictionaryValue(new KeyValuePair<Value, Value>[] {
            new(new StringValue("type"), new StringValue("type2")),
            new(new StringValue("message"), new StringValue("bye")),
        });

        var test1result = interpreter.Evaluate(test1Node, anotherInterpreter);
        var test2result = interpreter.Evaluate(test2Node, anotherInterpreter);
        
        test1result.Should().Be(new StringValue("hi"));
        test1Interpreter.NodeEvaluated.Should().Be(test1Node);
        test1Interpreter.InterpreterPassedToEvaluate.Should().Be(anotherInterpreter);
        test2result.Should().Be(new StringValue("bye"));
        test2Interpreter.NodeEvaluated.Should().Be(test2Node);
        test2Interpreter.InterpreterPassedToEvaluate.Should().Be(anotherInterpreter);
    }

    [Test]
    public void WhenAnInterpreterIsNotPassedToEvaluateTheInterpreterPassesItself()
    {
        var interpreter = new Interpreter();
        var testInterpreter = new TestInterpreter();
        interpreter.AddInterpreter("test", testInterpreter);
        var testNode = new DictionaryValue(new KeyValuePair<Value, Value>[] {
            new(new StringValue("type"), new StringValue("test")),
            new(new StringValue("message"), new StringValue("whoa!")),
        });

        var result = interpreter.Evaluate(testNode);
        
        testInterpreter.InterpreterPassedToEvaluate.Should().Be(interpreter);
    }

    public class TestInterpreter : IInterpreter
    {
        public DictionaryValue? NodeEvaluated { get; private set; }
        public IInterpreter? InterpreterPassedToEvaluate { get; private set; }

        public Value Evaluate(DictionaryValue node, IInterpreter interpreter)
        {
            NodeEvaluated = node;
            InterpreterPassedToEvaluate = interpreter;

            return node[new StringValue("message")];
        }
    }
}