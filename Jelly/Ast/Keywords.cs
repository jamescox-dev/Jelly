namespace Jelly.Ast;

using Jelly.Values;

public static class Keywords
{
    public static readonly StringValue A = new StringValue("a");
    public static readonly StringValue Args = new StringValue("args");
    public static readonly StringValue ArgNames = new StringValue("argnames");
    public static readonly StringValue ArgDefaults = new StringValue("argdefaults");
    public static readonly StringValue Assignment = new StringValue("assignment");
    public static readonly StringValue B = new StringValue("b");
    public static readonly StringValue BinOp = new StringValue("binop");
    public static readonly StringValue Body = new StringValue("body");
    public static readonly StringValue Command = new StringValue("command");
    public static readonly StringValue Commands = new StringValue("commands");
    public static readonly StringValue Composite = new StringValue("composite");
    public static readonly StringValue Condition = new StringValue("condition");
    public static readonly StringValue DefineVariable = new StringValue("defvariable");
    public static readonly StringValue DefineCommand = new StringValue("defcommand");
    public static readonly StringValue Else = new StringValue("else");
    public static readonly StringValue ErrorType = new StringValue("errortype");
    public static readonly StringValue Expression = new StringValue("expression");
    public static readonly StringValue ErrorHandlers = new StringValue("errorhandlers");
    public static readonly StringValue Finally = new StringValue("finally");
    public static readonly StringValue If = new StringValue("if");
    public static readonly StringValue Literal = new StringValue("literal");
    public static readonly StringValue Message = new StringValue("message");
    public static readonly StringValue Name = new StringValue("name");
    public static readonly StringValue Op = new StringValue("op");
    public static readonly StringValue Parts = new StringValue("parts");
    public static readonly StringValue Raise = new StringValue("raise");
    public static readonly StringValue RestArgName = new StringValue("restargname");
    public static readonly StringValue Scope = new StringValue("scope");
    public static readonly StringValue Script = new StringValue("script");
    public static readonly StringValue Subexpresions = new StringValue("subexpressions");
    public static readonly StringValue Then = new StringValue("then");
    public static readonly StringValue Try = new StringValue("try");
    public static readonly StringValue Type = new StringValue("type");
    public static readonly StringValue UniOp = new StringValue("uniop");
    public static readonly StringValue Value = new StringValue("value");
    public static readonly StringValue Variable = new StringValue("variable");
    public static readonly StringValue While = new StringValue("while");
}