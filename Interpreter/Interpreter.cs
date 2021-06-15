using Interpreter.Environment;
using Interpreter.Models;
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

        public Namespace Namespace => _namespace;

        public Interpreter(Namespace ns, RuntimeEnvironment environment)
        {
            _namespace = ns;
            _environment = environment;
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

            if (expression is IFunctionModel)
                return EnterMethodStatement(expression as IFunctionModel, scope);

            if (expression is FunctionCallStatement)
                return EnterFunctionCallStatement(expression as FunctionCallStatement, scope);

            if (expression is ListInitializationExpressionModel)
                return EnterListInitializationExpression(expression as ListInitializationExpressionModel, scope);

            if (expression is ObjectInitializationExpressionModel)
                return EnterObjectInitializationExpression(expression as ObjectInitializationExpressionModel, scope);

            throw new NotImplementedException();
        }

        public dynamic EnterBinaryExpression(BinaryExpressionModel expression, Scoping scope)
        {
            dynamic leftExpressionEval = EnterExpression(expression.LeftExpression, scope);
            dynamic rightExpressionEval = EnterExpression(expression.RightExpression, scope);

            switch (expression.Operator)
            {
                case BinaryOperator.Plus:
                    return leftExpressionEval + rightExpressionEval;
                case BinaryOperator.Minus:
                    return leftExpressionEval - rightExpressionEval;
                case BinaryOperator.Mult:
                    return leftExpressionEval * rightExpressionEval;
                case BinaryOperator.Div:
                    return leftExpressionEval / rightExpressionEval;
                case BinaryOperator.Mod:
                    return leftExpressionEval % rightExpressionEval;
                case BinaryOperator.Equal:
                    return leftExpressionEval == rightExpressionEval;
                case BinaryOperator.NotEqual:
                    return leftExpressionEval != rightExpressionEval;
                case BinaryOperator.LessThan:
                    return leftExpressionEval < rightExpressionEval;
                case BinaryOperator.LessThanOrEqual:
                    return leftExpressionEval <= rightExpressionEval;
                case BinaryOperator.GreaterThan:
                    return leftExpressionEval > rightExpressionEval;
                case BinaryOperator.GreaterThanOrEqual:
                    return leftExpressionEval >= rightExpressionEval;
                case BinaryOperator.LogicalAnd:
                    return leftExpressionEval && rightExpressionEval;
                case BinaryOperator.LogicalXOr:
                    return leftExpressionEval ^ rightExpressionEval;
                case BinaryOperator.LogicalOr:
                    return leftExpressionEval || rightExpressionEval;
                case BinaryOperator.BitwiseAnd:
                    return leftExpressionEval & rightExpressionEval;
                case BinaryOperator.BitwiseXOr:
                    return leftExpressionEval ^ rightExpressionEval;
                case BinaryOperator.BitwiseOr:
                    return leftExpressionEval | rightExpressionEval;
                case BinaryOperator.ShiftLeft:
                    return leftExpressionEval << rightExpressionEval;
                case BinaryOperator.ShiftRight:
                    return leftExpressionEval >> rightExpressionEval;
                default:
                    throw new NotImplementedException();
            }
        }

        public dynamic EnterLiteralExpression(LiteralExpressionModel expression, Scoping scope)
        {
            return expression.Value;
        }

        public dynamic EnterListInitializationExpression(ListInitializationExpressionModel expression, Scoping scope)
        {
            var list = new List<dynamic>();
            foreach (IExpressionModel elementExpression in expression.Elements)
            {
                list.Add(EnterExpression(elementExpression, scope));
            }
            return list;
        }

        public dynamic EnterIdentifierExpression(IdentifierExpressionModel expression, Scoping scope)
        {
            if (expression.Identifier.Length is 1)
            {
                if (scope.ContainsBacktrack(expression.Identifier[0]))
                    return scope.GetBacktrackedVariable(expression.Identifier[0]);
                else if (_namespace.GetImportedBindings().ContainsKey(expression.Identifier[0]))
                    return _namespace.GetImportedBinding(expression.Identifier[0]);

                throw new KeyNotFoundException("Could not find variable named " + expression.Identifier[0]);
            }

            RuntimeObject obj =
                scope.ContainsBacktrack(expression.Identifier[0]) ?
                    scope.GetBacktrackedVariable(expression.Identifier[0]) :
                _namespace.GetImportedBindings().ContainsKey(expression.Identifier[0]) ?
                    _namespace.GetImportedBinding(expression.Identifier[0]) :
                null;

            if (obj != null)
            {
                for (int i = 1; i < expression.Identifier.Length - 1; i++)
                {
                    if (obj.TryGetMember(new RuntimeObject.GetterBinder(expression.Identifier[i]), out object prop))
                        obj = (RuntimeObject)prop;
                    else
                        throw new KeyNotFoundException($"Could not find property '{expression.Identifier[i]}' in '{expression.Identifier[i - 1]}'");
                }

                if (obj.TryGetMember(new RuntimeObject.GetterBinder(expression.Identifier.Last()), out dynamic value))
                    return value;

                throw new KeyNotFoundException($"Could not find property '{expression.Identifier.Last()}' in '{expression.Identifier[expression.Identifier.Length - 2]}'");
            }

            throw new KeyNotFoundException("Could not find variable named " + string.Concat(expression.Identifier));
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
                    scope.ContainsBacktrack(functionCall.IdentifierPath[0]) ?
                        scope.GetBacktrackedVariable(functionCall.IdentifierPath[0]) :
                    _namespace.GetImportedBindings().ContainsKey(functionCall.IdentifierPath[0]) ?
                        _namespace.GetImportedBinding(functionCall.IdentifierPath[0]) :
                    null;

                for (int i = 1; i < functionCall.IdentifierPath.Length - 1; i++)
                {
                    if (obj.TryGetMember(new RuntimeObject.GetterBinder(functionCall.IdentifierPath[i]), out object prop))
                        obj = (RuntimeObject)prop;
                    else
                        throw new KeyNotFoundException($"Could not find property '{functionCall.IdentifierPath[i]}' in '{functionCall.IdentifierPath[i - 1]}'");
                }

                if (obj.TryGetMember(new RuntimeObject.GetterBinder(functionCall.IdentifierPath.Last()), out method))
                {
                }
                else
                    throw new KeyNotFoundException($"Could not find property '{functionCall.IdentifierPath.Last()}' in '{functionCall.IdentifierPath[functionCall.IdentifierPath.Length - 2]}'");
            }
            // Variable is single variable
            else if (scope.ContainsBacktrack(functionCall.IdentifierPath[0]))
            {
                method = scope.GetBacktrackedVariable(functionCall.IdentifierPath[0]);
            }
            else if (_namespace.GetImportedBindings().ContainsKey(functionCall.IdentifierPath[0]))
            {
                method = _namespace.GetImportedBinding(functionCall.IdentifierPath[0]);
            }

            if (method is null)
                throw new KeyNotFoundException($"The method type '{string.Join(".", functionCall.IdentifierPath)}' does not exist or it exists in another namespace");

            if (functionCall.Arguments is null || functionCall.Arguments.Arguments is null)
            {
                if (method is Action)
                {
                    method();
                    return null; // An action does not return value
                }
                else if (method is Func<dynamic>)
                    return method();
                else
                    throw new NotImplementedException();
            }

            var args = new List<dynamic>();
            foreach (IExpressionModel expression in functionCall.Arguments.Arguments)
            {
                args.Add(EnterExpression(expression, scope));
            }

            if (method is Func<IList<dynamic>, dynamic>)
                return method(args);
            else if (method is Action<IList<dynamic>>)
            {
                method(args);
                return null;
            }

            throw new NotImplementedException();
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
            // Late evaluation
            return new Action(() =>
            {
                // Create scope for function body
                var blockScope = new Scoping();

                // Chain together with outer scope
                blockScope.SetLeftScope(outerScope);

                // Evaluate block
                EnterBlock(actionStatement.Body, blockScope);
            });
        }

        public dynamic EnterFunctionStatement(FunctionStatementModel functionStatement, Scoping outerScope)
        {
            // Create a late evaluation
            return new Func<IList<dynamic>, dynamic>(args =>
            {
                var parameters = functionStatement.Parameters.Parameters;

                // Create scope for function body
                var blockScope = new Scoping();

                // Chain together with outer scope
                blockScope.SetLeftScope(outerScope);

                // Put the argument values in this local scope
                for (int i = 0; i < parameters.Count; i++)
                {
                    string argIdentifier = parameters.ElementAt(i);
                    dynamic argValue = args[i];

                    blockScope.AddLocalVariable(argIdentifier, argValue);
                }

                // Execute the block
                EnterBlock(functionStatement.Body, blockScope);

                // Evaluate return expression
                return EnterExpression(functionStatement.Return, blockScope);
            });
        }

        public dynamic EnterProviderStatement(ProviderStatementModel providerStatement, Scoping outerScope)
        {
            // Create late evaluation
            return new Func<dynamic>(() =>
            {
                // Create scope for function body
                var blockScope = new Scoping();

                // Chain together with outer scope
                blockScope.SetLeftScope(outerScope);

                // Execute the block
                EnterBlock(providerStatement.Body, blockScope);

                // Evaluate return expression
                return EnterExpression(providerStatement.Return, blockScope);
            });
        }

        public dynamic EnterConsumerStatement(ConsumerStatementModel consumerStatement, Scoping outerScope)
        {
            // Create late evaluation
            return new Action<IList<dynamic>>(args =>
            {
                var parameters = consumerStatement.Parameters.Parameters;

                // Create local scope for function body
                var localScope = new Scoping();

                // Chain together with outer scope
                localScope.SetLeftScope(outerScope);

                // Put argument values in this local scope
                for (int i = 0; i < parameters.Count; i++)
                {
                    string argIdentifier = parameters.ElementAt(i);
                    dynamic argValue = args[i];

                    localScope.AddLocalVariable(argIdentifier, argValue);
                }

                // Evaluate function body
                EnterBlock(consumerStatement.Body, localScope);
            });
        }

        public dynamic EnterNativeConsumerStatement(NativeConsumerStatementModel consumerStatement, Scoping outerScope)
        {
            return new Action<IList<dynamic>>(args =>
            {
                var parameters = consumerStatement.Parameters.Parameters;
                var localScope = new Scoping();
                localScope.SetLeftScope(outerScope);

                // Put argument values in this local scope
                for (int i = 0; i < parameters.Count; i++)
                {
                    string argIdentifier = parameters.ElementAt(i);
                    dynamic argValue = args[i];

                    localScope.AddLocalVariable(argIdentifier, argValue);
                }

                consumerStatement.NativeImplementation(args);
            });
        }

        public dynamic EnterNativeProviderStatement(NativeProviderStatementModel providerStatement, Scoping outerScope)
        {
            return new Func<dynamic>(() =>
            {
                var localScope = new Scoping();
                localScope.SetLeftScope(outerScope);

                return providerStatement.NativeImplementation();
            });
        }

        public dynamic EnterNativeFunctionStatement(NativeFunctionStatementModel functionStatement, Scoping outerScope)
        {
            return new Func<IList<dynamic>, dynamic>(args =>
            {
                var parameters = functionStatement.Parameters.Parameters;
                var localScope = new Scoping();
                localScope.SetLeftScope(outerScope);

                // Put argument values in this local scope
                for (int i = 0; i < parameters.Count; i++)
                {
                    string argIdentifier = parameters.ElementAt(i);
                    dynamic argValue = args[i];

                    localScope.AddLocalVariable(argIdentifier, argValue);
                }

                return functionStatement.NativeImplementation(args);
            });
        }

        public dynamic EnterNativeActionStatement(NativeActionStatementModel actionStatement, Scoping outerScope)
        {
            return new Action(() => actionStatement.NativeImplementation());
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
            loopScope.SetLeftScope(outerScope);

            while (EnterExpression(loop.Condition, loopScope))
            {
                EnterBlock(loop.Body, loopScope);
            }
        }

        public void EnterConditionalStatement(ConditionalStatementModel conditionalStatement, Scoping outerScope)
        {
            var conditionalScope = new Scoping();
            conditionalScope.SetLeftScope(outerScope);

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

        private void UpdateBinding(string identifier, AssignmentOperator operatorCombination, dynamic value, Scoping scope)
        {
            switch (operatorCombination)
            {
                case AssignmentOperator.Assign:
                    scope.SetBacktrackedVariable(identifier, value);
                    break;
                case AssignmentOperator.AddAssign:
                    scope.SetBacktrackedVariable(identifier, scope.GetBacktrackedVariable(identifier) + value);
                    break;
                case AssignmentOperator.SubAssign:
                    scope.SetBacktrackedVariable(identifier, scope.GetBacktrackedVariable(identifier) - value);
                    break;
                case AssignmentOperator.MultAssign:
                    scope.SetBacktrackedVariable(identifier, scope.GetBacktrackedVariable(identifier) * value);
                    break;
                case AssignmentOperator.DivAssign:
                    scope.SetBacktrackedVariable(identifier, scope.GetBacktrackedVariable(identifier) / value);
                    break;
                case AssignmentOperator.ModAssign:
                    scope.SetBacktrackedVariable(identifier, scope.GetBacktrackedVariable(identifier) % value);
                    break;
                case AssignmentOperator.AndAssign:
                    scope.SetBacktrackedVariable(identifier, scope.GetBacktrackedVariable(identifier) && value);
                    break;
                case AssignmentOperator.XorAssign:
                    scope.SetBacktrackedVariable(identifier, scope.GetBacktrackedVariable(identifier) ^ value);
                    break;
                case AssignmentOperator.OrAssign:
                    scope.SetBacktrackedVariable(identifier, scope.GetBacktrackedVariable(identifier) || value);
                    break;
                case AssignmentOperator.BitwiseAndAssign:
                    scope.SetBacktrackedVariable(identifier, scope.GetBacktrackedVariable(identifier) & value);
                    break;
                case AssignmentOperator.BitwiseXorAssign:
                    scope.SetBacktrackedVariable(identifier, scope.GetBacktrackedVariable(identifier) ^ value);
                    break;
                case AssignmentOperator.BitwiseOrAssign:
                    scope.SetBacktrackedVariable(identifier, scope.GetBacktrackedVariable(identifier) | value);
                    break;
                case AssignmentOperator.ShiftLeftAssign:
                    scope.SetBacktrackedVariable(identifier, scope.GetBacktrackedVariable(identifier) << value);
                    break;
                case AssignmentOperator.ShiftRightAssign:
                    scope.SetBacktrackedVariable(identifier, scope.GetBacktrackedVariable(identifier) >> value);
                    break;
                default:
                    throw new NotImplementedException("Operator combination not implemented.");
            }
        }

        private void UpdateImportedBinding(string identifier, AssignmentOperator operatorCombination, dynamic value, Namespace ns)
        {
            IDictionary<string, dynamic> bindings = ns.GetImportedBindings();
            switch (operatorCombination)
            {
                case AssignmentOperator.Assign:
                    bindings[identifier] = value;
                    break;
                case AssignmentOperator.AddAssign:
                    bindings[identifier] += value;
                    break;
                case AssignmentOperator.SubAssign:
                    bindings[identifier] -= value;
                    break;
                case AssignmentOperator.MultAssign:
                    bindings[identifier] *= value;
                    break;
                case AssignmentOperator.DivAssign:
                    bindings[identifier] /= value;
                    break;
                case AssignmentOperator.ModAssign:
                    bindings[identifier] %= value;
                    break;
                case AssignmentOperator.AndAssign:
                    bindings[identifier] &= value;
                    break;
                case AssignmentOperator.XorAssign:
                    bindings[identifier] ^= value;
                    break;
                case AssignmentOperator.OrAssign:
                    bindings[identifier] |= value;
                    break;
                case AssignmentOperator.BitwiseAndAssign:
                    bindings[identifier] &= value;
                    break;
                case AssignmentOperator.BitwiseXorAssign:
                    bindings[identifier] ^= value;
                    break;
                case AssignmentOperator.BitwiseOrAssign:
                    bindings[identifier] |= value;
                    break;
                case AssignmentOperator.ShiftLeftAssign:
                    bindings[identifier] <<= value;
                    break;
                case AssignmentOperator.ShiftRightAssign:
                    bindings[identifier] >>= value;
                    break;
                default:
                    throw new NotImplementedException("Operator combination not implemented.");
            }
        }

        private bool TryUpdateOrSetRuntimeObjectMember(RuntimeObject obj, string identifier, AssignmentOperator operatorCombination, dynamic value)
        {
            if (!obj.TryGetMember(new RuntimeObject.GetterBinder(identifier), out object o) && operatorCombination == AssignmentOperator.Assign)
                return obj.TrySetMember(new RuntimeObject.SetterBinder(identifier), value);

            var binder = new RuntimeObject.SetterBinder(identifier);

            return operatorCombination switch
            {
                AssignmentOperator.Assign => obj.TrySetMember(binder, value),
                AssignmentOperator.AddAssign => obj.TrySetMember(binder, (dynamic)o + value),
                AssignmentOperator.SubAssign => obj.TrySetMember(binder, (dynamic)o - value),
                AssignmentOperator.MultAssign => obj.TrySetMember(binder, (dynamic)o * value),
                AssignmentOperator.DivAssign => obj.TrySetMember(binder, (dynamic)o / value),
                AssignmentOperator.ModAssign => obj.TrySetMember(binder, (dynamic)o % value),
                AssignmentOperator.AndAssign => obj.TrySetMember(binder, (dynamic)o && value),
                AssignmentOperator.XorAssign => obj.TrySetMember(binder, (dynamic)o ^ value),
                AssignmentOperator.OrAssign => obj.TrySetMember(binder, (dynamic)o || value),
                AssignmentOperator.BitwiseAndAssign => obj.TrySetMember(binder, (dynamic)o & value),
                AssignmentOperator.BitwiseXorAssign => obj.TrySetMember(binder, (dynamic)o ^ value),
                AssignmentOperator.BitwiseOrAssign => obj.TrySetMember(binder, (dynamic)o | value),
                AssignmentOperator.ShiftLeftAssign => obj.TrySetMember(binder, (dynamic)o << value),
                AssignmentOperator.ShiftRightAssign => obj.TrySetMember(binder, (dynamic)o >> value),
                _ => throw new NotImplementedException($"{nameof(TryUpdateOrSetRuntimeObjectMember)}: That operator combination is not implemented."),
            };
        }

        public void EnterAssignStatement(AssignStatementModel assignStatement, Scoping scope)
        {
            IExpressionModel expression = assignStatement.Assignee;
            AssignmentOperator operatorCombination = assignStatement.OperatorCombination;
            dynamic result = EnterExpression(expression, scope);

            // Standalone identifier
            if (assignStatement.Identifier.Length is 1)
            {
                string identifier = assignStatement.Identifier[0];

                if (scope.ContainsBacktrack(identifier))
                {
                    UpdateBinding(identifier, operatorCombination, result, scope);
                }
                else if (_namespace.GetImportedBindings().ContainsKey(identifier))
                {
                    if (operatorCombination == AssignmentOperator.Assign)
                    {
                        _namespace.GetImportedBindings().Add(identifier, result);
                    }
                    else
                    {
                        UpdateImportedBinding(identifier, operatorCombination, result, _namespace);
                    }
                }
                else
                {
                    if (operatorCombination == AssignmentOperator.Assign)
                    {
                        scope.AddLocalVariable(identifier, result);
                    }
                    else throw new KeyNotFoundException($"Cannot use operator '{operatorCombination}' with the undefined variable '{identifier}'.".Replace('\'', '"'));
                }

                return;
            }

            RuntimeObject obj =
                    scope.ContainsBacktrack(assignStatement.Identifier[0]) ?
                        scope.GetBacktrackedVariable(assignStatement.Identifier[0]) :
                    _namespace.GetImportedBindings().ContainsKey(assignStatement.Identifier[0]) ?
                        _namespace.GetImportedBinding(assignStatement.Identifier[0]) :
                    null;

            if (obj != null)
            {
                for (int i = 1; i < assignStatement.Identifier.Length - 1; i++)
                {
                    if (obj.TryGetMember(new RuntimeObject.GetterBinder(assignStatement.Identifier[i]), out object prop))
                        obj = (RuntimeObject)prop;
                    else
                        throw new KeyNotFoundException($"Could not find property '{assignStatement.Identifier[i]}' in '{assignStatement.Identifier[i - 1]}'");
                }

                //if (obj.TrySetMember(new RuntimeObject.SetterBinder(assignStatement.Identifier.Last()), result))
                if (TryUpdateOrSetRuntimeObjectMember(obj, assignStatement.Identifier.Last(), operatorCombination, result))
                    return;
                else
                    throw new KeyNotFoundException($"Could not find property '{assignStatement.Identifier.Last()}' in '{assignStatement.Identifier[assignStatement.Identifier.Length - 2]}'");

            }

            throw new KeyNotFoundException("Could not find variable named " + assignStatement.Identifier[0]);
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
                throw new KeyNotFoundException($"Could not find namespace '{nsPath}' in current environment", e);
            }

        }

        public void EnterTryCatchStatement(TryCatchStatementModel statement, Scoping scope)
        {
            try
            {
                var tryBlockScope = new Scoping();
                tryBlockScope.SetLeftScope(scope);
                EnterBlock(statement.Try.Body, tryBlockScope);
            }
            catch (LanguageException le) // Catches exceptions thrown by language interpreter
            {
                var catchBlockScope = new Scoping();
                catchBlockScope.SetLeftScope(scope);
                catchBlockScope.AddLocalVariable(statement.Catch.ArgumentName, le.Argument);
                EnterBlock(statement.Catch.Body, catchBlockScope);
            }
            catch (Exception e) // Catches exceptions thrown by .NET
            {
                var catchBlockScope = new Scoping();
                catchBlockScope.SetLeftScope(scope);

                // Map the .NET exception to an object readable by the interpreter
                var dotnetExceptionRuntimeObject = new RuntimeObject();
                dotnetExceptionRuntimeObject.TrySetMember(new RuntimeObject.SetterBinder("message"), $"Core exception: {e.Message}");
                dotnetExceptionRuntimeObject.TrySetMember(new RuntimeObject.SetterBinder("messageFull"), $"Core exception: {e}");

                catchBlockScope.AddLocalVariable(statement.Catch.ArgumentName, dotnetExceptionRuntimeObject);

                EnterBlock(statement.Catch.Body, catchBlockScope);
            }
        }

        public void EnterThrowStatement(ThrowStatement statement, Scoping scope) => throw new LanguageException(EnterExpression(statement.Expression, scope));
    }
}
