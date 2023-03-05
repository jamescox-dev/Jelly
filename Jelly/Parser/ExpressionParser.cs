using Jelly.Ast;
using Jelly.Errors;
using Jelly.Parser.Scanning;
using Jelly.Values;

namespace Jelly.Parser;

public class ExpressionParser : IParser
{
    static readonly StringValue ValueKeyword = new StringValue("value");

    static readonly WordParser _wordParser = new();

    public DictionaryValue? Parse(Scanner scanner)
    {
        var words = new List<DictionaryValue>();
        var operators = new Stack<Operator>();
        var operands = new Stack<DictionaryValue>();

        operators.Push(Operator.None);

        if (scanner.AdvanceIf(s => s.IsExpressionBegin))
        {
            foreach (var word in ParseWords(scanner))
            {
                if (IsOperator(word))
                {
                    var op = GetOperatorFromLiteral(word);
                    while (op.GetPrecedence() < operators.Peek().GetPrecedence())
                    {
                        PopBinaryOperator();
                    }
                    operators.Push(op);
                }
                else
                {
                    operands.Push(word);
                }

                words.Add(word);
            }

            while (operators.Count > 1)
            {
                PopBinaryOperator();
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

        void PopBinaryOperator()
        {
            var top = operators.Pop();
            var b = operands.Pop();
            var a = operands.Pop();
            operands.Push(Node.BinOp(top.GetName(), a, b));
        }
    }

    static IEnumerable<DictionaryValue> ParseWords(Scanner scanner)
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