namespace Jelly.Parser;

public class ExpressionParser : IParser
{
    public static readonly Value SubExpressionsKeyword = new StrValue("subexpressions");

    readonly WordParser _wordParser;

    public ExpressionParser(ScriptParser? subscriptParser = null)
    {
        _wordParser = new(ScannerConfig.Default.ExpressionEnd, subscriptParser, this, true);
    }

    public DictValue? Parse(Scanner scanner)
    {
        var start = scanner.Position;
        DictValue? functionName = null;
        var subExpressions = new List<DictValue>();
        var operators = new Stack<OperatorAndNode>();
        var operands = new Stack<DictValue>();

        operators.Push(new(Operator.None, Node.Literal(Value.Empty, scanner.Position, scanner.Position)));

        DictValue? prevWord = null;
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
                        if (op.IsBinaryOperator() && op != Operator.SubExpressionSeparator)
                        {
                            throw new ParseError("Unexpected operator in expression.") {
                                StartPosition = Node.GetStartPosition(word),
                                EndPosition = Node.GetEndPosition(word)
                            };
                        }
                    }

                    if (op == Operator.SubExpressionSeparator)
                    {
                        BuildSubExpression(true);
                    }
                    else
                    {
                        while (op.GetPrecedence() < operators.Peek().Operator.GetPrecedence())
                        {
                            PopOperator();
                        }
                        operators.Push(new(op, word));
                    }
                }
                else
                {
                    if (!prevWasOperator)
                    {
                        if (functionName is not null && Node.IsExpression(word))
                        {
                            operands.Push(Node.Command(functionName, word[SubExpressionsKeyword].ToListValue()));
                            functionName = null;
                        }
                        else
                        {
                            throw new ParseError("Unexpected value in expression.")
                            {
                                StartPosition = Node.GetStartPosition(word),
                                EndPosition = Node.GetEndPosition(word)
                            };
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
                prevWord = word;
            }

            BuildSubExpression();

            return Node.Expression(start, scanner.Position, subExpressions.ToArray());
        }

        return null;

        void BuildSubExpression(bool includeEmpty=false)
        {
            if ((prevWasOperator && operators.Count > 1) || functionName is not null)
            {
                throw new ParseError("Missing value after operator in expression.")
                {
                    StartPosition = prevWord is not null ? Node.GetEndPosition(prevWord) : -1,
                    EndPosition = prevWord is not null ? Node.GetEndPosition(prevWord) : -1,
                };
            }
            PopRemainingOperators();

            if (operands.Count == 0 && (subExpressions.Count > 0 || includeEmpty))
            {
                subExpressions.Add(Node.Literal(Value.Empty, scanner.Position - 1, scanner.Position - 1));
            }
            else if (operands.Count > 0)
            {
                subExpressions.Add(operands.Pop());
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
            var op = operators.Peek().Operator;
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
            var uniOp = Node.Reposition(Node.UniOp(op.Operator.GetName(), a), op.Node, a);
            operands.Push(uniOp);
        }

        void PopBinaryOperator()
        {
            var op = operators.Pop().Operator;
            var b = operands.Pop();
            var a = operands.Pop();
            operands.Push(Node.Reposition(Node.BinOp(op.GetName(), a, b), a, b));
        }
    }

    IEnumerable<DictValue> ParseWords(Scanner scanner)
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
            throw new MissingEndTokenError("Unexpected end-of-file.")
            {
                StartPosition = scanner.Position,
                EndPosition = scanner.Position
            };
        }
    }

    static bool IsOperator(DictValue word)
    {
        return Node.IsLiteral(word) && ScannerConfig.Default.OperatorNames.ContainsKey(Node.GetLiteralValue(word).ToString());
    }

    static Operator GetOperatorFromLiteral(DictValue word)
    {
        return ScannerConfig.Default.OperatorNames[Node.GetLiteralValue(word).ToString()];
    }

    class OperatorAndNode
    {
        public OperatorAndNode(Operator op, DictValue node)
        {
            Operator = op;
            Node = node;
        }

        public Operator Operator { get; }
        public DictValue Node { get; }
    }
}