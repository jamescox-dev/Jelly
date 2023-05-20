




namespace Jelly.Parser;

public class ExpressionParser : IParser
{
    public static readonly Value SubexpressionsKeyword = new StringValue("subexpressions");

    readonly WordParser _wordParser;

    public ExpressionParser(ScriptParser? subscriptParser = null)
    {
        _wordParser = new(ScannerConfig.Default.ExpressionEnd, subscriptParser, this, true);
    }

    public DictionaryValue? Parse(Scanner scanner)
    {
        DictionaryValue? functionName = null;
        var subexpressions = new List<DictionaryValue>();
        var operators = new Stack<Operator>();
        var operands = new Stack<DictionaryValue>();

        operators.Push(Operator.None);

        var prevWasOperator = true;
        if (scanner.AdvanceIf(s => s.IsExpressionBegin))
        {
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
                        if (op.IsBinaryOperator() && op != Operator.SubexpressionSeparator)
                        {
                            throw Error.Parse("Invalid expression.");
                        }
                    }

                    if (op == Operator.SubexpressionSeparator)
                    {
                        BuildSubexpression(true);
                    }
                    else
                    {
                        while (op.GetPrecedence() < operators.Peek().GetPrecedence())
                        {
                            PopOperator();
                        }
                        operators.Push(op);
                    }
                }
                else
                {
                    if (!prevWasOperator)
                    {
                        if (functionName is not null && Node.IsExprssion(word))
                        {
                            operands.Push(Node.Command(functionName, word[SubexpressionsKeyword].ToListValue()));
                            functionName = null;
                        }
                        else
                        {
                            throw Error.Parse("Invalid expression.");
                        }
                    }
                    else
                    {
                        if (functionName is null && Node.IsLiteral(word) && double.IsNaN(Node.GetLiteralValue(word).ToDouble()))
                        {
                            functionName = word;
                        }
                        else
                        {
                            operands.Push(word);
                        }
                    }
                }

                prevWasOperator = isOperator;
            }

            BuildSubexpression();

            return Node.Expression(subexpressions.ToArray());
        }

        return null;

        void BuildSubexpression(bool includeEmpty=false)
        {
            if ((prevWasOperator && operators.Count > 1) || functionName is not null)
            {
                throw Error.Parse("Invalid expression.");
            }
            PopRemainingOperators();

            if (operands.Count == 0 && (subexpressions.Count > 0 || includeEmpty))
            {
                subexpressions.Add(Node.Literal(Value.Empty));
            }
            else if (operands.Count > 0)
            {
                subexpressions.Add(operands.Pop());
            }
        }

        void PopRemainingOperators()
        {
            while (operators.Count > 1)
            {
                PopOperator();
            }
        }

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