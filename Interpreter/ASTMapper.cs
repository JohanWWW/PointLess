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
        private readonly IDictionary<string, CompilerConstDefinition> _compilerConstDefinitions;

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
            [ZeroPointParser.CHAR]                  = LiteralType.Char,
            [ZeroPointParser.BOOLEAN]               = LiteralType.Boolean,
            [ZeroPointParser.NUMBER]                = LiteralType.Number,
            [ZeroPointParser.NULL]                  = LiteralType.Null,
            [ZeroPointParser.VOID]                  = LiteralType.Void
        };

        private static readonly IReadOnlyDictionary<int, string> INDEXER_FUNC_NAMES = new Dictionary<int, string>
        {
            [ZeroPointParser.RULE_indexer_get_statement] = "__indexer_get__",
            [ZeroPointParser.RULE_indexer_set_statement] = "__indexer_set__"
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
            _compilerConstDefinitions = new Dictionary<string, CompilerConstDefinition>();
        }

        public ASTMapper()
        {
            _nativeImplementations = null;
            _compilerConstDefinitions = new Dictionary<string, CompilerConstDefinition>();
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

            // Listen for syntax errors detected by antlr
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
                ZeroPointParser.RULE_method_call_statement      => EnterMethodCallStatement(context.method_call_statement()),
                ZeroPointParser.RULE_loop_statement             => EnterLoopStatement(context.loop_statement()),
                ZeroPointParser.RULE_try_catch_statement        => EnterTryCatchStatement(context.try_catch_statement()),
                ZeroPointParser.RULE_throw_statement            => EnterThrowStatement(context.throw_statement()),
                ZeroPointParser.RULE_indexer_set_call_statement => EnterIndexerSetCallStatement(context.indexer_set_call_statement()),
                ZeroPointParser.RULE_compiler_const_definition  => EnterCompilerConstDefinition(context.compiler_const_definition()),
                _                                               => throw new NotImplementedException($"Rule {ruleIndex} does not exist")
            };
        }

        private IStatementModel EnterCompilerConstDefinition(ZeroPointParser.Compiler_const_definitionContext context)
        {
            string id = context.IDENTIFIER().GetText();

            CompilerConstDefinition statement = new()
            {
                Identifier = id,
                Expression = EnterExpression(context.expression()),
                StartToken = context.Start,
                StopToken = context.Stop
            };

            _compilerConstDefinitions.Add(id, statement);

            return statement;
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
            var ifStatementCtx = context.if_statement();
            var ifStatement = new IfStatementModel
            {
                Condition = EnterExpression(ifStatementCtx.expression()),
                Body = EnterBlock(ifStatementCtx.block()),
                StartToken = ifStatementCtx.Start,
                StopToken = ifStatementCtx.Stop
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
                        Body = EnterBlock(ei.block()),
                        StartToken = ei.Start,
                        StopToken = ei.Stop
                    });
                }
            }

            ElseStatementModel elseStatement = null;
            if (context.else_statement().IsPresent())
            {
                var elseStatementCtx = context.else_statement();
                elseStatement = new ElseStatementModel
                {
                    Body = EnterBlock(elseStatementCtx.block()),
                    StartToken = elseStatementCtx.Start,
                    StopToken = elseStatementCtx.Stop
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
            if (context.while_loop_statement().IsPresent())
            {
                return EnterWhileLoop(context.while_loop_statement());
            }

            if (context.foreach_loop_statement().IsPresent())
            {
                return EnterForeachLoop(context.foreach_loop_statement());
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

        private ForeachLoopStatement EnterForeachLoop(ZeroPointParser.Foreach_loop_statementContext context)
        {
            return new ForeachLoopStatement
            {
                Identifier = context.IDENTIFIER().GetText(),
                EnumerableExpression = EnterExpression(context.expression()),
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
            if (ctx.nameof_expression().IsPresent())
            {
                return EnterNameofExpression(ctx.nameof_expression());
            }

            if (ctx.literal().IsPresent())
            {
                return EnterLiteral(ctx.literal());
            }

            if (ctx.IDENTIFIER().IsPresent())
            {
                CompilerConstDefinition ics;
                string id = ctx.IDENTIFIER().GetText();

                if (_compilerConstDefinitions.TryGetValue(id, out ics))
                    // Paste/Apply constant
                    return ics.Expression;

                return new IdentifierExpressionModel
                {
                    Identifier = new[] { id },
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

            if (ctx.method_call_statement().IsPresent())
            {
                return EnterMethodCallStatement(ctx.method_call_statement());
            }

            if (ctx.object_initialization_expression().IsPresent())
            {
                return EnterObjectInitialization(ctx.object_initialization_expression());
            }

            if (ctx.anonymous_function_definition_statement().IsPresent())
            {
                return EnterFunctionDefinitionExpression(ctx.anonymous_function_definition_statement());
            }

            if (ctx.array_literal_notation().IsPresent())
            {
                return EnterArrayLiteralNotation(ctx.array_literal_notation());
            }

            if (ctx.dictionary_literal_notation().IsPresent())
            {
                return EnterDictionaryLiteralNotation(ctx.dictionary_literal_notation());
            }

            throw new LanguageSyntaxException(ctx.Start.Line, ctx.Start.Column, ctx.Stop.Column, "Invalid atom type");
        }

        private IExpressionModel EnterNameofExpression(ZeroPointParser.Nameof_expressionContext ctx)
        {
            return new NameofExpression
            {
                IdentifierModel = new IdentifierExpressionModel
                {
                    Identifier = ctx.IDENTIFIER().IsPresent() ? new string[] { ctx.IDENTIFIER().GetText() } : EnterIdentifierAccess(ctx.identifier_access()),
                    StartToken = ctx.Start,
                    StopToken = ctx.Stop
                },
                StartToken = ctx.Start,
                StopToken = ctx.Stop
            };
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
                LiteralType.Char => ParseChar(context),
                LiteralType.Boolean => new LiteralExpressionModel(BoolOperable.FromBool(bool.Parse(context.BOOLEAN().GetText())), context.Start, context.Stop),
                LiteralType.Number => ParseNumber(context),
                LiteralType.Null or LiteralType.Void => new LiteralExpressionModel(VoidOperable.Void),
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
            else throw new LanguageSyntaxException(value.Start.Line, value.Start.Column, value.Stop.Column, $"Could not recognize {numberText} as number literal because it was either too large or formatted incorrectly");
        }

        private LiteralExpressionModel ParseString(ZeroPointParser.LiteralContext value)
        {
            string s = value.STRING().GetText();

            if (s.Length is 2)
            {
                s = string.Empty;
                return new LiteralExpressionModel((StringObjectOperable)s, value.Start, value.Stop);
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

            return new LiteralExpressionModel((StringObjectOperable)s, value.Start, value.Stop);
        }

        private LiteralExpressionModel ParseChar(ZeroPointParser.LiteralContext ctx)
        {
            string c = ctx.CHAR().GetText();

            // Account for two extra characters in string -> ''
            if (c.Length is 2)
                throw new LanguageSyntaxException(ctx.Start.Line, ctx.Start.Column, ctx.Stop.Column, "Character literal cannot be empty");

            c = c[1..^1];

            if (c.Length > 1)
            {
                if (c[0] != '\\')
                    throw new LanguageSyntaxException(ctx.Start.Line, ctx.Start.Column, ctx.Stop.Column, "Character literal cannot contain more than one character or escape sequence");

                switch (c[1])
                {
                    case 'n':
                        return new((CharacterOperable)'\n', ctx.Start, ctx.Stop);
                    case 't':
                        return new((CharacterOperable)'\t', ctx.Start, ctx.Stop);
                    case '\\':
                        return new((CharacterOperable)'\\', ctx.Start, ctx.Stop);
                    case '\'':
                        return new((CharacterOperable)'\'', ctx.Start, ctx.Stop);
                    case 'x':
                        var hex = c.AsSpan()[2..];
                        var hexRegx = new Regex("^[0-9A-Fa-f]+$");
                        if (!hexRegx.IsMatch(hex.ToString()))
                            throw new LanguageSyntaxException(ctx.Start.Line, ctx.Start.Column, ctx.Stop.Column, "Illegal character present in hex sequence");
                        return new((CharacterOperable)NumberParserHelper.HexToInt(hex), ctx.Start, ctx.Stop);
                    default:
                        throw new LanguageSyntaxException(ctx.Start.Line, ctx.Start.Column, ctx.Stop.Column, "Could not recognize escape sequence");
                }
            }

            return new((CharacterOperable)c.First());
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

        private MethodCallStatementModel EnterMethodCallStatement(ZeroPointParser.Method_call_statementContext context)
        {
            int ruleIndex = context.GetRuleContext<ParserRuleContext>(0).RuleIndex;
            switch (ruleIndex)
            {
                case ZeroPointParser.RULE_paren_call:
                    {
                        var parenthesisCall = context.paren_call();

                        if (parenthesisCall.IDENTIFIER().IsPresent())
                        {
                            return new MethodCallStatementModel
                            {
                                IdentifierPath = new[] { parenthesisCall.IDENTIFIER().GetText() },
                                Arguments = parenthesisCall.argument_list() != null ? EnterArgumentList(parenthesisCall.argument_list()) : null,
                                StartToken = context.Start,
                                StopToken = context.Stop
                            };
                        }

                        return new MethodCallStatementModel
                        {
                            IdentifierPath = EnterIdentifierAccess(parenthesisCall.identifier_access()),
                            Arguments = parenthesisCall.argument_list() != null ? EnterArgumentList(parenthesisCall.argument_list()) : null,
                            StartToken = context.Start,
                            StopToken = context.Stop
                        };
                    }
                case ZeroPointParser.RULE_indexer_get_call_statement: // Syntactic sugar
                    {
                        var indexerCall = context.indexer_get_call_statement();

                        if (indexerCall.IDENTIFIER().IsPresent())
                        {
                            return new MethodCallStatementModel
                            {
                                IdentifierPath = new[] { indexerCall.IDENTIFIER().GetText(), INDEXER_FUNC_NAMES[ZeroPointParser.RULE_indexer_get_statement] },
                                Arguments = EnterArgumentList(indexerCall.argument_list()),
                                StartToken = indexerCall.Start,
                                StopToken = indexerCall.Stop
                            };
                        }

                        var path = new List<string>();
                        path.AddRange(EnterIdentifierAccess(indexerCall.identifier_access()));
                        path.Add(INDEXER_FUNC_NAMES[ZeroPointParser.RULE_indexer_get_statement]);

                        return new MethodCallStatementModel
                        {
                            IdentifierPath = path.ToArray(),
                            Arguments = EnterArgumentList(indexerCall.argument_list()),
                            StartToken = indexerCall.Start,
                            StopToken = indexerCall.Stop
                        };
                    }
                default:
                    throw new NotImplementedException($"Rule {ruleIndex} does not exist");
            }
        }

        // Syntactic Sugar
        private MethodCallStatementModel EnterIndexerSetCallStatement(ZeroPointParser.Indexer_set_call_statementContext ctx)
        {
            var args = new List<IExpressionModel>();
            args.AddRange(EnterArgumentList(ctx.argument_list()));
            args.Add(EnterExpression(ctx.expression()));

            if (ctx.IDENTIFIER().IsPresent())
            {
                return new MethodCallStatementModel
                {
                    IdentifierPath = new[] { ctx.IDENTIFIER().GetText(), INDEXER_FUNC_NAMES[ZeroPointParser.RULE_indexer_set_statement] },
                    Arguments = args.ToArray(),
                    StartToken = ctx.Start,
                    StopToken = ctx.Stop
                };
            }

            var path = new List<string>();
            path.AddRange(EnterIdentifierAccess(ctx.identifier_access()));
            path.Add(INDEXER_FUNC_NAMES[ZeroPointParser.RULE_indexer_set_statement]);

            return new MethodCallStatementModel
            {
                IdentifierPath = path.ToArray(),
                Arguments = args.ToArray(),
                StartToken = ctx.Start,
                StopToken = ctx.Stop
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
                    case ZeroPointParser.RULE_indexer_method_statement:
                        {
                            var indexerMethodStatement = s as ZeroPointParser.Indexer_method_statementContext;

                            if (indexerMethodStatement.indexer_get_statement().IsPresent())
                            {
                                var indexerGetStatement = indexerMethodStatement.indexer_get_statement();
                                properties.Add(new ObjectPropertyExpressionModel
                                {
                                    Identifier = INDEXER_FUNC_NAMES[indexerGetStatement.RuleIndex],
                                    Value = IndexerGetSyntaxSugarToFunctionStatement(indexerGetStatement)
                                });
                            }
                            else if (indexerMethodStatement.indexer_set_statement().IsPresent())
                            {
                                var indexerSetStatement = indexerMethodStatement.indexer_set_statement();
                                properties.Add(new ObjectPropertyExpressionModel
                                {
                                    Identifier = INDEXER_FUNC_NAMES[indexerSetStatement.RuleIndex],
                                    Value = IndexerSetSyntaxSugarToConsumerStatement(indexerSetStatement)
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

        private ConsumerStatementModel IndexerSetSyntaxSugarToConsumerStatement(ZeroPointParser.Indexer_set_statementContext ctx)
        {
            var parameters = new List<string>();
            parameters.AddRange(EnterParameterList(ctx.parameter_list()));
            parameters.Add(ctx.IDENTIFIER().GetText());
            return new ConsumerStatementModel
            {
                IsIndexerConsumer = true,
                Parameters = parameters.ToArray(),
                Body = ctx.block().IsPresent() ? EnterBlock(ctx.block()) : null,
                StartToken = ctx.Start,
                StopToken = ctx.Stop
            };
        }

        private FunctionStatementModel IndexerGetSyntaxSugarToFunctionStatement(ZeroPointParser.Indexer_get_statementContext ctx)
        {
            var parameters = new List<string>();
            parameters.AddRange(EnterParameterList(ctx.parameter_list()));
            return new FunctionStatementModel
            {
                IsIndexerFunction = true,
                Parameters = parameters.ToArray(),
                Body = ctx.block().IsPresent() ? EnterBlock(ctx.block()) : null,
                Return = EnterExpression(ctx.expression()),
                StartToken = ctx.Start,
                StopToken = ctx.Stop
            };
        }

        private ArrayLiteralNotationModel EnterArrayLiteralNotation(ZeroPointParser.Array_literal_notationContext ctx)
        {
            if (ctx.ALLOC().IsPresent())
            {
                if (ctx.argument_list().expression().Length > 1)
                    throw new LanguageSyntaxException(ctx.Start.Line, ctx.Start.Column, ctx.Stop.Column, "Array allocate statement contains more than one argument");

                return new ArrayLiteralNotationModel
                {
                    IsAllocSyntax = true,
                    Arguments = EnterArgumentList(ctx.argument_list()),
                    StartToken = ctx.Start,
                    StopToken = ctx.Stop
                };
            }

            if (!ctx.argument_list().IsPresent())
                return new ArrayLiteralNotationModel
                {
                    Arguments = null,
                    StartToken = ctx.Start,
                    StopToken = ctx.Stop
                };

            return new ArrayLiteralNotationModel
            {
                Arguments = EnterArgumentList(ctx.argument_list()),
                StartToken = ctx.Start,
                StopToken = ctx.Stop
            };
        }

        private DictionaryLiteralNotation EnterDictionaryLiteralNotation(ZeroPointParser.Dictionary_literal_notationContext ctx)
        {
            if (!ctx.dictionary_arguments().IsPresent())
                return new DictionaryLiteralNotation
                {
                    Arguments = null,
                    StartToken = ctx.Start,
                    StopToken = ctx.Stop
                };

            var arguments = EnterDictionaryArguments(ctx.dictionary_arguments());
            return new DictionaryLiteralNotation
            {
                Arguments = arguments.ToArray(),
                StartToken = ctx.Start,
                StopToken = ctx.Stop
            };
        }

        private IEnumerable<(IExpressionModel key, IExpressionModel value, IToken start, IToken stop)> EnterDictionaryArguments(ZeroPointParser.Dictionary_argumentsContext ctx)
        {
            var dictKeys = ctx.dictionary_key();
            var dictValues = ctx.dictionary_value();
            for (int i = 0; i < dictKeys.Length; i++)
                yield return (EnterExpression(dictKeys[i].expression()), EnterExpression(dictValues[i].expression()), dictKeys[i].Start, dictValues[i].Stop);

        }
    }

    internal static class ContextExtensions
    {
        public static bool IsPresent(this ParserRuleContext context) => context != null;
        public static bool IsPresent(this ITerminalNode node) => node != null;
        public static bool IsPresent(this IToken token) => token != null;
    }
}
