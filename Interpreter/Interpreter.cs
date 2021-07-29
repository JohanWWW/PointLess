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
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public class Interpreter
    {
        private readonly Namespace _namespace;
        private readonly RuntimeEnvironment _environment;
        private readonly string _filePath;

        private readonly IReadOnlyDictionary<AssignmentOperator, BinaryOperator> _ASSIGNMENT_TO_BINARY_OPERATOR_EQUIVALENT = new Dictionary<AssignmentOperator, BinaryOperator>
        {
            [AssignmentOperator.AddAssign] = BinaryOperator.Plus,
            [AssignmentOperator.SubAssign] = BinaryOperator.Minus,
            [AssignmentOperator.MultAssign] = BinaryOperator.Mult,
            [AssignmentOperator.DivAssign] = BinaryOperator.Div,
            [AssignmentOperator.ModAssign] = BinaryOperator.Mod,
            [AssignmentOperator.AndAssign] = BinaryOperator.LogicalAnd,
            [AssignmentOperator.BitwiseAndAssign] = BinaryOperator.BitwiseAnd,
            [AssignmentOperator.OrAssign] = BinaryOperator.LogicalOr,
            [AssignmentOperator.BitwiseOrAssign] = BinaryOperator.BitwiseOr,
            [AssignmentOperator.XorAssign] = BinaryOperator.LogicalXOr,
            [AssignmentOperator.BitwiseXorAssign] = BinaryOperator.BitwiseXOr,
            [AssignmentOperator.ShiftLeftAssign] = BinaryOperator.ShiftLeft,
            [AssignmentOperator.ShiftRightAssign] = BinaryOperator.ShiftRight
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
        /// Evaluates a binary expression. The right operand should be provided as a lambda that returns dynamic.
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
        /// <param name="a">An evaluated operand</param>
        /// <param name="b">An unevaluated operand</param>
        /// <returns>Returns the evaluated result of the provided operands</returns>
        /// <exception cref="RuntimeBinderException"></exception>
        /// <exception cref="DivideByZeroException">If attempt to divide by zero</exception>
        private static dynamic EvaluateBinaryExpression(BinaryOperator op, dynamic a, Func<dynamic> b) => op switch
        {
            BinaryOperator.Plus =>                  a + b(),
            BinaryOperator.Minus =>                 a - b(),
            BinaryOperator.Mult =>                  a * b(),
            BinaryOperator.Div =>                   a / b(),
            BinaryOperator.Mod =>                   a % b(),
            BinaryOperator.Equal =>                 a == b(),
            BinaryOperator.NotEqual =>              a != b(),
            BinaryOperator.LessThan =>              a < b(),
            BinaryOperator.LessThanOrEqual =>       a <= b(),
            BinaryOperator.GreaterThan =>           a > b(),
            BinaryOperator.GreaterThanOrEqual =>    a >= b(),
            BinaryOperator.LogicalAnd =>            a && b(),
            BinaryOperator.LogicalXOr =>            a ^ b(),
            BinaryOperator.LogicalOr =>             a || b(),
            BinaryOperator.BitwiseAnd =>            a & b(),
            BinaryOperator.BitwiseXOr =>            a ^ b(),
            BinaryOperator.BitwiseOr =>             a | b(),
            BinaryOperator.ShiftLeft =>             a << (int)b(),
            BinaryOperator.ShiftRight =>            a >> (int)b(),
            _ =>                                    throw new NotImplementedException(),
        };

        private static void AddLocalBinding(string identifier, object value, Scoping scope)
        {
            if (value is Method method)
            {
                scope.AddLocalBinding(identifier, new MethodData(method));
                return;
            }
            scope.AddLocalBinding(identifier, value);
        }

        private static void AddGlobalBinding(string identifier, object value, Scoping scope)
        {
            if (value is Method method)
            {
                scope.SetGlobalBinding(identifier, new MethodData(method));
                return;
            }
            scope.SetGlobalBinding(identifier, value);
        }

        private dynamic AttemptToEvaluateExpression(BinaryExpressionModel expression, Scoping scope)
        {
            BinaryOperator op = expression.Operator;

            dynamic a = EnterExpression(expression.LeftExpression, scope);
            
            return AttemptToEvaluateExpression(op, a, (Func<dynamic>)(() => EnterExpression(expression.RightExpression, scope)), expression);
        }

        /// <summary>
        /// Attempt to evaluate given expression. Throw exception if it fails.
        /// </summary>
        private dynamic AttemptToEvaluateExpression(BinaryOperator op, dynamic a, Func<dynamic> b, IModel runtimeModel)
        {
            dynamic bEval = null;
            try
            {
                return bEval = EvaluateBinaryExpression(op, a, (Func<dynamic>)(() => bEval = b()));
            }
            catch (DivideByZeroException e)
            {
                throw new InterpreterRuntimeException(runtimeModel, _filePath, "Attempted to divide by zero", e);
            }
            catch (RuntimeBinderException e)
            {
                throw new InterpreterRuntimeException(runtimeModel, _filePath,
                    $"Operator {op} cannot be applied on operands of type {((object)a).GetType()} and {((object)bEval).GetType()}", e);
            }
            catch (MethodOverloadException e)
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
            if (statement is AssignStatementModel)
            {
                EnterAssignStatement(statement as AssignStatementModel, scope);
                return;
            }

            if (statement is ConditionalStatementModel)
            {
                EnterConditionalStatement(statement as ConditionalStatementModel, scope);
                return;
            }

            if (statement is FunctionCallStatement)
            {
                EnterFunctionCallStatement(statement as FunctionCallStatement, scope);
                return;
            }

            if (statement is ILoopStatementModel)
            {
                EnterLoopStatement(statement as ILoopStatementModel, scope);
                return;
            }

            if (statement is UseStatementModel)
            {
                EnterUseStatementModel(statement as UseStatementModel, scope);
                return;
            }

            if (statement is TryCatchStatementModel)
            {
                EnterTryCatchStatement(statement as TryCatchStatementModel, scope);
                return;
            }

            if (statement is ThrowStatement)
            {
                EnterThrowStatement(statement as ThrowStatement, scope);
                return;
            }
        }

        public dynamic EnterExpression(IExpressionModel expression, Scoping scope)
        {
            if (expression is LiteralExpressionModel)
                return EnterLiteralExpression(expression as LiteralExpressionModel, scope);

            if (expression is IdentifierExpressionModel)
                return EnterIdentifierExpression(expression as IdentifierExpressionModel, scope);

            if (expression is BinaryExpressionModel)
                return EnterBinaryExpression(expression as BinaryExpressionModel, scope);

            if (expression is ITernaryExpressionModel)
                return EnterTernaryExpression(expression as ITernaryExpressionModel, scope);

            if (expression is IFunctionModel)
                return EnterMethodStatement(expression as IFunctionModel, scope);

            if (expression is FunctionCallStatement)
                return EnterFunctionCallStatement(expression as FunctionCallStatement, scope);

            if (expression is ObjectInitializationExpressionModel)
                return EnterObjectInitializationExpression(expression as ObjectInitializationExpressionModel, scope);

            throw new NotImplementedException();
        }

        public dynamic EnterBinaryExpression(BinaryExpressionModel expression, Scoping scope) => AttemptToEvaluateExpression(expression, scope);

        public dynamic EnterTernaryExpression(ITernaryExpressionModel expression, Scoping scope)
        {
            if (expression is ConditionalTernaryExpressionModel conditionalExpression)
            {
                return EnterConditionalTernaryExpression(conditionalExpression, scope);
            }

            throw new NotImplementedException();
        }

        public dynamic EnterConditionalTernaryExpression(ConditionalTernaryExpressionModel expression, Scoping scope)
        {
            dynamic conditionEval = EnterExpression(expression.ConditionExpression, scope);
            if (!(conditionEval is bool))
                throw new InterpreterRuntimeException(expression, _filePath, "Condition part of ternary expression was not a boolean expression");

            if ((bool)conditionEval)
            {
                return EnterExpression(expression.TrueExpression, scope);
            }
            else
            {
                return EnterExpression(expression.FalseExpression, scope);
            }

        }

        public dynamic EnterLiteralExpression(LiteralExpressionModel expression, Scoping scope) => expression.Value;

        public dynamic EnterIdentifierExpression(IdentifierExpressionModel expression, Scoping scope)
        {
            if (expression.Identifier.Length is 1)
            {
                if (scope.ContainsGlobalBinding(expression.Identifier[0]))
                    return scope.GetGlobalValue(expression.Identifier[0]);
                else if (_namespace.GetImportedBindings().ContainsKey(expression.Identifier[0]))
                    return _namespace.GetImportedValue(expression.Identifier[0]);

                throw new InterpreterRuntimeException(expression, _filePath, $"${expression.Identifier[0]} is not defined in current scope");
            }

            RuntimeObject obj =
                scope.ContainsGlobalBinding(expression.Identifier[0]) ?
                    scope.GetGlobalValue(expression.Identifier[0]) :
                _namespace.GetImportedBindings().ContainsKey(expression.Identifier[0]) ?
                    _namespace.GetImportedValue(expression.Identifier[0]) :
                null;

            if (obj != null)
            {
                for (int i = 1; i < expression.Identifier.Length - 1; i++)
                {
                    if (obj.TryGetMember(new RuntimeObject.GetterBinder(expression.Identifier[i]), out object prop))
                        obj = (RuntimeObject)prop;
                    else
                        throw new InterpreterRuntimeException(expression, _filePath, $"Member ${expression.Identifier[i - 1]}->{expression.Identifier[i]} is not a defined");
                }

                if (obj.TryGetMember(new RuntimeObject.GetterBinder(expression.Identifier.Last()), out dynamic value))
                    return value;

                throw new InterpreterRuntimeException(expression, _filePath, $"Member ${expression.Identifier[expression.Identifier.Length - 2]}->{expression.Identifier.Last()} is not defined");
            }

            throw new InterpreterRuntimeException(expression, _filePath, $"Member ${expression.Identifier[0]} is not defined in current scope");

        }

        public dynamic EnterObjectInitializationExpression(ObjectInitializationExpressionModel expression, Scoping scope)
        {
            var runtimeObject = new RuntimeObject();

            foreach (ObjectPropertyExpressionModel property in expression.Properties)
            {
                dynamic value = EnterExpression(property.Value, scope);
                runtimeObject.TrySetMember(new RuntimeObject.SetterBinder(property.Identifier), value);
            }

            return runtimeObject;
        }

        public dynamic EnterFunctionCallStatement(FunctionCallStatement functionCall, Scoping scope)
        {
            dynamic method = null;

            // Variable is object
            if (functionCall.IdentifierPath.Length > 1)
            {
                RuntimeObject obj =
                    scope.ContainsGlobalBinding(functionCall.IdentifierPath[0]) ?
                        scope.GetGlobalValue(functionCall.IdentifierPath[0]) :
                    _namespace.GetImportedBindings().ContainsKey(functionCall.IdentifierPath[0]) ?
                        _namespace.GetImportedValue(functionCall.IdentifierPath[0]) :
                    null;

                if (obj is null)
                    throw new InterpreterRuntimeException(functionCall, _filePath, $"Member ${string.Join("->", functionCall.IdentifierPath)} is not defined");

                for (int i = 1; i < functionCall.IdentifierPath.Length - 1; i++)
                {
                    if (obj.TryGetMember(new RuntimeObject.GetterBinder(functionCall.IdentifierPath[i]), out object prop))
                        obj = (RuntimeObject)prop;
                    else
                        throw new InterpreterRuntimeException(functionCall, _filePath, $"Member ${string.Join("->", functionCall.IdentifierPath[0..(i - 1)])} is not defined");
                }

                if (obj.TryGetMember(new RuntimeObject.GetterBinder(functionCall.IdentifierPath.Last()), out method))
                {
                }
                else
                    throw new InterpreterRuntimeException(functionCall, _filePath, $"Member ${string.Join("->", functionCall.IdentifierPath)} is not defined");
            }
            // Variable is single variable
            else if (scope.ContainsGlobalBinding(functionCall.IdentifierPath[0]))
            {
                method = scope.GetGlobalValue(functionCall.IdentifierPath[0]);
            }
            else if (_namespace.GetImportedBindings().ContainsKey(functionCall.IdentifierPath[0]))
            {
                method = _namespace.GetImportedValue(functionCall.IdentifierPath[0]);
            }

            if (method is null)
                throw new InterpreterRuntimeException(functionCall, _filePath, $"Method ${string.Join("->", functionCall.IdentifierPath)} is not defined in current scope");


            IExpressionModel[] args = functionCall.Arguments;
            int argumentCount = functionCall.Arguments?.Length ?? 0;
            
            if (method is MethodData md)
            {
                if (!md.ContainsOverload(argumentCount))
                    throw new InterpreterRuntimeException(functionCall, _filePath, $"Argument count mismatch: Could not find suitable overload with parameter count {argumentCount}.");

                Method overload = md.GetOverload(argumentCount);

                switch (overload.MethodType)
                {
                    case MethodType.Function:
                        {
                            dynamic[] evalArgs = new dynamic[argumentCount];
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
                            dynamic[] evalArgs = new dynamic[argumentCount];
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
                        throw new NotImplementedException("Method type not implemented");
                }
            }
            else if (method is Method overload)
            {
                switch (overload.MethodType)
                {
                    case MethodType.Function:
                        {
                            dynamic[] evalArgs = new dynamic[argumentCount];
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
                            dynamic[] evalArgs = new dynamic[argumentCount];
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
                        throw new NotImplementedException("Method type not implemented");
                }
            }

            throw new NotImplementedException("The provided method type is not implemented");
        }

        public dynamic EnterMethodStatement(IFunctionModel method, Scoping scope)
        {
            if (method is ActionStatementModel)
                return EnterActionStatement(method as ActionStatementModel, scope);

            if (method is FunctionStatementModel)
                return EnterFunctionStatement(method as FunctionStatementModel, scope);

            if (method is ProviderStatementModel)
                return EnterProviderStatement(method as ProviderStatementModel, scope);

            if (method is ConsumerStatementModel)
                return EnterConsumerStatement(method as ConsumerStatementModel, scope);

            if (method is LambdaFunctionStatementModel)
                return EnterLambdaStatement(method as LambdaFunctionStatementModel, scope);

            if (method is NativeConsumerStatementModel)
                return EnterNativeConsumerStatement(method as NativeConsumerStatementModel, scope);

            if (method is NativeProviderStatementModel)
                return EnterNativeProviderStatement(method as NativeProviderStatementModel, scope);

            if (method is NativeFunctionStatementModel)
                return EnterNativeFunctionStatement(method as NativeFunctionStatementModel, scope);

            if (method is NativeActionStatementModel)
                return EnterNativeActionStatement(method as NativeActionStatementModel, scope);

            throw new NotImplementedException();
        }

        public dynamic EnterActionStatement(ActionStatementModel actionStatement, Scoping outerScope)
        {
            return new Method(
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
        }

        public dynamic EnterFunctionStatement(FunctionStatementModel functionStatement, Scoping outerScope)
        {
            return new Method(
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
                        dynamic argValue = args[i];

                        AddLocalBinding(argIdentifier, argValue, blockScope);
                    }

                    // Execute the block
                    EnterBlock(functionStatement.Body, blockScope);

                    // Evaluate return expression
                    return EnterExpression(functionStatement.Return, blockScope);
                })
            );
        }

        public dynamic EnterProviderStatement(ProviderStatementModel providerStatement, Scoping outerScope)
        {
            return new Method(
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
        }

        public dynamic EnterConsumerStatement(ConsumerStatementModel consumerStatement, Scoping outerScope)
        {
            return new Method(
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
                        dynamic argValue = args[i];

                        AddLocalBinding(argIdentifier, argValue, localScope);
                    }

                    // Evaluate function body
                    EnterBlock(consumerStatement.Body, localScope);
                })
            );
        }

        public dynamic EnterLambdaStatement(LambdaFunctionStatementModel lambdaStatement, Scoping outerScope)
        {
            string[] parameters = lambdaStatement.Parameters;

            if (parameters is null)
            {
                if (lambdaStatement.IsModeReturn)
                {
                    return new Method(
                        parameterCount: 0,
                        type: MethodType.Provider,
                        method: new ProviderMethod(() => EnterExpression(lambdaStatement.Mode as IExpressionModel, outerScope))
                    );
                }
                else
                {
                    return new Method(
                        parameterCount: 0,
                        type: MethodType.Action,
                        method: new ActionMethod(() => EnterAssignStatement(lambdaStatement.Mode as AssignStatementModel, outerScope))
                    );
                }
            }
            else
            {
                if (lambdaStatement.IsModeReturn)
                {
                    return new Method(
                        parameterCount: parameters.Length,
                        type: MethodType.Function,
                        method: new FunctionMethod(args =>
                        {
                            var localScope = new Scoping();
                            localScope.SetOuterScope(outerScope);

                            for (int i = 0; i < parameters.Length; i++)
                            {
                                string argId = parameters[i];
                                dynamic argVal = args[i];

                                AddLocalBinding(argId, argVal, localScope);
                            }

                            return EnterExpression(lambdaStatement.Mode as IExpressionModel, localScope);
                        })
                    );
                }
                else
                {
                    return new Method(
                        parameterCount: parameters.Length,
                        type: MethodType.Consumer,
                        method: new ConsumerMethod(args =>
                        {
                            var localScope = new Scoping();
                            localScope.SetOuterScope(outerScope);

                            for (int i = 0; i < parameters.Length; i++)
                            {
                                string argId = parameters[i];
                                dynamic argVal = args[i];

                                AddLocalBinding(argId, argVal, localScope);
                            }

                            EnterAssignStatement(lambdaStatement.Mode as AssignStatementModel, localScope);
                        })
                    );
                }
            }
        }

        public dynamic EnterNativeConsumerStatement(NativeConsumerStatementModel consumerStatement, Scoping outerScope)
        {
            return new Method(
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
                        dynamic argValue = args[i];

                        AddLocalBinding(argIdentifier, argValue, localScope);
                    }

                    consumerStatement.NativeImplementation(args);
                })
            );
        }

        public dynamic EnterNativeProviderStatement(NativeProviderStatementModel providerStatement, Scoping outerScope)
        {
            return new Method(
                parameterCount: 0,
                type: MethodType.Provider,
                method: new ProviderMethod(() =>
                {
                    var localScope = new Scoping();
                    localScope.SetOuterScope(outerScope);

                    return providerStatement.NativeImplementation();
                })
            );
        }

        public dynamic EnterNativeFunctionStatement(NativeFunctionStatementModel functionStatement, Scoping outerScope)
        {
            return new Method(
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
                        dynamic argValue = args[i];

                        AddLocalBinding(argIdentifier, argValue, localScope);
                    }

                    return functionStatement.NativeImplementation(args);
                })
            );
        }

        public dynamic EnterNativeActionStatement(NativeActionStatementModel actionStatement, Scoping outerScope)
        {
            return new Method(
                parameterCount: 0,
                type: MethodType.Action,
                method: new ActionMethod(() => actionStatement.NativeImplementation())
            );
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

        public void EnterLoopStatement(ILoopStatementModel loop, Scoping scope)
        {
            if (loop is WhileLoopStatement)
            {
                EnterWhileLoopStatement(loop as WhileLoopStatement, scope);
                return;
            }
        }

        public void EnterWhileLoopStatement(WhileLoopStatement loop, Scoping outerScope)
        {
            var loopScope = new Scoping();
            loopScope.SetOuterScope(outerScope);

            while (EnterExpression(loop.Condition, loopScope))
            {
                EnterBlock(loop.Body, loopScope);
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
            if (EnterExpression(ifStatement.Condition, scope))
            {
                EnterBlock(ifStatement.Body, scope);
                return true;
            }

            return false;
        }

        public bool EnterElseIfStatement(ElseIfStatementModel elseIfStatement, Scoping scope)
        {
            if (EnterExpression(elseIfStatement.Condition, scope))
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
            dynamic rightOperand = EnterExpression(expression, scope);

            // Standalone identifier
            if (assignStatement.Identifier.Length is 1)
            {
                string identifier = assignStatement.Identifier[0];

                if (scope.ContainsGlobalBinding(identifier))
                {
                    if (operatorCombination == AssignmentOperator.Assign)
                    {
                        AddGlobalBinding(identifier, rightOperand, scope);
                    }
                    else
                    {
                        dynamic evaluatedResult =
                                AttemptToEvaluateExpression(_ASSIGNMENT_TO_BINARY_OPERATOR_EQUIVALENT[operatorCombination], scope.GetGlobalValue(identifier), (Func<dynamic>)(() => rightOperand), assignStatement);

                        AddGlobalBinding(identifier, evaluatedResult, scope);
                    }
                }
                else if (_namespace.GetImportedBindings().ContainsKey(identifier))
                {
                    if (operatorCombination == AssignmentOperator.Assign)
                    {
                        _namespace.GetImportedBindings().Add(identifier, rightOperand);
                    }
                    else
                    {
                        dynamic evaluatedResult =
                                AttemptToEvaluateExpression(_ASSIGNMENT_TO_BINARY_OPERATOR_EQUIVALENT[operatorCombination], _namespace.GetImportedValue(identifier), (Func<dynamic>)(() => rightOperand), assignStatement);

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

            RuntimeObject obj =
                    scope.ContainsGlobalBinding(assignStatement.Identifier[0]) ?
                        scope.GetGlobalValue(assignStatement.Identifier[0]) :
                    _namespace.GetImportedBindings().ContainsKey(assignStatement.Identifier[0]) ?
                        _namespace.GetImportedValue(assignStatement.Identifier[0]) :
                    null;

            if (obj != null)
            {
                // Traverse through the identifier path
                for (int i = 1; i < assignStatement.Identifier.Length - 1; i++)
                {
                    if (obj.TryGetMember(new RuntimeObject.GetterBinder(assignStatement.Identifier[i]), out object prop))
                        obj = (RuntimeObject)prop;
                    else
                        throw new InterpreterRuntimeException(assignStatement, _filePath, $"Member ${string.Join("->", assignStatement.Identifier[0..(i + 1)])} is not defined");
                }

                // If the member does not exist, add new binding
                string idLast = assignStatement.Identifier.Last();
                if (!obj.TryGetMember(new RuntimeObject.GetterBinder(idLast), out object member))
                {
                    if (operatorCombination != AssignmentOperator.Assign)
                        throw new InterpreterRuntimeException(assignStatement, _filePath, $"Tried to use operator {operatorCombination} on undefined member ${string.Join("->", assignStatement.Identifier)}");

                    obj.TrySetMember(new RuntimeObject.SetterBinder(idLast), rightOperand);
                    return;
                }
                // Otherwise overwrite or update the member
                else
                {
                    var setterBinder = new RuntimeObject.SetterBinder(idLast);
                    if (operatorCombination == AssignmentOperator.Assign)
                    {
                        obj.TrySetMember(setterBinder, rightOperand);
                        return;
                    }
                    else
                    {
                        dynamic evaluatedResult = AttemptToEvaluateExpression(_ASSIGNMENT_TO_BINARY_OPERATOR_EQUIVALENT[operatorCombination], member, (Func<dynamic>)(() => rightOperand), assignStatement);
                        obj.TrySetMember(setterBinder, evaluatedResult);
                        return;
                    }
                }
            }

            throw new InterpreterRuntimeException(assignStatement, _filePath, $"${assignStatement.Identifier[0]} is not defined in current scope");
        }

        public void EnterUseStatementModel(UseStatementModel statement, Scoping scope)
        {
            string nsPath = statement.PathToNamespace[0];
            try
            {
                Namespace ns = _environment.GetNamespace(nsPath);
                _namespace.Import(ns);
            }
            catch (KeyNotFoundException e)
            {
                throw new InterpreterRuntimeException(statement, _filePath, $"Could not find namespace '{nsPath}' in current environment", e);
            }

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
                dotnetExceptionRuntimeObject.TrySetMember(new RuntimeObject.SetterBinder("message"), $"Core exception: {e.Message}");
                dotnetExceptionRuntimeObject.TrySetMember(new RuntimeObject.SetterBinder("messageFull"), $"Core exception: {e}");

                AddLocalBinding(statement.Catch.ArgumentName, dotnetExceptionRuntimeObject, catchBlockScope);

                EnterBlock(statement.Catch.Body, catchBlockScope);
            }
        }

        public void EnterThrowStatement(ThrowStatement statement, Scoping scope) =>
            throw new LanguageException(EnterExpression(statement.Expression, scope), statement, _filePath);
    }
}
