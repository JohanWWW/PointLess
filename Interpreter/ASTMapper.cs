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
using Interpreter.Runtime;
using Interpreter.Helpers;
using Antlr4.Runtime.Tree;

namespace Interpreter
{
    public class ASTMapper
    {
        private readonly IReadOnlyDictionary<string, IFunctionModel> _nativeImplementations;

        private static readonly Regex _integerNumberPattern = new(@"^\d+$");
        private static readonly Regex _integerBinaryPattern = new(@"^0b[01]+$");
        private static readonly Regex _integerHexPattern    = new(@"^0x([0-9]|[a-fA-F])+$");
        private static readonly Regex _decimalNumberPattern = new(@"^\d*\.\d+$");
        private static readonly Regex _ubytePattern         = new(@"^b'([01]?[0-9][0-9]?|2[0-4][0-9]|25[0-5])$");
        private static readonly Regex _ubyteBinaryPattern   = new(@"^b'0b[01]{1,8}$");
        private static readonly Regex _ubyteHexPattern      = new(@"^b'0x([0-9]|[a-fA-F]){1,2}$");

        private static readonly IReadOnlyDictionary<int, BinaryOperator> BINARY_OPERATORS = new Dictionary<int, BinaryOperator>
        {
            [ZeroPointParser.PLUS]                  = BinaryOperator.Add,
            [ZeroPointParser.MINUS]                 = BinaryOperator.Sub,
            [ZeroPointParser.MULT]                  = BinaryOperator.Mult,
            [ZeroPointParser.DIV]                   = BinaryOperator.Div,
            [ZeroPointParser.MOD]                   = BinaryOperator.Mod,
            [ZeroPointParser.EQUAL]                 = BinaryOperator.Equal,
            [ZeroPointParser.STRICT_EQUAL]          = BinaryOperator.StrictEqual,
            [ZeroPointParser.NOTEQUAL]              = BinaryOperator.NotEqual,
            [ZeroPointParser.STRICT_NOTEQUAL]       = BinaryOperator.StrictNotEqual,
            [ZeroPointParser.LESS_THAN]             = BinaryOperator.LessThan,
            [ZeroPointParser.LESS_THAN_OR_EQUAL]    = BinaryOperator.LessThanOrEqual,
            [ZeroPointParser.GREATER_THAN]          = BinaryOperator.GreaterThan,
            [ZeroPointParser.GREATER_THAN_OR_EQUAL] = BinaryOperator.GreaterThanOrEqual,
            [ZeroPointParser.AND]                   = BinaryOperator.LogicalAnd,
            [ZeroPointParser.XOR]                   = BinaryOperator.LogicalXOr,
            [ZeroPointParser.OR]                    = BinaryOperator.LogicalOr,
            [ZeroPointParser.BITWISE_AND]           = BinaryOperator.BitwiseAnd,
            [ZeroPointParser.BITWISE_XOR]           = BinaryOperator.BitwiseXOr,
            [ZeroPointParser.BITWISE_OR]            = BinaryOperator.BitwiseOr,
            [ZeroPointParser.SHIFT_LEFT]            = BinaryOperator.ShiftLeft,
            [ZeroPointParser.SHIFT_RIGHT]           = BinaryOperator.ShiftRight
        };

        private static readonly IReadOnlyDictionary<int, AssignmentOperator> ASSIGNMENT_OPERATORS = new Dictionary<int, AssignmentOperator>
        {
            [ZeroPointParser.ASSIGN]                = AssignmentOperator.Assign,
            [ZeroPointParser.ADD_ASSIGN]            = AssignmentOperator.AddAssign,
            [ZeroPointParser.SUB_ASSIGN]            = AssignmentOperator.SubAssign,
            [ZeroPointParser.MULT_ASSIGN]           = AssignmentOperator.MultAssign,
            [ZeroPointParser.DIV_ASSIGN]            = AssignmentOperator.DivAssign,
            [ZeroPointParser.MOD_ASSIGN]            = AssignmentOperator.ModAssign,
            [ZeroPointParser.AND_ASSIGN]            = AssignmentOperator.AndAssign,
            [ZeroPointParser.XOR_ASSIGN]            = AssignmentOperator.XorAssign,
            [ZeroPointParser.OR_ASSIGN]             = AssignmentOperator.OrAssign,
            [ZeroPointParser.BITWISE_AND_ASSIGN]    = AssignmentOperator.BitwiseAndAssign,
            [ZeroPointParser.BITWISE_XOR_ASSIGN]    = AssignmentOperator.BitwiseXorAssign,
            [ZeroPointParser.BITWISE_OR_ASSIGN]     = AssignmentOperator.BitwiseOrAssign,
            [ZeroPointParser.SHIFT_LEFT_ASSIGN]     = AssignmentOperator.ShiftLeftAssign,
            [ZeroPointParser.SHIFT_RIGHT_ASSIGN]    = AssignmentOperator.ShiftRightAssign
        };

