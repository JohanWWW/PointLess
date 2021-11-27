using Antlr4.Runtime;
using Interpreter.Environment;
using Interpreter.Environment.Exceptions;
using Interpreter.Models;
using Interpreter.Models.Delegates;
using Interpreter.Models.Enums;
using Interpreter.Models.Interfaces;
using Interpreter.Runtime;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public class Interpreter
    {
        private readonly Namespace _namespace;
        private readonly RuntimeEnvironment _environment;
        private readonly string _filePath;

        private static readonly IReadOnlyDictionary<AssignmentOperator, BinaryOperator> _ASSIGNMENT_TO_BINARY_OPERATOR_EQUIVALENT = new Dictionary<AssignmentOperator, BinaryOperator>
        {
            [AssignmentOperator.AddAssign]          = BinaryOperator.Add,
            [AssignmentOperator.SubAssign]          = BinaryOperator.Sub,
            [AssignmentOperator.MultAssign]         = BinaryOperator.Mult,
            [AssignmentOperator.DivAssign]          = BinaryOperator.Div,
            [AssignmentOperator.ModAssign]          = BinaryOperator.Mod,
            [AssignmentOperator.AndAssign]          = BinaryOperator.LogicalAnd,
            [AssignmentOperator.BitwiseAndAssign]   = BinaryOperator.BitwiseAnd,
            [AssignmentOperator.OrAssign]           = BinaryOperator.LogicalOr,
            [AssignmentOperator.BitwiseOrAssign]    = BinaryOperator.BitwiseOr,
            [AssignmentOperator.XorAssign]          = BinaryOperator.LogicalXOr,
            [AssignmentOperator.BitwiseXorAssign]   = BinaryOperator.BitwiseXOr,
            [AssignmentOperator.ShiftLeftAssign]    = BinaryOperator.ShiftLeft,
            [AssignmentOperator.ShiftRightAssign]   = BinaryOperator.ShiftRight
        };

        public Namespace Namespace => _namespace;
        public string FilePath => _filePath;

        public Interpreter(Namespace ns, RuntimeEnvironment environment, string filePath)
        {
            _namespace = ns;
            _environment = environment;
            _filePath = filePath;
        }

        /// <summary>
        /// Evaluates a binary expression. The right operand should be provided as a lambda that returns <see cref="IOperable"/>.
        /// This is so that logical expressions can be determined in advanced without the need to evaluate the right operand. <br></br>
        /// For example:
        /// <example>
        /// <code>
        /// // In this case the expression will evaluate to false and arbitraryOperation is never evaluated <br></br>
        /// x = false &amp;&amp; arbitraryOperation()
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="op">The binary operator to use</param>
        /// <param name="a">An evaluated operand of type <see cref="IBinaryOperable"/></param>
        /// <param name="b">An unevaluated operand of type <see cref="IOperable"/></param>
        /// <returns>Returns the evaluated result of the provided operands</returns>
        /// <exception cref="DivideByZeroException">If attempt to divide by zero</exception>
        private static IOperable EvaluateBinaryExpression(BinaryOperator op, IBinaryOperable a, Func<IOperable> b) => op switch
        {
            BinaryOperator.Add                      => a.Add(b()),
            BinaryOperator.Sub                      => a.Subtract(b()),
            BinaryOperator.Mult                     => a.Multiply(b()),
            BinaryOperator.Div                      => a.Divide(b()),
            BinaryOperator.Mod                      => a.Mod(b()),
            BinaryOperator.Equal                    => a.Equal(b()),
            BinaryOperator.StrictEqual              => a.StrictEqual(b()),
            BinaryOperator.NotEqual                 => a.NotEqual(b()),
            BinaryOperator.StrictNotEqual           => a.StrictNotEqual(b()),
            BinaryOperator.LessThan                 => a.LessThan(b()),
            BinaryOperator.LessThanOrEqual          => a.LessThanOrEqual(b()),
            BinaryOperator.GreaterThan              => a.GreaterThan(b()),
            BinaryOperator.GreaterThanOrEqual       => a.GreaterThanOrEqual(b()),
            BinaryOperator.LogicalAnd               => a.LogicalAnd(b),
            BinaryOperator.LogicalXOr               => a.LogicalXOr(b()),
            BinaryOperator.LogicalOr                => a.LogicalOr(b),
            BinaryOperator.BitwiseAnd               => a.BitwiseAnd(b),
            BinaryOperator.BitwiseXOr               => a.BitwiseXOr(b()),
            BinaryOperator.BitwiseOr                => a.BitwiseOr(b),
            BinaryOperator.ShiftLeft                => a.ShiftLeft(b()),
            BinaryOperator.ShiftRight               => a.ShiftRight(b()),
            _                                       => throw new NotImplementedException(),
        };

        private static IOperable EvaluateUnaryExpression(UnaryOperator op, IUnaryOperable x) => op switch
        {
            UnaryOperator.Not                       => x.UnaryNot(),
            UnaryOperator.Minus                     => x.UnaryMinus(),
            _                                       => throw new NotImplementedException()
        };

        private static void AddLocalBinding(string identifier, IOperable value, Scoping scope)
        {
            if (value.OperableType == ObjectType.Method)
            {
                MethodData md = new((value as IOperable<Method>).Value);
                scope.AddLocalBinding(identifier, new MethodDataOperable(md));
                return;
            }
            scope.AddLocalBinding(identifier, value);
        }

        private static void AddGlobalBinding(string identifier, IOperable value, Scoping scope)
        {
            if (value.OperableType == ObjectType.Method)
            {
                MethodData md = new((value as IOperable<Method>).Value);
                scope.SetGlobalBinding(identifier, new MethodDataOperable(md));
                return;
            }
            scope.SetGlobalBinding(identifier, value);
        }

        private static bool TrySetRuntimeObjectMember(RuntimeObject obj, string identifier, IOperable value)
        {
            if (value.OperableType == ObjectType.Method)
            {
                Method m = (value as IOperable<Method>).Value;

                if (m.IsIndexerMethod && obj.ContainsMember(identifier)) // This prevents indexer methods from being overwritten
                {
                    MethodData indexer = (obj[identifier] as IOperable<MethodData>).Value;
                    indexer.AddOverload(m); // TODO: Catch MethodOverloadException and provide details for InterpreterRuntimeException
                    return true;
                }

                MethodData md = new(m);
                
                return obj.TrySetMember(identifier, (MethodDataOperable)md);
            }

            return obj.TrySetMember(identifier, value);
        }

        private static bool TryGetRuntimeObjectMember(RuntimeObject obj, string identifier, out IOperable value) =>
            obj.TryGetMember(identifier, out value);

        private IOperable AttemptToEvaluateExpression(BinaryExpressionModel expression, Scoping scope)
        {
            BinaryOperator op = expression.Operator;

            IOperable a = EnterExpression(expression.LeftExpression, scope);
            
            return AttemptToEvaluateExpression(op, (IBinaryOperable)a, () => EnterExpression(expression.RightExpression, scope), expression);
        }

        /// <summary>
        /// Attempt to evaluate given expression. Throw exception if it fails.
        /// </summary>
        private IOperable AttemptToEvaluateExpression(BinaryOperator op, IBinaryOperable a, Func<IOperable> b, IModel runtimeModel)
        {
            IOperable bEval = null;
            try
            {
                return bEval = EvaluateBinaryExpression(op, a, () => bEval = b());
            }
            catch (DivideByZeroException e)
            {
                throw new InterpreterRuntimeException(runtimeModel, _filePath, "Attempted to divide by zero", e);
            }
            catch (MethodOverloadException e)
            {
                throw new InterpreterRuntimeException(runtimeModel, _filePath, e.Message, e);
            }
            catch (MissingOperatorOverrideException e)
            {
                throw new InterpreterRuntimeException(runtimeModel, _filePath, e.Message, e);
            }
        }

        private IOperable AttemptToEvaluateUnaryExpression(UnaryExpressionModel expression, Scoping scope)
        {
            IUnaryOperable x = (IUnaryOperable)EnterExpression(expression.Expression, scope);
            return AttemptToEvaluateUnaryExpression(expression.Operator, x, expression);
        }

        /// <summary>
        /// Attempt to evaluate unary expression. Throw exception if it fails.
        /// </summary>
        private IOperable AttemptToEvaluateUnaryExpression(UnaryOperator op, IUnaryOperable x, IModel runtimeModel)
        {
            try
            {
                return EvaluateUnaryExpression(op, x);
            }
            catch (MethodOverloadException e)
            {
                throw new InterpreterRuntimeException(runtimeModel, _filePath, e.Message, e);
            }
            catch (MissingOperatorOverrideException e)
            {
                throw new InterpreterRuntimeException(runtimeModel, _filePath, e.Message, e);
            }
        }

        public void Interpret(RootModel root)
        {
            EnterRoot(root);
        }

        public void EnterRoot(RootModel root)
        {
            foreach (IStatementModel model in root.Statements)
            {
                EnterStatement(model, _namespace.Scope);
            }
        }

        public void EnterStatement(IStatementModel statement, Scoping scope)
        {
            switch (statement.TypeCode)
            {
                case ModelTypeCode.AssignStatement:
                    EnterAssignStatement(statement as AssignStatementModel, scope);
                    break;
                case ModelTypeCode.ConditionalStatement:
                    EnterConditionalStatement(statement as ConditionalStatementModel, scope);
                    break;
                case ModelTypeCode.MethodCallStatement:
                    EnterFunctionCallStatement(statement as MethodCallStatementModel, scope);
                    break;
                case ModelTypeCode.WhileLoopStatement:
                    EnterWhileLoopStatement(statement as WhileLoopStatement, scope);
                    break;
                case ModelTypeCode.ForeachLoopStatement:
                    EnterForeachLoopStatement(statement as ForeachLoopStatement, scope);
                    break;
                case ModelTypeCode.UseStatement:
                    EnterUseStatementModel(statement as UseStatementModel, scope);
                    break;
                case ModelTypeCode.TryCatchStatement:
                    EnterTryCatchStatement(statement as TryCatchStatementModel, scope);
                    break;
                case ModelTypeCode.ThrowStatement:
                    EnterThrowStatement(statement as ThrowStatement, scope);
                    break;
                case ModelTypeCode.CompilerConstDefinition:
                    // Don't interpret because it has already been processed by the compiler
                    break;
                default:
                    throw new NotImplementedException($"Statement with type code '{statement.TypeCode}' is not implemented");
            }
        }

        public IOperable EnterExpression(IExpressionModel expression, Scoping scope) => expression.TypeCode switch
        {
            ModelTypeCode.LiteralExpression                 => EnterLiteralExpression(expression as LiteralExpressionModel, scope),
            ModelTypeCode.IdentifierExpression              => EnterIdentifierExpression(expression as IdentifierExpressionModel, scope),
            ModelTypeCode.BinaryExpression                  => EnterBinaryExpression(expression as BinaryExpressionModel, scope),
            ModelTypeCode.UnaryExpression                   => EnterUnaryExpression(expression as UnaryExpressionModel, scope),
            ModelTypeCode.ConditionalTernaryExpression      => EnterConditionalTernaryExpression(expression as ConditionalTernaryExpressionModel, scope),

            // ++Methods++
            ModelTypeCode.ActionStatement                   => EnterActionStatement(expression as ActionStatementModel, scope),
            ModelTypeCode.FunctionStatement                 => EnterFunctionStatement(expression as FunctionStatementModel, scope),
            ModelTypeCode.ProviderStatement                 => EnterProviderStatement(expression as ProviderStatementModel, scope),
            ModelTypeCode.ConsumerStatement                 => EnterConsumerStatement(expression as ConsumerStatementModel, scope),
            ModelTypeCode.LambdaFunctionStatement           => EnterLambdaStatement(expression as LambdaFunctionStatementModel, scope),
            ModelTypeCode.NativeConsumerStatement           => EnterNativeConsumerStatement(expression as NativeConsumerStatementModel, scope),
            ModelTypeCode.NativeProviderStatement           => EnterNativeProviderStatement(expression as NativeProviderStatementModel, scope),
            ModelTypeCode.NativeFunctionStatement           => EnterNativeFunctionStatement(expression as NativeFunctionStatementModel, scope),
            ModelTypeCode.NativeActionStatement             => EnterNativeActionStatement(expression as NativeActionStatementModel, scope),
            // --Methods--

            ModelTypeCode.MethodCallStatement               => EnterFunctionCallStatement(expression as MethodCallStatementModel, scope),
            ModelTypeCode.ObjectInitializationExpression    => EnterObjectInitializationExpression(expression as ObjectInitializationExpressionModel, scope),
            ModelTypeCode.ArrayLiteralNotation              => EnterArrayLiteralNotation(expression as ArrayLiteralNotationModel, scope),
            ModelTypeCode.DictionaryLiteralNotation         => EnterDictionaryObjectOperable(expression as DictionaryLiteralNotation, scope),
            _                                               => throw new InterpreterRuntimeException(expression, _filePath, $"Expression with type code '{expression.TypeCode}' is not implemented"),
        };

        public IOperable EnterBinaryExpression(BinaryExpressionModel expression, Scoping scope) => AttemptToEvaluateExpression(expression, scope);

        public IOperable EnterUnaryExpression(UnaryExpressionModel expression, Scoping scope) => AttemptToEvaluateUnaryExpression(expression, scope);

        public IOperable EnterConditionalTernaryExpression(ConditionalTernaryExpressionModel expression, Scoping scope)
        {
            IOperable conditionEval = EnterExpression(expression.ConditionExpression, scope);
            if (conditionEval.OperableType != ObjectType.Boolean)
                throw new InterpreterRuntimeException(expression, _filePath, "Condition part of ternary expression was not a boolean expression");

            if ((bool)conditionEval.Value)
            {
                return EnterExpression(expression.TrueExpression, scope);
            }
            else
            {
                return EnterExpression(expression.FalseExpression, scope);
            }

        }

        public IOperable EnterLiteralExpression(LiteralExpressionModel expression, Scoping scope) => expression.Value;

        public IOperable EnterIdentifierExpression(IdentifierExpressionModel expression, Scoping scope)
        {
            IOperable __out;
            if (expression.Identifier.Length is 1)
            {
                if (scope.TryGetGlobalBinding(expression.Identifier[0], out __out))
                    return __out;
                else if (_namespace.TryGetImportedBinding(expression.Identifier[0], out __out))
                    return __out;

                throw new InterpreterRuntimeException(expression, _filePath, $"${expression.Identifier[0]} is not defined in current scope");
            }

            IOperable value =
                scope.TryGetGlobalBinding(expression.Identifier[0], out __out) ? __out :
                _namespace.TryGetImportedBinding(expression.Identifier[0], out __out) ? __out :
                throw new InterpreterRuntimeException(expression, _filePath, $"${expression.Identifier[0]} is not defined");

            RuntimeObject obj = value.OperableType switch
            {
                ObjectType.Object => (value as IOperable<RuntimeObject>).Value,
                ObjectType.Void => throw new InterpreterRuntimeException(expression, _filePath, $"Cannot access member on void type ${expression.Identifier[0]}"),
                _ => throw new InterpreterRuntimeException(expression, _filePath, $"Attempted to access member of atom type '{value.OperableType}'")
            };

            for (int i = 1; i < expression.Identifier.Length - 1; i++)
            {
                if (TryGetRuntimeObjectMember(obj, expression.Identifier[i], out IOperable memberValue))
                {
                    obj = memberValue.OperableType switch
                    {
                        ObjectType.Object => (RuntimeObject)memberValue.Value,
                        ObjectType.Void => throw new InterpreterRuntimeException(expression, _filePath, $"Member ${expression.Identifier[i - 1]}->{expression.Identifier[i]} is defined but is null reference"),
                        _ => throw new InterpreterRuntimeException(expression, _filePath, $"Attempted to access member of atom type '{value.OperableType}'")
                    };
                }
                else
                    throw new InterpreterRuntimeException(expression, _filePath, $"Member ${expression.Identifier[i - 1]}->{expression.Identifier[i]} is not a defined");
            }

            if (TryGetRuntimeObjectMember(obj, expression.Identifier.Last(), out IOperable outerMemberValue))
                return outerMemberValue;

            throw new InterpreterRuntimeException(expression, _filePath, $"Member ${expression.Identifier[^2]}->{expression.Identifier.Last()} is not defined");

        }

        public IOperable EnterObjectInitializationExpression(ObjectInitializationExpressionModel expression, Scoping scope)
        {
            var runtimeObject = new RuntimeObject();

            foreach (ObjectPropertyExpressionModel property in expression.Properties)
            {
                IOperable value = EnterExpression(property.Value, scope);
                if (!TrySetRuntimeObjectMember(runtimeObject, property.Identifier, value))
                    throw new InterpreterRuntimeException(expression, _filePath, $"Failed when binding object member '{property.Identifier}'");
            }

            return (ObjectOperable)runtimeObject;
        }

        public IOperable EnterFunctionCallStatement(MethodCallStatementModel functionCall, Scoping scope)
        {
            IOperable method;

            // Variable is object
            if (functionCall.IdentifierPath.Length > 1)
            {
                IOperable value =
                    scope.TryGetGlobalBinding(functionCall.IdentifierPath.First(), out IOperable __out) ? __out :
                    _namespace.TryGetImportedBinding(functionCall.IdentifierPath.First(), out __out) ? __out :
                    throw new InterpreterRuntimeException(functionCall, _filePath, $"${functionCall.IdentifierPath.First()} is not defined");

                RuntimeObject obj = value.OperableType switch
                {
                    ObjectType.Object or
                    ObjectType.StringObject or
                    ObjectType.ArrayObject or
                    ObjectType.DictionaryObject => (value as IOperable<RuntimeObject>).Value,
                    ObjectType.Void => throw new InterpreterRuntimeException(functionCall, _filePath, $"Cannot access members on void type ${functionCall.IdentifierPath.First()}"),
                    _ => throw new InterpreterRuntimeException(functionCall, _filePath, $"Attempted to access member on atom type '{value.OperableType}'")
                };

                for (int i = 1; i < functionCall.IdentifierPath.Length - 1; i++)
                {
                    if (TryGetRuntimeObjectMember(obj, functionCall.IdentifierPath[i], out IOperable memberValue))
                    {
                        obj = memberValue.OperableType switch
                        {
                            ObjectType.Object => (RuntimeObject)memberValue.Value,
                            ObjectType.Void => throw new InterpreterRuntimeException(functionCall, _filePath, $"Member ${functionCall.IdentifierPath[i - 1]}->{functionCall.IdentifierPath[i]} is defined but is null reference"),
                            _ => throw new InterpreterRuntimeException(functionCall, _filePath, $"Attempted to access member on atom type '{value.OperableType}'")
                        };
                    }
                    else
                        throw new InterpreterRuntimeException(functionCall, _filePath, $"Member ${string.Join("->", functionCall.IdentifierPath[0..(i - 1)])} is not defined");
                }

                if (TryGetRuntimeObjectMember(obj, functionCall.IdentifierPath.Last(), out method))
                {
                }
                else
                    throw new InterpreterRuntimeException(functionCall, _filePath, $"Member ${string.Join("->", functionCall.IdentifierPath)} is not defined");
            }
            // Variable is single variable
            else if (scope.TryGetGlobalBinding(functionCall.IdentifierPath.First(), out method))
            {
            }
            else if (_namespace.TryGetImportedBinding(functionCall.IdentifierPath.First(), out method))
            {
            }

            if (method is null)
                throw new InterpreterRuntimeException(functionCall, _filePath, $"Method ${string.Join("->", functionCall.IdentifierPath)} is not defined in current scope");

            switch (method.OperableType)
            {
                case ObjectType.MethodData:
                case ObjectType.Method:
                    break;
                default:
                    throw new InterpreterRuntimeException(functionCall, _filePath, $"Member ${string.Join("->", functionCall.IdentifierPath)} is not a method");
            }

            IExpressionModel[] args = functionCall.Arguments;
            int argumentCount = functionCall.Arguments?.Length ?? 0;
            
            try
            {
                if (method.OperableType == ObjectType.MethodData)
                {
                    MethodData md = (method as IOperable<MethodData>).Value;

                    if (!md.TryGetOverload(argumentCount, out Method overload))
                        throw new InterpreterRuntimeException(functionCall, _filePath, $"Argument count mismatch: Could not find suitable overload with parameter count {argumentCount}.");

                    switch (overload.MethodType)
                    {
                        case MethodType.Function:
                            {
                                IOperable[] evalArgs = new IOperable[argumentCount];
                                for (int i = 0; i < argumentCount; i++)
                                {
                                    evalArgs[i] = EnterExpression(args[i], scope);
                                }
                                return overload.GetFunction().Invoke(evalArgs);
                            }
                        case MethodType.Action:
                            overload.GetAction().Invoke();
                            return null;
                        case MethodType.Consumer:
                            {
                                IOperable[] evalArgs = new IOperable[argumentCount];
                                for (int i = 0; i < argumentCount; i++)
                                {
                                    evalArgs[i] = EnterExpression(args[i], scope);
                                }
                                overload.GetConsumer().Invoke(evalArgs);
                                return null;
                            }
                        case MethodType.Provider:
                            return overload.GetProvider().Invoke();
                        default:
                            throw new NotImplementedException($"Method type {overload.MethodType} not implemented");
                    }
                }
                else if (method.OperableType == ObjectType.Method)
                {
                    Method overload = (method as IOperable<Method>).Value;

                    switch (overload.MethodType)
                    {
                        case MethodType.Function:
                            {
                                IOperable[] evalArgs = new IOperable[argumentCount];
                                for (int i = 0; i < argumentCount; i++)
                                {
                                    evalArgs[i] = EnterExpression(args[i], scope);
                                }
                                return overload.GetFunction().Invoke(evalArgs);
                            }
                        case MethodType.Action:
                            overload.GetAction().Invoke();
                            return null;
                        case MethodType.Consumer:
                            {
                                IOperable[] evalArgs = new IOperable[argumentCount];
                                for (int i = 0; i < argumentCount; i++)
                                {
                                    evalArgs[i] = EnterExpression(args[i], scope);
                                }
                                overload.GetConsumer().Invoke(evalArgs);
                                return null;
                            }
                        case MethodType.Provider:
                            return overload.GetProvider().Invoke();
                        default:
                            throw new NotImplementedException($"Method type {overload.MethodType} not implemented");
                    }
                }
            }
            catch (OperableException e)
            {
                throw new InterpreterRuntimeException(functionCall, _filePath, e.Message, e);
            }
            catch (NativeImplementationException e)
            {
                throw new InterpreterRuntimeException(functionCall, _filePath, e.Message, e);
            }


            throw new NotImplementedException("The provided method atom type is not implemented");
        }

        public IOperable<Method> EnterActionStatement(ActionStatementModel actionStatement, Scoping outerScope)
        {
            Method action = new Method(
                parameterCount: 0,
                type: MethodType.Action,
                method: new ActionMethod(() =>
                {
                    // Create scope for function body
                    var blockScope = new Scoping();

                    // Chain together with outer scope
                    blockScope.SetOuterScope(outerScope);

                    // Evaluate block
                    EnterBlock(actionStatement.Body, blockScope);
                })
            );

            return new MethodOperable(action);
        }

        public IOperable<Method> EnterFunctionStatement(FunctionStatementModel functionStatement, Scoping outerScope)
        {
            Method function = new Method(
                parameterCount: functionStatement.Parameters.Length,
                type: MethodType.Function,
                method: new FunctionMethod(args =>
                {
                    string[] parameters = functionStatement.Parameters;

                    // Create scope for function body
                    var blockScope = new Scoping();

                    // Chain together with outer scope
                    blockScope.SetOuterScope(outerScope);

                    // Put the argument values in this local scope
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        string argIdentifier = parameters[i];
                        IOperable argValue = args[i];

                        AddLocalBinding(argIdentifier, argValue, blockScope);
                    }

                    // Execute the block
                    EnterBlock(functionStatement.Body, blockScope);

                    // Evaluate return expression
                    return EnterExpression(functionStatement.Return, blockScope);
                }),
                isIndexerMethod: true
            );

            return new MethodOperable(function);
        }

        public IOperable<Method> EnterProviderStatement(ProviderStatementModel providerStatement, Scoping outerScope)
        {
            Method provider = new Method(
                parameterCount: 0,
                type: MethodType.Provider,
                method: new ProviderMethod(() =>
                {
                    // Create scope for function body
                    var blockScope = new Scoping();

                    // Chain together with outer scope
                    blockScope.SetOuterScope(outerScope);

                    // Execute the block
                    EnterBlock(providerStatement.Body, blockScope);

                    // Evaluate return expression
                    return EnterExpression(providerStatement.Return, blockScope);
                })
            );

            return new MethodOperable(provider);
        }

        public IOperable<Method> EnterConsumerStatement(ConsumerStatementModel consumerStatement, Scoping outerScope)
        {
            Method consumer = new Method(
                parameterCount: consumerStatement.Parameters.Length,
                type: MethodType.Consumer,
                method: new ConsumerMethod(args =>
                {
                    string[] parameters = consumerStatement.Parameters;

                    // Create local scope for function body
                    var localScope = new Scoping();

                    // Chain together with outer scope
                    localScope.SetOuterScope(outerScope);

                    // Put argument values in this local scope
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        string argIdentifier = parameters[i];
                        IOperable argValue = args[i];

                        AddLocalBinding(argIdentifier, argValue, localScope);
                    }

                    // Evaluate function body
                    EnterBlock(consumerStatement.Body, localScope);
                }),
                isIndexerMethod: true
            );

            return new MethodOperable(consumer);
        }

        public IOperable<Method> EnterLambdaStatement(LambdaFunctionStatementModel lambdaStatement, Scoping outerScope)
        {
            string[] parameters = lambdaStatement.Parameters;

            if (parameters is null)
            {
                if (lambdaStatement.IsModeReturn)
                {
                    Method provider = new Method(
                        parameterCount: 0,
                        type: MethodType.Provider,
                        method: new ProviderMethod(() => EnterExpression(lambdaStatement.Mode as IExpressionModel, outerScope))
                    );
                    return new MethodOperable(provider);
                }
                else
                {
                    Method action = new Method(
                        parameterCount: 0,
                        type: MethodType.Action,
                        method: new ActionMethod(() => EnterAssignStatement(lambdaStatement.Mode as AssignStatementModel, outerScope))
                    );
                    return new MethodOperable(action);
                }
            }
            else
            {
                if (lambdaStatement.IsModeReturn)
                {
                    Method function = new Method(
                        parameterCount: parameters.Length,
                        type: MethodType.Function,
                        method: new FunctionMethod(args =>
                        {
                            var localScope = new Scoping();
                            localScope.SetOuterScope(outerScope);

                            for (int i = 0; i < parameters.Length; i++)
                            {
                                string argId = parameters[i];
                                IOperable argVal = args[i];

                                AddLocalBinding(argId, argVal, localScope);
                            }

                            return EnterExpression(lambdaStatement.Mode as IExpressionModel, localScope);
                        })
                    );
                    return new MethodOperable(function);
                }
                else
                {
                    Method consumer = new Method(
                        parameterCount: parameters.Length,
                        type: MethodType.Consumer,
                        method: new ConsumerMethod(args =>
                        {
                            var localScope = new Scoping();
                            localScope.SetOuterScope(outerScope);

                            for (int i = 0; i < parameters.Length; i++)
                            {
                                string argId = parameters[i];
                                IOperable argVal = args[i];

                                AddLocalBinding(argId, argVal, localScope);
                            }

                            EnterAssignStatement(lambdaStatement.Mode as AssignStatementModel, localScope);
                        })
                    );
                    return new MethodOperable(consumer);
                }
            }
        }

        public IOperable<Method> EnterNativeConsumerStatement(NativeConsumerStatementModel consumerStatement, Scoping outerScope)
        {
            Method consumer = new Method(
                parameterCount: consumerStatement.Parameters.Length,
                type: MethodType.Consumer,
                method: new ConsumerMethod(args =>
                {
                    string[] parameters = consumerStatement.Parameters;
                    var localScope = new Scoping();
                    localScope.SetOuterScope(outerScope);

                    // Put argument values in this local scope
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        string argIdentifier = parameters[i];
                        IOperable argValue = args[i];

                        AddLocalBinding(argIdentifier, argValue, localScope);
                    }

                    consumerStatement.NativeImplementation(args);
                })
            );
            return new MethodOperable(consumer);
        }

        public IOperable<Method> EnterNativeProviderStatement(NativeProviderStatementModel providerStatement, Scoping outerScope)
        {
            Method provider = new Method(
                parameterCount: 0,
                type: MethodType.Provider,
                method: new ProviderMethod(() =>
                {
                    var localScope = new Scoping();
                    localScope.SetOuterScope(outerScope);

                    return providerStatement.NativeImplementation();
                })
            );
            return new MethodOperable(provider);
        }

        public IOperable<Method> EnterNativeFunctionStatement(NativeFunctionStatementModel functionStatement, Scoping outerScope)
        {
            Method function = new Method(
                parameterCount: functionStatement.Parameters.Length,
                type: MethodType.Function,
                method: new FunctionMethod(args =>
                {
                    string[] parameters = functionStatement.Parameters;
                    var localScope = new Scoping();
                    localScope.SetOuterScope(outerScope);

                    // Put argument values in this local scope
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        string argIdentifier = parameters[i];
                        IOperable argValue = args[i];

                        AddLocalBinding(argIdentifier, argValue, localScope);
                    }

                    return functionStatement.NativeImplementation(args);
                })
            );
            return new MethodOperable(function);
        }

        public IOperable<Method> EnterNativeActionStatement(NativeActionStatementModel actionStatement, Scoping outerScope)
        {
            Method action = new Method(
                parameterCount: 0,
                type: MethodType.Action,
                method: new ActionMethod(() => actionStatement.NativeImplementation())
            );
            return new MethodOperable(action);
        }

        public void EnterBlock(BlockModel block, Scoping scope)
        {
            if (block is null)
                return;

            foreach (IStatementModel statement in block.Statements)
            {
                EnterStatement(statement, scope);
            }
        }

        public void EnterWhileLoopStatement(WhileLoopStatement loop, Scoping outerScope)
        {
            // Expression must be evaluated for each iteration!!
            IOperable condition = EnterExpression(loop.Condition, outerScope);
            if (condition.OperableType != ObjectType.Boolean)
                throw new InterpreterRuntimeException(loop, _filePath, $"While loop condition expression was not a boolean");

            while ((bool)condition.Value)
            {
                Scoping localScope = new();
                localScope.SetOuterScope(outerScope);

                EnterBlock(loop.Body, localScope);

                // Reevaluate the condition
                condition = EnterExpression(loop.Condition, outerScope);
                if (condition.OperableType != ObjectType.Boolean)
                    throw new InterpreterRuntimeException(loop, _filePath, $"While loop condition expression was not a boolean");
            }
        }

        // Syntactic sugar
        public void EnterForeachLoopStatement(ForeachLoopStatement loop, Scoping outerScope)
        {
            IOperable evaluatedEnumerable = EnterExpression(loop.EnumerableExpression, outerScope);
            if (!(evaluatedEnumerable is IOperable<RuntimeObject>))
                throw new InterpreterRuntimeException(loop, _filePath, "foreach loop: Right expression was not an object");

            RuntimeObject enumerable = (evaluatedEnumerable as IOperable<RuntimeObject>).Value;
            if (!enumerable.TryGetMember("enumerator", out IOperable enumeratorMethodOp))
                throw new InterpreterRuntimeException(loop, _filePath, "foreach loop: Foreach loop requires an enumerator which was not present in the enumerable");
            if (!(enumeratorMethodOp is IOperable<MethodData>))
                throw new InterpreterRuntimeException(loop, _filePath, "foreach loop: The enumerator present in the enumerable was not an object");

            if (!(enumeratorMethodOp as IOperable<MethodData>).Value.TryGetOverload(0, out Method m) && m.MethodType != MethodType.Provider)
                throw new InterpreterRuntimeException(loop, _filePath, "foreach loop: Enumerable does not contain an enumerator provider overload with parameter count 0");
            ProviderMethod enumeratorMethod = m.GetProvider();

            IOperable enumeratorOp = enumeratorMethod.Invoke();
            if (enumeratorOp.OperableType != ObjectType.Object)
                throw new InterpreterRuntimeException(loop, _filePath, "foreach loop: Enumerator is not an object");

            RuntimeObject enumerator = (enumeratorOp as IOperable<RuntimeObject>).Value;

            if (!enumerator.TryGetMember("next", out IOperable nextMethodOp))
                throw new InterpreterRuntimeException(loop, _filePath, " foreach loop: Enumerator does not contain method 'next'");
            if (!enumerator.TryGetMember("current", out IOperable currentMethodOp))
                throw new InterpreterRuntimeException(loop, _filePath, "foreach loop: Enumerator does not contain method 'current'");


            if (!(nextMethodOp as IOperable<MethodData>).Value.TryGetOverload(0, out m) && m.MethodType != MethodType.Provider)
                throw new InterpreterRuntimeException(loop, _filePath, "foreach loop: Enumerator does not contain a next provider overload with parameter count 0");
            ProviderMethod nextMethod = m.GetProvider();

            if (!(currentMethodOp as IOperable<MethodData>).Value.TryGetOverload(0, out m) && m.MethodType != MethodType.Provider)
                throw new InterpreterRuntimeException(loop, _filePath, "foreach loop: Enumerator does not contain a current provider overload with parameter count 0");
            ProviderMethod currentMethod = m.GetProvider();

            IOperable hasNextOp = nextMethod.Invoke();
            if (hasNextOp.OperableType != ObjectType.Boolean)
                throw new InterpreterRuntimeException(loop, _filePath, "foreach loop: Value returned from $next was not a boolean");
            
            while ((bool)hasNextOp.Value)
            {
                Scoping localScope = new();
                localScope.SetOuterScope(outerScope);

                // Sets the iterator variable to current
                IOperable currentValue = currentMethod.Invoke();
                localScope.UpdateLocalBinding(loop.Identifier, currentValue);

                EnterBlock(loop.Body, localScope);

                hasNextOp = nextMethod.Invoke();
                if (hasNextOp.OperableType != ObjectType.Boolean)
                    throw new InterpreterRuntimeException(loop, _filePath, "foreach loop: Value returned from $next was not a boolean");
            }

            // If enumerator contains a dispose action, then call it
            if (enumerator.TryGetMember("dispose", out IOperable disposeMethodOp) && disposeMethodOp.OperableType == ObjectType.MethodData)
            {
                if ((disposeMethodOp as IOperable<MethodData>).Value.TryGetOverload(0, out m) && m.MethodType == MethodType.Action)
                {
                    ActionMethod disposeMethod = m.GetAction();
                    disposeMethod.Invoke();
                }
            }
        }

        public void EnterConditionalStatement(ConditionalStatementModel conditionalStatement, Scoping outerScope)
        {
            var conditionalScope = new Scoping();
            conditionalScope.SetOuterScope(outerScope);

            if (EnterIfStatement(conditionalStatement.If, conditionalScope))
                return;

            foreach (ElseIfStatementModel elseIf in conditionalStatement.ElseIf)
            {
                if (EnterElseIfStatement(elseIf, conditionalScope))
                    return;
            }

            if (conditionalStatement.Else != null)
                EnterElseStatement(conditionalStatement.Else, conditionalScope);
        }

        public bool EnterIfStatement(IfStatementModel ifStatement, Scoping scope)
        {
            IOperable condition = EnterExpression(ifStatement.Condition, scope);
            if (condition.OperableType != ObjectType.Boolean)
                throw new InterpreterRuntimeException(ifStatement, _filePath, "If clause condition was not of a boolean type");

            if ((bool)condition.Value)
            {
                EnterBlock(ifStatement.Body, scope);
                return true;
            }

            return false;
        }

        public bool EnterElseIfStatement(ElseIfStatementModel elseIfStatement, Scoping scope)
        {
            IOperable condition = EnterExpression(elseIfStatement.Condition, scope);
            if (condition.OperableType != ObjectType.Boolean)
                throw new InterpreterRuntimeException(elseIfStatement, _filePath, "Else if clause condition was not of a boolean type");

            if ((bool)condition.Value)
            {
                EnterBlock(elseIfStatement.Body, scope);
                return true;
            }

            return false;
        }

        public void EnterElseStatement(ElseStatementModel elseStatement, Scoping scope)
        {
            EnterBlock(elseStatement.Body, scope);
        }

        public void EnterAssignStatement(AssignStatementModel assignStatement, Scoping scope)
        {
            IExpressionModel expression = assignStatement.Assignee;
            AssignmentOperator operatorCombination = assignStatement.OperatorCombination;
            IOperable rightOperand = EnterExpression(expression, scope);

            IOperable __out;

            if (rightOperand is null)
                throw new InterpreterRuntimeException(assignStatement, _filePath, "Assignee has no return value");

            // Standalone identifier
            if (assignStatement.Identifier.Length is 1)
            {
                string identifier = assignStatement.Identifier[0];

                if (scope.TryGetGlobalBinding(identifier, out __out))
                {
                    if (operatorCombination == AssignmentOperator.Assign)
                    {
                        AddGlobalBinding(identifier, rightOperand, scope);
                    }
                    else
                    {
                        IOperable evaluatedResult =
                                AttemptToEvaluateExpression(_ASSIGNMENT_TO_BINARY_OPERATOR_EQUIVALENT[operatorCombination], (IBinaryOperable)__out, () => rightOperand, assignStatement);

                        AddGlobalBinding(identifier, evaluatedResult, scope);
                    }
                }
                else if (_namespace.TryGetImportedBinding(identifier, out __out))
                {
                    if (operatorCombination == AssignmentOperator.Assign)
                    {
                        _namespace.AddOrUpdateBinding(identifier, rightOperand);
                    }
                    else
                    {
                        IOperable evaluatedResult =
                                AttemptToEvaluateExpression(_ASSIGNMENT_TO_BINARY_OPERATOR_EQUIVALENT[operatorCombination], (IBinaryOperable)__out, () => rightOperand, assignStatement);

                        _namespace.AddOrUpdateBinding(identifier, evaluatedResult);
                    }
                }
                else
                {
                    if (operatorCombination == AssignmentOperator.Assign)
                    {
                        AddLocalBinding(identifier, rightOperand, scope);
                    }
                    else throw new InterpreterRuntimeException(assignStatement, _filePath, $"Tried to use assignment operator {operatorCombination} on an undefined variable");
                }

                return;
            }

            IOperable value =
                scope.TryGetGlobalBinding(assignStatement.Identifier.First(), out __out) ? __out :
                _namespace.TryGetImportedBinding(assignStatement.Identifier.First(), out __out) ? __out :
                throw new InterpreterRuntimeException(assignStatement, _filePath, $"${assignStatement.Identifier.First()} is not defined");

            RuntimeObject obj = value.OperableType switch
            {
                ObjectType.Object => (RuntimeObject)value.Value,
                ObjectType.Void => throw new InterpreterRuntimeException(expression, _filePath, $"Cannot access member on void type ${assignStatement.Identifier.First()}"),
                _ => throw new InterpreterRuntimeException(expression, _filePath, $"Attempted to access member of atom type '{value.OperableType}'")
            };

            // Traverse through the identifier path
            for (int i = 1; i < assignStatement.Identifier.Length - 1; i++)
            {
                if (TryGetRuntimeObjectMember(obj, assignStatement.Identifier[i], out IOperable prop))
                {
                    obj = prop.OperableType switch
                    {
                        ObjectType.Object => (RuntimeObject)prop.Value,
                        ObjectType.Void => throw new InterpreterRuntimeException(assignStatement, _filePath, $"Member ${assignStatement.Identifier[i - 1]}->{assignStatement.Identifier[i]} is defined but is null reference"),
                        _ => throw new InterpreterRuntimeException(expression, _filePath, $"Attempted to access member of atom type '{prop.OperableType}'")
                    };
                }
                else
                    throw new InterpreterRuntimeException(assignStatement, _filePath, $"Member ${string.Join("->", assignStatement.Identifier[0..(i + 1)])} is not defined");
            }

            // If the member does not exist, add new binding
            string idLast = assignStatement.Identifier.Last();
            if (!TryGetRuntimeObjectMember(obj, idLast, out IOperable member))
            {
                if (operatorCombination != AssignmentOperator.Assign)
                    throw new InterpreterRuntimeException(assignStatement, _filePath, $"Tried to use operator {operatorCombination} on undefined member ${string.Join("->", assignStatement.Identifier)}");

                TrySetRuntimeObjectMember(obj, idLast, rightOperand);
                return;
            }
            // Otherwise overwrite or update the member
            else
            {
                if (operatorCombination == AssignmentOperator.Assign)
                {
                    TrySetRuntimeObjectMember(obj, idLast, rightOperand);
                    return;
                }
                else
                {
                    IOperable evaluatedResult = AttemptToEvaluateExpression(_ASSIGNMENT_TO_BINARY_OPERATOR_EQUIVALENT[operatorCombination], (IBinaryOperable)member, () => rightOperand, assignStatement);
                    TrySetRuntimeObjectMember(obj, idLast, evaluatedResult);
                    return;
                }
            }

            throw new InterpreterRuntimeException(assignStatement, _filePath, $"${assignStatement.Identifier.First()} is not defined in current scope");
        }

        public void EnterUseStatementModel(UseStatementModel statement, Scoping scope)
        {
            string nsPath = statement.PathToNamespace[0];

            if (!_environment.TryGetNamespace(nsPath, out Namespace ns))
                throw new InterpreterRuntimeException(statement, _filePath, $"Namespace '{nsPath}' does not exist in current environment");

            _namespace.Import(ns);
        }

        public void EnterTryCatchStatement(TryCatchStatementModel statement, Scoping scope)
        {
            try
            {
                var tryBlockScope = new Scoping();
                tryBlockScope.SetOuterScope(scope);
                EnterBlock(statement.Try.Body, tryBlockScope);
            }
            catch (LanguageException le) // Catches exceptions thrown by language interpreter
            {
                var catchBlockScope = new Scoping();
                catchBlockScope.SetOuterScope(scope);
                AddLocalBinding(statement.Catch.ArgumentName, le.Argument, catchBlockScope);
                EnterBlock(statement.Catch.Body, catchBlockScope);
            }
            catch (Exception e) // Catches exceptions thrown by .NET
            {
                var catchBlockScope = new Scoping();
                catchBlockScope.SetOuterScope(scope);

                // Map the .NET exception to an object readable by the interpreter
                var dotnetExceptionRuntimeObject = new RuntimeObject();
                TrySetRuntimeObjectMember(dotnetExceptionRuntimeObject, "message", new StringOperable($"Core exception: {e.Message}"));
                TrySetRuntimeObjectMember(dotnetExceptionRuntimeObject, "messageFull", new StringOperable($"Core exception: {e}"));

                AddLocalBinding(statement.Catch.ArgumentName, (ObjectOperable)dotnetExceptionRuntimeObject, catchBlockScope);

                EnterBlock(statement.Catch.Body, catchBlockScope);
            }
        }

        public void EnterThrowStatement(ThrowStatement statement, Scoping scope) =>
            throw new LanguageException(EnterExpression(statement.Expression, scope), statement, _filePath);

        public ArrayObjectOperable EnterArrayLiteralNotation(ArrayLiteralNotationModel expression, Scoping scope)
        {
            if (expression.IsAllocSyntax)
            {
                if (expression.Arguments.Length != 1)
                    throw new InterpreterRuntimeException(expression, _filePath, "Array alloc statement cannot contain more than one argument");

                IOperable sizeArg = EnterExpression(expression.Arguments[0], scope);
                return sizeArg.OperableType switch
                {
                    ObjectType.ArbitraryBitInteger => ArrayObjectOperable.Allocate((ulong)(sizeArg as IOperable<BigInteger>).Value),
                    ObjectType.UnsignedByte => ArrayObjectOperable.Allocate((sizeArg as IOperable<byte>).Value),
                    _ => throw new InterpreterRuntimeException(expression, _filePath, $"Illegal argument type '{sizeArg.OperableType}'"),
                };
            }

            if (expression.Arguments is null)
                return new ArrayObjectOperable();

            return new ArrayObjectOperable(expression.Arguments.Select(arg => EnterExpression(arg, scope)).ToArray());
        }

        public DictionaryObjectOperable EnterDictionaryObjectOperable(DictionaryLiteralNotation expression, Scoping scope)
        {
            if (expression.Arguments is null)
                return new DictionaryObjectOperable();

            int i = 0;
            var dictionary = new Dictionary<IOperable, IOperable>();
            foreach (var (keyExpr, valueExpr, start, stop) in expression.Arguments)
            {
                IOperable key = EnterExpression(keyExpr, scope);
                IOperable value = EnterExpression(valueExpr, scope);
                if (!dictionary.TryAdd(key, value))
                    throw new InterpreterRuntimeException(start, stop, _filePath, $"Could not allocate dictionary due to duplicate key at index {i}");
                i++;
            }

            return new DictionaryObjectOperable(dictionary);
        }
    }
}
