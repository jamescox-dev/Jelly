using Jelly.Ast;
using Jelly.Errors;
using Jelly.Parser.Scanning;
using Jelly.Values;

namespace Jelly.Parser;

public class ExpressionParser : IParser
{
    static readonly StringValue ValueKeyword = new StringValue("value");

    readonly WordParser _wordParser;

    public ExpressionParser(ScriptParser? subscriptParser = null)
    {
        _wordParser = new(ScannerConfig.Default.ExpressionEnd, subscriptParser, this);
    }

    public DictionaryValue? Parse(Scanner scanner)
    {
        var words = new List<DictionaryValue>();
        var operators = new Stack<Operator>();
        var operands = new Stack<DictionaryValue>();

        operators.Push(Operator.None);

        if (scanner.AdvanceIf(s => s.IsExpressionBegin))
        {
            var prevWasOperator = true;
            foreach (var word in ParseWords(scanner))
            {
                var isOperator = IsOperator(word);
                if (isOperator)
                {
                    var op = GetOperatorFromLiteral(word);
                    if (prevWasOperator)
                    {
                        if (op == Operator.Add)
                        {
                            op = Operator.Positive;
                        }
                        else if (op == Operator.Subtract)
                        {
                            op = Operator.Negative;
                        }
                        if (op.IsBinaryOperator())
                        {
                            throw Error.Parse("Invalid expression.");
                        }
                    }
                    
                    while (op.GetPrecedence() < operators.Peek().GetPrecedence())
                    {
                        PopOperator();
                    }
                    operators.Push(op);
                }
                else
                {
                    if (!prevWasOperator)
                    {
                        throw Error.Parse("Invalid expression.");
                    }
                    operands.Push(word);
                }

                words.Add(word);
                prevWasOperator = isOperator;
            }

            while (operators.Count > 1)
            {
                PopOperator();
            }

            if (operands.Count == 0)
            {
                return Node.Expression(Node.Literal(Value.Empty));
            }
            else
            {
                return Node.Expression(operands.Pop());
            }
        }

        return null;

        void PopOperator()
        {
            var op = operators.Peek();
            if (op.IsBinaryOperator())
            {
                PopBinaryOperator();
            }
            else
            {
                PopUnaryOperator();
            }
        }

        void PopUnaryOperator()
        {
            var op = operators.Pop();
            var a = operands.Pop();
            operands.Push(Node.UniOp(op.GetName(), a));
        }

        void PopBinaryOperator()
        {
            var op = operators.Pop();
            var b = operands.Pop();
            var a = operands.Pop();
            operands.Push(Node.BinOp(op.GetName(), a, b));
        }
    }

    IEnumerable<DictionaryValue> ParseWords(Scanner scanner)
    {
        var endFound = false;
        while (!scanner.IsEof)
        {
            scanner.AdvanceWhile(s => s.IsExpressionWordSeparator);
            var word = _wordParser.Parse(scanner);
            if (word is not null)
            {
                yield return word;
            }
            scanner.AdvanceWhile(s => s.IsExpressionWordSeparator);
            if (scanner.AdvanceIf(s => s.IsExpressionEnd))
            {
                endFound = true;
                break;
            }
        }
        
        if (!endFound)
        {
            throw new MissingEndTokenError("Unexpected end-of-file.");
        }
    }

    static bool IsOperator(DictionaryValue word)
    {
        return Node.IsLiteral(word) && ScannerConfig.Default.OperatorNames.ContainsKey(Node.GetLiteralValue(word).ToString());
    }

    static Operator GetOperatorFromLiteral(DictionaryValue word)
    {
        return ScannerConfig.Default.OperatorNames[Node.GetLiteralValue(word).ToString()];
    }
}