        private static readonly IReadOnlyDictionary<int, UnaryOperator> UNARY_OPERATORS = new Dictionary<int, UnaryOperator>
        {
            [ZeroPointParser.EXCLAMATION_MARK]      = UnaryOperator.Not,
            [ZeroPointParser.MINUS]                 = UnaryOperator.Minus
        };

        private static readonly IReadOnlyDictionary<int, LiteralType> LITERALS = new Dictionary<int, LiteralType>
        {
            [ZeroPointParser.STRING]                = LiteralType.String,
            [ZeroPointParser.BOOLEAN]               = LiteralType.Boolean,
            [ZeroPointParser.NUMBER]                = LiteralType.Number,
            [ZeroPointParser.NULL]                  = LiteralType.Null
        };

        // TODO: ObjectOperable has this too. Move to some static resource instead.
        private static readonly IReadOnlyDictionary<BinaryOperator, string> BINARY_OPERATOR_FUNC_NAMES = new Dictionary<BinaryOperator, string>
        {
            [BinaryOperator.Add]                    = "__operator_add__",
            [BinaryOperator.Sub]                    = "__operator_sub__",
            [BinaryOperator.Mult]                   = "__operator_mult__",
            [BinaryOperator.Div]                    = "__operator_div__",
            [BinaryOperator.Mod]                    = "__operator_mod__",
            [BinaryOperator.Equal]                  = "__operator_equals__",
            [BinaryOperator.NotEqual]               = "__operator_not_equals__",
            [BinaryOperator.LessThan]               = "__operator_less_than__",
            [BinaryOperator.LessThanOrEqual]        = "__operator_less_than_or_equals__",
            [BinaryOperator.GreaterThan]            = "__operator_greater_than__",
            [BinaryOperator.GreaterThanOrEqual]     = "__operator_greater_than_or_equals__",
            [BinaryOperator.LogicalAnd]             = "__operator_logical_and__",
            [BinaryOperator.LogicalOr]              = "__operator_logical_or__",
            [BinaryOperator.LogicalXOr]             = "__operator_logical_xor__",
            [BinaryOperator.BitwiseAnd]             = "__operator_bitwise_and__",
            [BinaryOperator.BitwiseOr]              = "__operator_bitwise_or__",
            [BinaryOperator.BitwiseXOr]             = "__operator_bitwise_xor__",
            [BinaryOperator.ShiftLeft]              = "__operator_shift_left__",
            [BinaryOperator.ShiftRight]             = "__operator_shift_right__"
        };

        private static readonly IReadOnlyDictionary<UnaryOperator, string> UNARY_OPERATOR_FUNC_NAMES = new Dictionary<UnaryOperator, string>
        {
            [UnaryOperator.Not]                     = "__operator_unary_not__",
            [UnaryOperator.Minus]                   = "__operator_unary_minus__"
        };

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
            int ruleIndex = context.GetRuleContext<ParserRuleContext>(0).RuleIndex;

