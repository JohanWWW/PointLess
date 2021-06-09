using Antlr4.Runtime;
using Interpreter.Models;
using Interpreter.Models.Enums;
using Interpreter.Models.Interfaces;
using Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public class ASTMapper
    {
        private readonly IReadOnlyDictionary<string, IFunctionModel> _nativeImplementations;

        public ASTMapper(NativeImplementationBase implementation)
        {
            _nativeImplementations = implementation.GetImplementationMap();
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
            parser.BuildParseTree = true;

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
                        PathToNamespace = new[] { s.IDENTIFIER()[0].GetText() }
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

            throw new NotImplementedException();
        }

        private IStatementModel EnterAssignStatement(ZeroPointParser.Assign_statementContext context)
        {
            if (context.IDENTIFIER() != null)
            {
                return new AssignStatementModel
                {
                    Identifier = new[] { context.IDENTIFIER().GetText() },
                    Assignee = EnterExpression(context.expression())
                };
            }

            return new AssignStatementModel
            {
                Identifier = context.identifier_access().IDENTIFIER().Select(i => i.GetText()).ToArray(),
                Assignee = EnterExpression(context.expression())
            };
        }

        private ConditionalStatementModel EnterConditionalStatement(ZeroPointParser.Conditional_statementContext context)
        {
            var ifStatement = new IfStatementModel
            {
                Condition = EnterExpression(context.if_statement().expression()),
                Body = EnterBlock(context.if_statement().block())
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
                Else = elseStatement
            };
        }

        private ILoopStatementModel EnterLoopStatement(ZeroPointParser.Loop_statementContext context)
        {
            if (context.for_loop_statement() != null)
            {
                throw new NotImplementedException();
            }

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
                Body = EnterBlock(context.block())
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
                    Identifier = new[] { context.IDENTIFIER().GetText() }
                };
            }

            if (context.identifier_access() != null)
            {
                return new IdentifierExpressionModel
                {
                    Identifier = context.identifier_access().IDENTIFIER().Select(i => i.GetText()).ToArray()
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
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.BITWISE_AND() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.BitwiseAnd,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.BITWISE_OR() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.BitwiseOr,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.BITWISE_XOR() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.BitwiseXOr,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.DIV() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.Div,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.EQUAL() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.Equal,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.GREATER_THAN() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.GreaterThan,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.GREATER_THAN_OR_EQUAL() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.GreaterThanOrEqual,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.LESS_THAN() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.LessThan,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.LESS_THAN_OR_EQUAL() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.LessThanOrEqual,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.MINUS() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.Minus,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.MOD() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.Mod,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.MULT() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.Mult,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.NOTEQUAL() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.NotEqual,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.OR() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.LogicalOr,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.PLUS() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.Plus,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.SHIFT_LEFT() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.ShiftLeft,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
                };
            }
            else if (context.SHIFT_RIGHT() != null)
            {
                return new BinaryExpressionModel
                {
                    Operator = BinaryOperator.ShiftRight,
                    LeftExpression = EnterExpression(left),
                    RightExpression = EnterExpression(right)
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
            if (context.BOOLEAN() != null)
            {
                return new LiteralExpressionModel
                {
                    Value = bool.Parse(context.BOOLEAN().GetText())
                };
            }

            if (context.NULL() != null)
            {
                return new LiteralExpressionModel
                {
                    Value = null
                };
            }

            if (context.NUMBER() != null)
            {
                return new LiteralExpressionModel
                {
                    Value = BigInteger.Parse(context.NUMBER().GetText())
                };
            }

            if (context.STRING() != null)
            {
                string s = context.STRING().GetText();

                var stringBuilder = new StringBuilder();

                if (s.Length is 2) // ""
                    stringBuilder.Append(string.Empty);
                else
                {
                    for (int i = 1; i < s.Length - 1; i++)
                        stringBuilder.Append(s[i]);
                }

                return new LiteralExpressionModel
                {
                    Value = stringBuilder.ToString()
                };
            }

            throw new NotImplementedException();
        }

        private FunctionStatementModel EnterFunctionStatement(ZeroPointParser.Function_statementContext context)
        {
            return new FunctionStatementModel
            {
                Parameters = new ParameterListModel { Parameters = context.parameter_list().IDENTIFIER().Select(i => i.GetText()).ToArray() },
                Body = EnterBlock(context.block()),
                Return = EnterExpression(context.expression())
            };
        }

        private ActionStatementModel EnterActionStatement(ZeroPointParser.Action_statementContext context)
        {
            return new ActionStatementModel
            {
                Body = EnterBlock(context.block())
            };
        }

        private ConsumerStatementModel EnterConsumerStatement(ZeroPointParser.Consumer_statementContext context)
        {
            return new ConsumerStatementModel
            {
                Parameters = new ParameterListModel { Parameters = context.parameter_list().IDENTIFIER().Select(i => i.GetText()).ToArray() },
                Body = EnterBlock(context.block())
            };
        }

        private ProviderStatementModel EnterProviderStatement(ZeroPointParser.Provider_statementContext context)
        {
            return new ProviderStatementModel
            {
                Body = EnterBlock(context.block()),
                Return = EnterExpression(context.expression())
            };
        }

        private NativeProviderStatementModel EnterNativeProviderStatement(ZeroPointParser.Native_provider_statementContext context)
        {
            string implementationIdentifier = EnterInjectStatement(context.inject_statement());

            return _nativeImplementations[implementationIdentifier] as NativeProviderStatementModel;
        }

        private NativeFunctionStatementModel EnterNativeFunctionStatement(ZeroPointParser.Native_function_statementContext context)
        {
            string implementationIdentifier = EnterInjectStatement(context.inject_statement());

            return _nativeImplementations[implementationIdentifier] as NativeFunctionStatementModel;
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
                    Arguments = context.argument_list() != null ? EnterArgumentList(context.argument_list()) : null
                };
            }

            return new FunctionCallStatement
            {
                IdentifierPath = EnterIdentifierAccess(context.identifier_access()),
                Arguments = context.argument_list() != null ? EnterArgumentList(context.argument_list()) : null
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
                return new ObjectInitializationExpressionModel();
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
                Properties = properties
            };
        }
    }
}
