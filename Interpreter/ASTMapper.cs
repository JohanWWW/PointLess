using Antlr4.Runtime;
using Interpreter.Models;
using Interpreter.Models.Enums;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Singulink.Numerics;

namespace Interpreter
{
    public class ASTMapper
    {
        private readonly IReadOnlyDictionary<string, IFunctionModel> _nativeImplementations;

        private static readonly Regex _integerNumberPattern = new Regex(@"^\d+$");
        private static readonly Regex _decimalNumberPattern = new Regex(@"^\d*\.\d+$");

        public ASTMapper(params NativeImplementationBase[] implementations)
        {
            _nativeImplementations = implementations.SelectMany(i => i.GetImplementationMap()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public ASTMapper()
        {
            _nativeImplementations = null;
        }

        /// <summary>
        /// Parses input string and generates an abstract syntax tree of the entire program
        /// </summary>
        /// <param name="program">The input program</param>
        /// <returns></returns>
        public RootModel ToAST(string program)
        {
            ICharStream charStream = new AntlrInputStream(program);
            ITokenSource tokenSource = new ZeroPointLexer(charStream);
            ITokenStream tokenStream = new CommonTokenStream(tokenSource);

            var parser = new ZeroPointParser(tokenStream);

            // Listen for syntax errors
            var errorListener = new ParserErrorListener();
            parser.AddErrorListener(errorListener);
            
            return EnterRoot(parser.root());
        }

        private RootModel EnterRoot(ZeroPointParser.RootContext context)
        {
            var statements = new List<IStatementModel>();

            if (context.use_statement() != null)
            {
                foreach (var s in context.use_statement())
                {
                    statements.Add(new UseStatementModel
                    {
                        PathToNamespace = new[] { s.IDENTIFIER()[0].GetText() },
                        StartToken = s.Start,
                        StopToken = s.Stop
                    });
                }
            }

            foreach (var s in context.statement())
            {
                statements.Add(EnterStatement(s));
            }

            return new RootModel
            {
                Statements = statements
            };
        }

        private IStatementModel EnterStatement(ZeroPointParser.StatementContext context)
        {
            if (context.assign_statement() != null)
            {
                return EnterAssignStatement(context.assign_statement());
            }

            if (context.conditional_statement() != null)
            {
                return EnterConditionalStatement(context.conditional_statement());
            }

            if (context.function_call_statement() != null)
            {
                return EnterFunctionCall(context.function_call_statement());
            }

            if (context.loop_statement() != null)
            {
                return EnterLoopStatement(context.loop_statement());
            }

            if (context.try_catch_statement() != null)
            {
                return EnterTryCatchStatement(context.try_catch_statement());
            }

            if (context.throw_statement() != null)
            {
                return EnterThrowStatement(context.throw_statement());
            }

            throw new NotImplementedException();
        }

        private IStatementModel EnterAssignStatement(ZeroPointParser.Assign_statementContext context)
        {
            // Standalone identifier
            if (context.IDENTIFIER() != null)
            {
                return new AssignStatementModel
                {
                    Identifier = new[] { context.IDENTIFIER().GetText() },
                    OperatorCombination = EnterAssignmentOperator(context.assignment_operator()),
                    Assignee = EnterExpression(context.expression()),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }

            // Path to identifier
            return new AssignStatementModel
            {
                Identifier = context.identifier_access().IDENTIFIER().Select(i => i.GetText()).ToArray(),
                OperatorCombination = EnterAssignmentOperator(context.assignment_operator()),
                Assignee = EnterExpression(context.expression()),
                StartToken = context.Start,
                StopToken = context.Stop
            };
        }

        private AssignmentOperator EnterAssignmentOperator(ZeroPointParser.Assignment_operatorContext context)
        {
            // Declare/Update referenced value (=)
            if (context.ASSIGN() != null)
                return AssignmentOperator.Assign;

            // Variants of the assignment operator should only work on existing variables (<operator>=)
            if (context.ADD_ASSIGN() != null)
                return AssignmentOperator.AddAssign;

            if (context.SUB_ASSIGN() != null)
                return AssignmentOperator.SubAssign;

            if (context.MULT_ASSIGN() != null)
                return AssignmentOperator.MultAssign;

            if (context.DIV_ASSIGN() != null)
                return AssignmentOperator.DivAssign;

            if (context.MOD_ASSIGN() != null)
                return AssignmentOperator.ModAssign;

            if (context.AND_ASSIGN() != null)
                return AssignmentOperator.AndAssign;

            if (context.XOR_ASSIGN() != null)
                return AssignmentOperator.XorAssign;

            if (context.OR_ASSIGN() != null)
                return AssignmentOperator.OrAssign;

            if (context.BITWISE_AND_ASSIGN() != null)
                return AssignmentOperator.BitwiseAndAssign;

            if (context.BITWISE_XOR_ASSIGN() != null)
                return AssignmentOperator.BitwiseXorAssign;

            if (context.BITWISE_OR_ASSIGN() != null)
                return AssignmentOperator.BitwiseOrAssign;

            if (context.SHIFT_LEFT_ASSIGN() != null)
                return AssignmentOperator.ShiftLeftAssign;

            if (context.SHIFT_RIGHT_ASSIGN() != null)
                return AssignmentOperator.ShiftRightAssign;

            throw new NotImplementedException("That operator is not supported.");
        }

        private ConditionalStatementModel EnterConditionalStatement(ZeroPointParser.Conditional_statementContext context)
        {
            var ifStatement = new IfStatementModel
            {
                Condition = EnterExpression(context.if_statement().expression()),
                Body = EnterBlock(context.if_statement().block()),
            };

            List<ElseIfStatementModel> elseIfStatement = null;
            if (context.else_if_statement() != null)
            {
                elseIfStatement = new List<ElseIfStatementModel>();

                foreach (var ei in context.else_if_statement())
                {
                    elseIfStatement.Add(new ElseIfStatementModel
                    {
                        Condition = EnterExpression(ei.expression()),
                        Body = EnterBlock(ei.block())
                    });
                }
            }

            ElseStatementModel elseStatement = null;
            if (context.else_statement() != null)
            {
                elseStatement = new ElseStatementModel
                {
                    Body = EnterBlock(context.else_statement().block())
                };
            }

            return new ConditionalStatementModel
            {
                If = ifStatement,
                ElseIf = elseIfStatement,
                Else = elseStatement,
                StartToken = context.Start,
                StopToken = context.Stop
            };
        }

        private ILoopStatementModel EnterLoopStatement(ZeroPointParser.Loop_statementContext context)
        {
            if (context.while_loop_statement() != null)
            {
                return EnterWhileLoop(context.while_loop_statement());
            }

            throw new NotImplementedException();
        }

        private WhileLoopStatement EnterWhileLoop(ZeroPointParser.While_loop_statementContext context)
        {
            return new WhileLoopStatement
            {
                Condition = EnterExpression(context.expression()),
                Body = EnterBlock(context.block()),
                StartToken = context.Start,
                StopToken = context.Stop
            };
        }

        private TryCatchStatementModel EnterTryCatchStatement(ZeroPointParser.Try_catch_statementContext context)
        {
            TryStatement tryStatement = EnterTryStatement(context.try_statement());
            CatchStatement catchStatement = EnterCatchStatment(context.catch_statement());

            return new TryCatchStatementModel
            {
                Try = tryStatement,
                Catch = catchStatement,
                StartToken = context.Start,
                StopToken = context.Stop
            };
        }

        private TryStatement EnterTryStatement(ZeroPointParser.Try_statementContext context)
        {
            return new TryStatement
            {
                Body = EnterBlock(context.block())
            };
        }

        private CatchStatement EnterCatchStatment(ZeroPointParser.Catch_statementContext context)
        {
            return new CatchStatement
            {
                ArgumentName = context.IDENTIFIER().GetText(),
                Body = EnterBlock(context.block())
            };
        }

        private ThrowStatement EnterThrowStatement(ZeroPointParser.Throw_statementContext context)
        {
            return new ThrowStatement
            {
                Expression = EnterExpression(context.expression()),
                StartToken = context.Start,
                StopToken = context.Stop
            };
        }

        private IExpressionModel EnterExpression(ZeroPointParser.ExpressionContext context)
        {
            if (context.anonymous_function_definition_statement() != null)
            {
                return EnterFunctionDefinitionExpression(context.anonymous_function_definition_statement());
            }

            if (context.function_call_statement() != null)
            {
                return EnterFunctionCall(context.function_call_statement());
            }

            if (context.IDENTIFIER() != null)
            {
                return new IdentifierExpressionModel
                {
                    Identifier = new[] { context.IDENTIFIER().GetText() },
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }

            if (context.identifier_access() != null)
            {
                return new IdentifierExpressionModel
                {
                    Identifier = context.identifier_access().IDENTIFIER().Select(i => i.GetText()).ToArray(),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }

            if (context.literal() != null)
            {
                return EnterLiteral(context.literal());
            }

            if (context.object_initialization_expression() != null)
            {
                return EnterObjectInitialization(context.object_initialization_expression());
            }

            return EnterExpressionExpression(context);
        }

        /// <summary>
        /// Unary or binary expression
        /// </summary>
        private IExpressionModel EnterExpressionExpression(ZeroPointParser.ExpressionContext context)
        {
            if (context.expression().Length is 1)
            {
                var e = context.expression()[0];
                return EnterExpression(e);
            }

            // Binary expression
            var left = context.expression()[0];
            var right = context.expression()[1];

            if (context.AND() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.LogicalAnd,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.BITWISE_AND() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.BitwiseAnd,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.BITWISE_OR() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.BitwiseOr,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.BITWISE_XOR() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.BitwiseXOr,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.DIV() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.Div,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.EQUAL() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.Equal,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.GREATER_THAN() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.GreaterThan,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.GREATER_THAN_OR_EQUAL() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.GreaterThanOrEqual,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.LESS_THAN() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.LessThan,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.LESS_THAN_OR_EQUAL() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.LessThanOrEqual,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.MINUS() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.Minus,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.MOD() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.Mod,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.MULT() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.Mult,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.NOTEQUAL() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.NotEqual,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.OR() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.LogicalOr,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.PLUS() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.Plus,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.SHIFT_LEFT() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.ShiftLeft,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.SHIFT_RIGHT() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.ShiftRight,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private IExpressionModel EnterFunctionDefinitionExpression(ZeroPointParser.Anonymous_function_definition_statementContext context)
        {
            if (context.function_statement() != null)
            {
                return EnterFunctionStatement(context.function_statement());
            }

            if (context.action_statement() != null)
            {
                return EnterActionStatement(context.action_statement());
            }

            if (context.consumer_statement() != null)
            {
                return EnterConsumerStatement(context.consumer_statement());
            }

            if (context.provider_statement() != null)
            {
                return EnterProviderStatement(context.provider_statement());
            }

            if (context.native_function_statement() != null)
            {
                return EnterNativeFunctionStatement(context.native_function_statement());
            }

            if (context.native_provider_statement() != null)
            {
                return EnterNativeProviderStatement(context.native_provider_statement());
            }

            throw new NotImplementedException();
        }

        private string[] EnterIdentifierAccess(ZeroPointParser.Identifier_accessContext context) =>
            context.IDENTIFIER().Select(i => i.GetText()).ToArray();

        private LiteralExpressionModel EnterLiteral(ZeroPointParser.LiteralContext context)
        {
            var literalExpression = new LiteralExpressionModel
            {
                StartToken = context.Start,
                StopToken = context.Stop
            };

            if (context.BOOLEAN() != null)
            {
                literalExpression.Value = bool.Parse(context.BOOLEAN().GetText());
                return literalExpression;
            }

            if (context.NULL() != null)
            {
                literalExpression.Value = null;
                return literalExpression;
            }

            if (context.NUMBER() != null)
            {
                string numberText = context.NUMBER().GetText();

                // Order might be important!
                if (_decimalNumberPattern.IsMatch(numberText))
                {
                    literalExpression.Value = BigDecimal.Parse(numberText, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture);
                    return literalExpression;
                }
                else if (_integerNumberPattern.IsMatch(numberText))
                {
                    literalExpression.Value = BigInteger.Parse(numberText);
                    return literalExpression;
                }
                else throw new FormatException($"Could not recognize {nameof(ZeroPointParser.NUMBER)}: {numberText}");
            }

            if (context.STRING() != null)
            {
                string s = context.STRING().GetText();

                var stringBuilder = new StringBuilder();

                if (s.Length is 2) // is empty string: ""
                    stringBuilder.Append(string.Empty);
                else
                {
                    for (int i = 1; i < s.Length - 1; i++)
                        stringBuilder.Append(s[i]);
                }

                // Parse escape characters
                stringBuilder.Replace("\\\"", "\"");
                stringBuilder.Replace("\\\\", "\\");
                stringBuilder.Replace("\\n", "\n");
                stringBuilder.Replace("\\t", "\t");

                literalExpression.Value = stringBuilder.ToString();
                return literalExpression;
            }

            throw new NotImplementedException();
        }

        private FunctionStatementModel EnterFunctionStatement(ZeroPointParser.Function_statementContext context)
        {
            return new FunctionStatementModel
            {
                Parameters = new ParameterListModel { Parameters = context.parameter_list().IDENTIFIER().Select(i => i.GetText()).ToArray() },
                Body = EnterBlock(context.block()),
                Return = EnterExpression(context.expression()),
                StartToken = context.Start,
                StopToken = context.Stop
            };
        }

        private ActionStatementModel EnterActionStatement(ZeroPointParser.Action_statementContext context)
        {
            return new ActionStatementModel
            {
                Body = EnterBlock(context.block()),
                StartToken = context.Start,
                StopToken = context.Stop
            };
        }

        private ConsumerStatementModel EnterConsumerStatement(ZeroPointParser.Consumer_statementContext context)
        {
            return new ConsumerStatementModel
            {
                Parameters = new ParameterListModel { Parameters = context.parameter_list().IDENTIFIER().Select(i => i.GetText()).ToArray() },
                Body = EnterBlock(context.block()),
                StartToken = context.Start,
                StopToken = context.Stop
            };
        }

        private ProviderStatementModel EnterProviderStatement(ZeroPointParser.Provider_statementContext context)
        {
            return new ProviderStatementModel
            {
                Body = EnterBlock(context.block()),
                Return = EnterExpression(context.expression()),
                StartToken = context.Start,
                StopToken = context.Stop
            };
        }

        private NativeProviderStatementModel EnterNativeProviderStatement(ZeroPointParser.Native_provider_statementContext context)
        {
            string implementationIdentifier = EnterInjectStatement(context.inject_statement());
            var np = _nativeImplementations[implementationIdentifier] as NativeProviderStatementModel;
            np.StartToken = context.Start;
            np.StopToken = context.Stop;
            return np;
        }

        private NativeFunctionStatementModel EnterNativeFunctionStatement(ZeroPointParser.Native_function_statementContext context)
        {
            string implementationIdentifier = EnterInjectStatement(context.inject_statement());
            var nf = _nativeImplementations[implementationIdentifier] as NativeFunctionStatementModel;
            nf.StartToken = context.Start;
            nf.StopToken = context.Stop;
            return nf;
        }

        private string EnterInjectStatement(ZeroPointParser.Inject_statementContext context)
        {
            string implIdentifier = context.STRING().GetText();
            var sb = new StringBuilder();
            for (int i = 1; i < implIdentifier.Length - 1; i++)
                sb.Append(implIdentifier[i]);
            return sb.ToString();
        }

        private BlockModel EnterBlock(ZeroPointParser.BlockContext context)
        {
            if (context.statement() is null)
            {
                return new BlockModel();
            }

            var statements = new List<IStatementModel>();

            foreach (var s in context.statement())
            {
                statements.Add(EnterStatement(s));
            }

            return new BlockModel { Statements = statements };
        }

        private FunctionCallStatement EnterFunctionCall(ZeroPointParser.Function_call_statementContext context)
        {
            if (context.IDENTIFIER() != null)
            {
                return new FunctionCallStatement
                {
                    IdentifierPath = new[] { context.IDENTIFIER().GetText() },
                    Arguments = context.argument_list() != null ? EnterArgumentList(context.argument_list()) : null,
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }

            return new FunctionCallStatement
            {
                IdentifierPath = EnterIdentifierAccess(context.identifier_access()),
                Arguments = context.argument_list() != null ? EnterArgumentList(context.argument_list()) : null,
                StartToken = context.Start,
                StopToken = context.Stop
            };
        }

        private ArgumentListModel EnterArgumentList(ZeroPointParser.Argument_listContext context)
        {
            var expressions = new List<IExpressionModel>();

            foreach (var e in context.expression())
            {
                expressions.Add(EnterExpression(e));
            }

            return new ArgumentListModel
            {
                Arguments = expressions
            };
        }

        private ObjectInitializationExpressionModel EnterObjectInitialization(ZeroPointParser.Object_initialization_expressionContext context)
        {
            if (context.assign_statement() is null)
            {
                return new ObjectInitializationExpressionModel
                {
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }

            var properties = new List<ObjectPropertyExpressionModel>();

            foreach (var s in context.assign_statement())
            {
                properties.Add(new ObjectPropertyExpressionModel
                {
                    Identifier = s.IDENTIFIER().GetText(),
                    Value = EnterExpression(s.expression())
                });
            }

            return new ObjectInitializationExpressionModel
            {
                Properties = properties,
                StartToken = context.Start,
                StopToken = context.Stop
            };
        }
    }
}
