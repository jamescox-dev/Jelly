using Jelly.Experimental;
using Jelly.Commands.ArgParsers;
using System.Text.Json;

var parser = new SeqArgParser(
    new ArgParser("key"), 
    new ArgParser("value"), 
    new ArgParser("of"), 
    new ArgParser("dict"), 
    new ArgParser("body")
);

var result = parser.Parse(new ListValue(
    "k".ToValue(), "v".ToValue(), Node.Literal("of"), "d".ToValue(), Node.Script()));

Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions {
    WriteIndented = true
}));