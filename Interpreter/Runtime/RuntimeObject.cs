using Interpreter.Environment;
using Interpreter.Environment.Exceptions;
using Interpreter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    /// <summary>
    /// Represents the object that is created during program runtime
    /// </summary>
    public class RuntimeObject : DynamicObject, IBinaryOperable<RuntimeObject> // TODO: Make separate wrapper class for RuntimeObject instead
    {
        private readonly IDictionary<string, IBinaryOperable> _properties = new Dictionary<string, IBinaryOperable>();

        ObjectType IBinaryOperable.OperableType => ObjectType.Object;

        public RuntimeObject Value => this;

        object IBinaryOperable.Value => Value;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string identifier = binder.Name;
            if (!_properties.TryGetValue(identifier, out IBinaryOperable o))
            {
                result = null;
                return false;
            }
            result = o;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string identifier = binder.Name;
            _properties[identifier] = (IBinaryOperable)value;
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (var member in _properties.Keys)
            {
                yield return member;
            }
        }

        public override string ToString()
        {
            if (!TryGetMember(new GetterBinder("toString"), out object m))
                return GetTree(this);

            MethodData toStringData = (m as IBinaryOperable<MethodData>).Value;
            Method toString = toStringData.GetOverload(0);
            return toString.GetProvider().Invoke().Value.ToString();
        }

        private string GetTree(RuntimeObject obj)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("obj{");

            var memberNames = obj.GetDynamicMemberNames().ToArray();
            for (int i = 0; i < memberNames.Length; i++)
            {
                string memberName = memberNames[i];
                IBinaryOperable memberValue = obj._properties[memberName];

                if (memberValue is IBinaryOperable<RuntimeObject>)
                {
                    string tree = memberValue.ToString();
                    stringBuilder.Append(memberName).Append('=').Append(tree);
                }
                else if (memberValue is IBinaryOperable<Method> methodOperable)
                {
                    stringBuilder.Append(memberName).Append('=');
                    switch (methodOperable.Value.MethodType)
                    {
                        case MethodType.Function:
                            stringBuilder.Append($"([{methodOperable.Value.ParameterCount}])=>*");
                            break;
                        case MethodType.Action:
                            stringBuilder.Append("()=>{}");
                            break;
                        case MethodType.Consumer:
                            stringBuilder.Append($"([{methodOperable.Value.ParameterCount}])=>{{}}");
                            break;
                        case MethodType.Provider:
                            stringBuilder.Append("()=>*");
                            break;
                    }
                }
                else if (memberValue is IBinaryOperable<MethodData> mdOperable)
                {
                    if (mdOperable.Value.OverloadCount is 1)
                    {
                        Method mtd = mdOperable.Value.GetSingle();
                        stringBuilder.Append(memberName).Append('=');
                        switch (mtd.MethodType)
                        {
                            case MethodType.Function:
                                stringBuilder.Append($"([{mtd.ParameterCount}])=>*");
                                break;
                            case MethodType.Action:
                                stringBuilder.Append("()=>{}");
                                break;
                            case MethodType.Consumer:
                                stringBuilder.Append($"([{mtd.ParameterCount}])=>{{}}");
                                break;
                            case MethodType.Provider:
                                stringBuilder.Append("()=>*");
                                break;
                        }
                    }
                    else
                    {
                        stringBuilder.Append(mdOperable.ToString());
                    }
                }
                else if (memberValue is IBinaryOperable<string> stringOperable)
                {
                    stringBuilder.Append(memberName).Append('=').Append('"').Append(stringOperable.Value.Replace("\\", "\\\\").Replace("\"", "\\\"")).Append('"');
                }
                else
                {
                    stringBuilder.Append(memberName).Append('=').Append(memberValue.ToString());
                }

                if (i < memberNames.Length - 1)
                    stringBuilder.Append(", ");
            }

            stringBuilder.Append('}');

            stringBuilder.Replace("\n", "\\n");
            stringBuilder.Replace("\t", "\\t");

            return stringBuilder.ToString();
        }

        private static IBinaryOperable InvokeBinaryOperator(MethodData op, IBinaryOperable left, IBinaryOperable right)
        {
            return op.GetOverload(2).GetFunction().Invoke(new IBinaryOperable[] { left, right });
        }

        private static void ThrowOperatorOverloadNotFound(BinaryOperator op)
        {
            throw new MethodOverloadException($"No overload exists for operator {op}");
        }

        private static void ThrowOperatorOverloadNotFound(BinaryOperator op, string additionalInfo)
        {
            throw new MethodOverloadException($"No overload exists for operator {op}\n{additionalInfo}");
        }

        public IBinaryOperable Add(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_add__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.Add);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable Subtract(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_sub__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.Sub);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable Multiply(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_mult__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.Mult);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable Divide(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_div__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.Div);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable Mod(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_mod__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.Mod);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable Equal(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_equals__"), out object o))
                // Default behaviour
                return operand.OperableType switch
                {
                    ObjectType.Object => BooleanWrapper.FromBool(Value == operand.Value),
                    ObjectType.NullReference => BooleanWrapper.False,
                    _ => throw new MethodOverloadException($"Cannot apply operator '{BinaryOperator.Equal}' on types '{(this as IBinaryOperable).OperableType}' and '{operand.OperableType}'. Consider overriding this operator.")
                };

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable NotEqual(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_not_equals__"), out object o))
                // Default behaviour
                return operand.OperableType switch
                {
                    ObjectType.Object => BooleanWrapper.FromBool(Value != operand.Value),
                    ObjectType.NullReference => BooleanWrapper.True,
                    _ => throw new MethodOverloadException($"Cannot apply operator '{BinaryOperator.NotEqual}' on types '{(this as IBinaryOperable).OperableType}' and '{operand.OperableType}'. Consider overriding this operator.")
                };

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable LessThan(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_less_than__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.LessThan);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable LessThanOrEqual(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_less_than_or_equals__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.LessThanOrEqual);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable GreaterThan(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_greater_than__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.GreaterThan);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable GreaterThanOrEqual(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_greater_than_or_equals__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.GreaterThanOrEqual);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable LogicalAnd(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_logical_and__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.LogicalAnd);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable LogicalOr(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_logical_or__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.LogicalOr);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable LogicalXOr(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_logical_xor__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.LogicalXOr);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable BitwiseAnd(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_bitwise_and__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.BitwiseAnd);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable BitwiseOr(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_bitwise_or__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.BitwiseOr);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable BitwiseXOr(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_bitwise_xor__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.BitwiseXOr);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable ShiftLeft(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_shift_left__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.ShiftLeft);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable ShiftRight(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_shift_right__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.ShiftRight);

            return InvokeBinaryOperator((o as IBinaryOperable<MethodData>).Value, this, operand);
        }

        public IBinaryOperable<bool> StrictEqual(IBinaryOperable operand)
        {
            if ((this as IBinaryOperable).OperableType != operand.OperableType)
                return BooleanWrapper.False;

            return BooleanWrapper.FromBool(this == operand.Value);
        }

        public IBinaryOperable<bool> StrictNotEqual(IBinaryOperable operand)
        {
            if ((this as IBinaryOperable).OperableType != operand.OperableType)
                return BooleanWrapper.True;

            return BooleanWrapper.FromBool(this != operand.Value);
        }

        public class SetterBinder : SetMemberBinder
        {
            public SetterBinder(string identifier) : base(identifier, false)
            {
            }

            public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion) =>
                throw new NotImplementedException();
        }

        public class GetterBinder : GetMemberBinder
        {
            public GetterBinder(string identifier) : base(identifier, false)
            {
            }

            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion) =>
                throw new NotImplementedException();
        }
    }
}
