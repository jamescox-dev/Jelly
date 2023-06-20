using Jelly.Experimental;
using Jelly.Commands.ArgParsers;

var parser = new SimpleArgParser(new Arg("name"));

var args = new List<Value>
{
    "Alfie".ToValue(),
};

var result = parser.Parse("test", args, );

Console.WriteLine("hello, world");