            return ruleIndex switch
            {
                ZeroPointParser.RULE_assign_statement           => EnterAssignStatement(context.assign_statement()),
                ZeroPointParser.RULE_conditional_statement      => EnterConditionalStatement(context.conditional_statement()),
                ZeroPointParser.RULE_function_call_statement    => EnterFunctionCall(context.function_call_statement()),
                ZeroPointParser.RULE_loop_statement             => EnterLoopStatement(context.loop_statement()),
                ZeroPointParser.RULE_try_catch_statement        => EnterTryCatchStatement(context.try_catch_statement()),
                ZeroPointParser.RULE_throw_statement            => EnterThrowStatement(context.throw_statement()),
                _                                               => throw new NotImplementedException($"Rule {ruleIndex} does not exist")
            };
        }

        private IStatementModel EnterAssignStatement(ZeroPointParser.Assign_statementContext context)
        {
            int opType = context.assignment_operator().op.Type;

            // Standalone identifier
            if (context.IDENTIFIER() != null)
            {
                return new AssignStatementModel
                {
                    Identifier = new[] { context.IDENTIFIER().GetText() },
                    OperatorCombination = ASSIGNMENT_OPERATORS[opType],
                    Assignee = EnterExpression(context.expression()),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }

            // Path to identifier
            return new AssignStatementModel
            {
                Identifier = context.identifier_access().IDENTIFIER().Select(i => i.GetText()).ToArray(),
                OperatorCombination = ASSIGNMENT_OPERATORS[opType],
                Assignee = EnterExpression(context.expression()),
                StartToken = context.Start,
                StopToken = context.Stop
            };
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
            if (context.atom().IsPresent())
            {
                return EnterAtom(context.atom());
            }

            return EnterExpressionExpression(context);
        }

        private IExpressionModel EnterAtom(ZeroPointParser.AtomContext ctx)
        {
            if (ctx.literal().IsPresent())
            {
                return EnterLiteral(ctx.literal());
            }

            if (ctx.IDENTIFIER().IsPresent())
            {
                return new IdentifierExpressionModel
                {
                    Identifier = new[] { ctx.IDENTIFIER().GetText() },
                    StartToken = ctx.Start,
                    StopToken = ctx.Stop,
                };
            }

            if (ctx.identifier_access().IsPresent())
            {
                return new IdentifierExpressionModel
                {
                    Identifier = EnterIdentifierAccess(ctx.identifier_access()),
                    StartToken = ctx.Start,
                    StopToken = ctx.Stop
                };
            }

            if (ctx.function_call_statement().IsPresent())
            {
                return EnterFunctionCall(ctx.function_call_statement());
            }

            if (ctx.object_initialization_expression().IsPresent())
            {
                return EnterObjectInitialization(ctx.object_initialization_expression());
            }

            if (ctx.anonymous_function_definition_statement().IsPresent())
            {
                return EnterFunctionDefinitionExpression(ctx.anonymous_function_definition_statement());
            }

            throw new NotImplementedException("Invalid atom type");
        }

        /// <summary>
        /// Compiles an n:ary expression
        /// </summary>
        private IExpressionModel EnterExpressionExpression(ZeroPointParser.ExpressionContext context)
        {
            int expressionCount = context.expression().Length;

            // If this is true we know that this is an expression enclosed in parenthesis
            // or is a unary expression
            if (expressionCount is 1)
            {
                var e = context.expression()[0];
                if (context.EXCLAMATION_MARK() != null)
                {
                    return new UnaryExpressionModel
                    {
                        Expression = EnterExpression(e),
                        Operator = UnaryOperator.Not,
                        StartToken = context.Start,
                        StopToken = context.Stop
                    };
                }
                else if (context.MINUS() != null)
                {
                    return new UnaryExpressionModel
                    {
                        Expression = EnterExpression(e),
                        Operator = UnaryOperator.Minus,
                        StartToken = context.Start,
                        StopToken = context.Stop
                    };
                }
                else
                {
                    return EnterExpression(e);
                }
                //return EnterExpression(e);
            }

            // Ternary expression (conditional expression)
            if (expressionCount is 3)
            {
                var conditionalExpression = context.expression()[0];
                var trueExpression = context.expression()[1];
                var falseExpression = context.expression()[2];
                return new ConditionalTernaryExpressionModel
                {
                    ConditionExpression = EnterExpression(conditionalExpression),
                    TrueExpression = EnterExpression(trueExpression),
                    FalseExpression = EnterExpression(falseExpression),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }

            // Binary expression

            var left = context.expression(0);
            var right = context.expression(1);
            int opType = context.op.Type;

            return new BinaryExpressionModel
            {
                Operator = BINARY_OPERATORS[opType],
                LeftExpression = EnterExpression(left),
                RightExpression = EnterExpression(right),
                StartToken = context.Start,
                StopToken = context.Stop
            };
        }

        private IExpressionModel EnterFunctionDefinitionExpression(ZeroPointParser.Anonymous_function_definition_statementContext context)
        {
            int ruleIndex = context.GetRuleContext<ParserRuleContext>(0).RuleIndex;

            return ruleIndex switch
            {
                ZeroPointParser.RULE_function_statement         => EnterFunctionStatement(context.function_statement()),
                ZeroPointParser.RULE_action_statement           => EnterActionStatement(context.action_statement()),
                ZeroPointParser.RULE_consumer_statement         => EnterConsumerStatement(context.consumer_statement()),
                ZeroPointParser.RULE_provider_statement         => EnterProviderStatement(context.provider_statement()),
                ZeroPointParser.RULE_lambda_function_statement  => EnterLambdaFunctionStatement(context.lambda_function_statement()),
                ZeroPointParser.RULE_native_function_statement  => EnterNativeFunctionStatement(context.native_function_statement()),
                ZeroPointParser.RULE_native_provider_statement  => EnterNativeProviderStatement(context.native_provider_statement()),
                _                                               => throw new NotImplementedException($"Rule {ruleIndex} does not exist")
            };
        }

        private string[] EnterIdentifierAccess(ZeroPointParser.Identifier_accessContext context) =>
            context.IDENTIFIER().Select(i => i.GetText()).ToArray();

        private LiteralExpressionModel EnterLiteral(ZeroPointParser.LiteralContext context)
        {
            int literalType = context.lit.Type;

            return LITERALS[literalType] switch
            {
                LiteralType.String => ParseString(context),
                LiteralType.Boolean => new LiteralExpressionModel(BoolOperable.FromBool(bool.Parse(context.BOOLEAN().GetText())), context.Start, context.Stop),
                LiteralType.Number => ParseNumber(context),
                LiteralType.Null => new LiteralExpressionModel(NullOperable.Null),
                _ => throw new NotImplementedException($"Literal of type {literalType} is not implemented")
            };
        }

        private LiteralExpressionModel ParseNumber(ZeroPointParser.LiteralContext value)
        {
            string numberText = value.NUMBER().GetText();

            // Order might be important!
            if (_decimalNumberPattern.IsMatch(numberText))
            {
                var tmp = new BigDecimalOperable(BigDecimal.Parse(numberText, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture));
                return new LiteralExpressionModel(tmp, value.Start, value.Stop);
            }
            else if (_integerNumberPattern.IsMatch(numberText))
            {
                var tmp = new BigIntOperable(BigInteger.Parse(numberText));
                return new LiteralExpressionModel(tmp, value.Start, value.Stop);
            }
            else if (_integerBinaryPattern.IsMatch(numberText))
            {
                Range delimitBinary = new(2, numberText.Length);
                string binary = numberText[delimitBinary];
                var tmp = (BigIntOperable)NumberParserHelper.BinaryToBigInt(binary);
                return new LiteralExpressionModel(tmp, value.Start, value.Stop);
            }
            else if (_integerHexPattern.IsMatch(numberText))
            {
                Range delimitHex = new(2, numberText.Length);
                string hex = numberText[delimitHex];
                var tmp = (BigIntOperable)NumberParserHelper.HexToBigInt(hex);
                return new LiteralExpressionModel(tmp, value.Start, value.Stop);
            }
            else if (_ubytePattern.IsMatch(numberText))
            {
                var tmp = new ByteOperable(byte.Parse(numberText.Split('\'')[1]));
                return new LiteralExpressionModel(tmp, value.Start, value.Stop);
            }
            else if (_ubyteBinaryPattern.IsMatch(numberText))
            {
                Range delimitBinary = new(numberText.IndexOf('\'') + 3, numberText.Length);
                string binary = numberText[delimitBinary];
                var tmp = (ByteOperable)NumberParserHelper.BinaryToUByte(binary);
                return new LiteralExpressionModel(tmp, value.Start, value.Stop);
            }
            else if (_ubyteHexPattern.IsMatch(numberText))
            {
                Range delimitHex = new(numberText.IndexOf('\'') + 3, numberText.Length);
                string hex = numberText[delimitHex];
                var tmp = (ByteOperable)NumberParserHelper.HexToUByte(hex);
                return new LiteralExpressionModel(tmp, value.Start, value.Stop);
            }
            else throw new FormatException($"Could not recognize {nameof(ZeroPointParser.NUMBER)}: {numberText}");
        }

        private LiteralExpressionModel ParseString(ZeroPointParser.LiteralContext value)
        {
            string s = value.STRING().GetText();

            if (s.Length is 2)
            {
                s = string.Empty;
                return new LiteralExpressionModel(new StringOperable(s), value.Start, value.Stop);
            }
            else
                s = s[1..^1];

            s = s.Replace("\\\"", "\"");

            // Ugly but working solution
            // TODO: Beautify
            s = Regex.Replace(s,
                @"\\n", m =>
                {
                    int index = m.Index;
                    if (index is 0)
                    {
                        return "\n";
                    }
                    else if (s[index - 1] is '\\')
                    {
                        return m.Value; // do nothing
                        }
                    else
                    {
                        return "\n";
                    }
                });

            s = Regex.Replace(s, @"\\t", m =>
            {
                int index = m.Index;
                if (index is 0)
                {
                    return "\t";
                }
                else if (s[index - 1] is '\\')
                {
                    return m.Value; // do nothing
                }
                else
                {
                    return "\t";
                }
            });

            s = s.Replace("\\\\", "\\");

            return new LiteralExpressionModel((StringOperable)s, value.Start, value.Stop);
        }

        private FunctionStatementModel EnterFunctionStatement(ZeroPointParser.Function_statementContext context)
        {
            return new FunctionStatementModel
            {
                Parameters = context.parameter_list() != null ? EnterParameterList(context.parameter_list()) : new[] { context.IDENTIFIER().GetText() },
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
                Parameters = context.parameter_list() != null ? EnterParameterList(context.parameter_list()) : new[] { context.IDENTIFIER().GetText() },
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

        private LambdaFunctionStatementModel EnterLambdaFunctionStatement(ZeroPointParser.Lambda_function_statementContext context)
        {
            string[] parameters;

            if (context.parameter_list() is null && context.IDENTIFIER() is null) // No arguments
            {
                parameters = null;
            }
            else if (context.parameter_list() != null) // Lambda has a list of parameters
            {
                parameters = EnterParameterList(context.parameter_list());
            }
            else if (context.IDENTIFIER() != null) // Lambda has a single parameter
            {
                parameters = new[] { context.IDENTIFIER().GetText() };
            }
            else throw new NotImplementedException("Not a valid syntax parameter type");

            if (context.expression() != null) // Is lambda mode return?
            {
                return new LambdaFunctionStatementModel
                {
                    Parameters = parameters,
                    Return = EnterExpression(context.expression()),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else if (context.assign_statement() != null) // Is lambda mode assign?
            {
                return new LambdaFunctionStatementModel
                {
                    Parameters = parameters,
                    AssignStatement = (AssignStatementModel)EnterAssignStatement(context.assign_statement()),
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }
            else throw new NotImplementedException("Not a valid lambda mode");
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

        private string[] EnterParameterList(ZeroPointParser.Parameter_listContext context)
        {
            ITerminalNode[] parameters = context.IDENTIFIER();

            if (parameters is null || parameters.Length is 0)
                throw new InvalidOperationException("No parameters were present");

            return parameters.Select(p => p.GetText()).ToArray();
        }

        private IExpressionModel[] EnterArgumentList(ZeroPointParser.Argument_listContext context)
        {
            ZeroPointParser.ExpressionContext[] expressionCtxs = context.expression();

            IExpressionModel[] expressions = new IExpressionModel[expressionCtxs.Length];

            for (int i = 0; i < expressions.Length; i++)
            {
                expressions[i] = EnterExpression(expressionCtxs[i]);
            }

            return expressions;
        }

        private ObjectInitializationExpressionModel EnterObjectInitialization(ZeroPointParser.Object_initialization_expressionContext context)
        {

            if (context.assign_statement() is null && context.operator_function_statement() is null)
            {
                return new ObjectInitializationExpressionModel
                {
                    StartToken = context.Start,
                    StopToken = context.Stop
                };
            }

            var properties = new List<ObjectPropertyExpressionModel>();

            foreach (var s in context.GetRuleContexts<ParserRuleContext>())
            {
                switch (s.RuleIndex)
                {
                    case ZeroPointParser.RULE_assign_statement:
                        {
                            var assignStatement = s as ZeroPointParser.Assign_statementContext;
                            properties.Add(new ObjectPropertyExpressionModel
                            {
                                Identifier = assignStatement.IDENTIFIER().GetText(),
                                Value = EnterExpression(assignStatement.expression())
                            });
                            break;
                        }
                    case ZeroPointParser.RULE_operator_function_statement:
                        {
                            var operatorStatement = s as ZeroPointParser.Operator_function_statementContext;

                            if (operatorStatement.binary_operator_function_statement().IsPresent())
                            {
                                var binaryOperatorStatement = operatorStatement.binary_operator_function_statement();
                                int opCode = binaryOperatorStatement.op.Type;
                                BinaryOperator op = BINARY_OPERATORS[opCode];
                                properties.Add(new ObjectPropertyExpressionModel
                                {
                                    Identifier = BINARY_OPERATOR_FUNC_NAMES[op],
                                    Value = BinaryOperatorSyntaxSugarToFunctionStatement(binaryOperatorStatement)
                                });
                            }
                            else if (operatorStatement.unary_operator_function_statement().IsPresent())
                            {
                                var unaryOperatorStatement = operatorStatement.unary_operator_function_statement();
                                int opCode = unaryOperatorStatement.op.Type;
                                UnaryOperator op = UNARY_OPERATORS[opCode];
                                properties.Add(new ObjectPropertyExpressionModel
                                {
                                    Identifier = UNARY_OPERATOR_FUNC_NAMES[op],
                                    Value = UnaryOperatorSyntaxSugarToFunctionStatement(unaryOperatorStatement)
                                });
                            }
                            else
                                throw new NotImplementedException();
                            break;
                        }
                    default:
                        throw new NotImplementedException($"Rule {s.RuleIndex} does not exist");
                }
            }

            return new ObjectInitializationExpressionModel
            {
                Properties = properties,
                StartToken = context.Start,
                StopToken = context.Stop
            };
        }

        private FunctionStatementModel BinaryOperatorSyntaxSugarToFunctionStatement(ZeroPointParser.Binary_operator_function_statementContext ctx)
        {
            return new FunctionStatementModel
            {
                Parameters = new[] { ctx.IDENTIFIER(0).GetText(), ctx.IDENTIFIER(1).GetText() },
                Body = ctx.block().IsPresent() ? EnterBlock(ctx.block()) : null,
                Return = ctx.expression().IsPresent() ? EnterExpression(ctx.expression()) : null,
                StartToken = ctx.Start,
                StopToken = ctx.Stop
            };
        }

        private FunctionStatementModel UnaryOperatorSyntaxSugarToFunctionStatement(ZeroPointParser.Unary_operator_function_statementContext ctx)
        {
            return new FunctionStatementModel
            {
                Parameters = new[] { ctx.IDENTIFIER().GetText() },
                Body = ctx.block().IsPresent() ? EnterBlock(ctx.block()) : null,
                Return = ctx.expression().IsPresent() ? EnterExpression(ctx.expression()) : null,
                StartToken = ctx.Start,
                StopToken = ctx.Stop
            };
        }
    }

    internal static class ContextExtensions
    {
        public static bool IsPresent(this ParserRuleContext context) => context != null;
        public static bool IsPresent(this ITerminalNode node) => node != null;
        public static bool IsPresent(this IToken token) => token != null;
    }
}
