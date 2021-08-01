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
    public class RuntimeObject : DynamicObject, IBinaryOperable<RuntimeObject> // TODO: Implement binary operable
    {
        private IDictionary<string, object> _properties = new Dictionary<string, object>();

        ObjectType IBinaryOperable.OperableType => ObjectType.Object;

        public RuntimeObject Value
        {
            get => (RuntimeObject)(this as IBinaryOperable).Value;
            set => (this as IBinaryOperable).Value = value;
        }

        object IBinaryOperable.Value { get; set; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string identifier = binder.Name;
            return _properties.TryGetValue(identifier, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string identifier = binder.Name;
            _properties[identifier] = value;
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

            MethodData toStringData = (MethodData)m;
            Method toString = toStringData.GetOverload(0);
            return toString.GetProvider().Invoke();
        }

        private string GetTree(RuntimeObject obj)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("obj{");

            var memberNames = obj.GetDynamicMemberNames().ToArray();
            for (int i = 0; i < memberNames.Length; i++)
            {
                string memberName = memberNames[i];
                object memberValue = obj._properties[memberName];

                if (memberValue is RuntimeObject)
                {
                    string tree = GetTree(memberValue as RuntimeObject);
                    stringBuilder.Append(memberName).Append('=').Append(tree);
                }
                else if (memberValue is Method m)
                {
                    stringBuilder.Append(memberName).Append('=');
                    switch (m.MethodType)
                    {
                        case MethodType.Function:
                            stringBuilder.Append($"([{m.ParameterCount}])=>*");
                            break;
                        case MethodType.Action:
                            stringBuilder.Append("()=>{}");
                            break;
                        case MethodType.Consumer:
                            stringBuilder.Append($"([{m.ParameterCount}])=>{{}}");
                            break;
                        case MethodType.Provider:
                            stringBuilder.Append("()=>*");
                            break;
                    }
                }
                else if (memberValue is MethodData md)
                {
                    if (md.OverloadCount is 1)
                    {
                        Method mtd = md.GetSingle();
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
                }
                else if (memberValue is string s)
                {
                    stringBuilder.Append(memberName).Append('=').Append('"').Append(s.Replace("\\", "\\\\").Replace("\"", "\\\"")).Append('"');
                }
                else
                {
                    stringBuilder.Append(memberName).Append('=').Append('\'').Append(memberValue).Append('\'');
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

        public IBinaryOperable Add(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_add__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.Add);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable Subtract(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_sub__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.Sub);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable Multiply(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_mult__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.Mult);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable Divide(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_divide__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.Div);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable Mod(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_mod__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.Mod);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable Equal(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_equals__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.Equal);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable NotEqual(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_not_equals__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.NotEqual);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable LessThan(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_less_than__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.LessThan);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable LessThanOrEqual(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_less_than_or_equals__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.LessThanOrEqual);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable GreaterThan(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_greater_than__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.GreaterThan);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable GreaterThanOrEqual(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_greater_than_or_equals__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.GreaterThanOrEqual);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable LogicalAnd(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_logical_and__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.LogicalAnd);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable LogicalOr(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_logical_or__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.LogicalOr);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable LogicalXOr(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_logical_xor__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.LogicalXOr);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable BitwiseAnd(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_bitwise_and__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.BitwiseAnd);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable BitwiseOr(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_bitwise_or__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.BitwiseOr);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable BitwiseXOr(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_bitwise_xor__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.BitwiseXOr);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable ShiftLeft(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_shift_left__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.ShiftLeft);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        public IBinaryOperable ShiftRight(IBinaryOperable operand)
        {
            if (!TryGetMember(new GetterBinder("__operator_shift_right__"), out object o))
                ThrowOperatorOverloadNotFound(BinaryOperator.ShiftRight);

            return InvokeBinaryOperator(o as MethodData, this, operand);
        }

        [Obsolete]
        public static dynamic operator +(RuntimeObject a, object b)
        {
            if (!a.TryGetMember(new GetterBinder("__operator_add__"), out object o))
                throw new MethodOverloadException($"No overload exists for operator {nameof(BinaryOperator.Add)}");

            var opAddData = (MethodData)o;
            Method opAdd = opAddData.GetOverload(2);
            return opAdd.GetFunction().Invoke(new dynamic[] { a, b });
        }

        [Obsolete]
        public static dynamic operator -(RuntimeObject a, object b)
        {
            if (!a.TryGetMember(new GetterBinder("__operator_sub__"), out object o))
                throw new MethodOverloadException($"No overload exists for operator {nameof(BinaryOperator.Sub)}");

            var opSubData = (MethodData)o;
            Method opSub = opSubData.GetOverload(2);
            return opSub.GetFunction().Invoke(new dynamic[] { a, b });
        }

        [Obsolete]
        public static dynamic operator *(RuntimeObject a, object b)
        {
            if (!a.TryGetMember(new GetterBinder("__operator_mult__"), out object o))
                throw new MethodOverloadException($"No overload exists for operator {nameof(BinaryOperator.Mult)}");

            var opMultData = (MethodData)o;
            Method opMult = opMultData.GetOverload(2);
            return opMult.GetFunction().Invoke(new dynamic[] { a, b });
        }

        [Obsolete]
        public static dynamic operator /(RuntimeObject a, object b)
        {
            if (!a.TryGetMember(new GetterBinder("__operator_div__"), out object o))
                throw new MethodOverloadException($"No overload exists for operator {nameof(BinaryOperator.Div)}");

            var opDivData = (MethodData)o;
            Method opDiv = opDivData.GetOverload(2);
            return opDiv.GetFunction().Invoke(new dynamic[] { a, b });
        }

        [Obsolete]
        public static dynamic operator %(RuntimeObject a, object b)
        {
            if (!a.TryGetMember(new GetterBinder("__operator_mod__"), out object o))
                throw new MethodOverloadException($"No overload exists for operator {nameof(BinaryOperator.Mod)}");

            var opModData = (MethodData)o;
            Method opMod = opModData.GetOverload(2);
            return opMod.GetFunction().Invoke(new dynamic[] { a, b });
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
