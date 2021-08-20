using Interpreter.Environment;
using Interpreter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class ObjectOperable : OperableBase<RuntimeObject>
    {
        private const string TOSTRING_NAME = "toString";

        private static readonly IReadOnlyDictionary<BinaryOperator, string> BINARY_OPERATOR_FUNC_NAMES = new Dictionary<BinaryOperator, string>
        {
            [BinaryOperator.Add]                = "__operator_add__",
            [BinaryOperator.Sub]                = "__operator_sub__",
            [BinaryOperator.Mult]               = "__operator_mult__",
            [BinaryOperator.Div]                = "__operator_div__",
            [BinaryOperator.Mod]                = "__operator_mod__",
            [BinaryOperator.Equal]              = "__operator_equals__",
            [BinaryOperator.NotEqual]           = "__operator_not_equals__",
            [BinaryOperator.LessThan]           = "__operator_less_than__",
            [BinaryOperator.LessThanOrEqual]    = "__operator_less_than_or_equals__",
            [BinaryOperator.GreaterThan]        = "__operator_greater_than__",
            [BinaryOperator.GreaterThanOrEqual] = "__operator_greater_than_or_equals__",
            [BinaryOperator.LogicalAnd]         = "__operator_logical_and__",
            [BinaryOperator.LogicalOr]          = "__operator_logical_or__",
            [BinaryOperator.LogicalXOr]         = "__operator_logical_xor__",
            [BinaryOperator.BitwiseAnd]         = "__operator_bitwise_and__",
            [BinaryOperator.BitwiseOr]          = "__operator_bitwise_or__",
            [BinaryOperator.BitwiseXOr]         = "__operator_bitwise_xor__",
            [BinaryOperator.ShiftLeft]          = "__operator_shift_left__",
            [BinaryOperator.ShiftRight]         = "__operator_shift_right__"
        };

        private static readonly IReadOnlyDictionary<UnaryOperator, string> UNARY_OPERATOR_FUNC_NAMES = new Dictionary<UnaryOperator, string>
        {
            [UnaryOperator.Not]                 = "__operator_unary_not__",
            [UnaryOperator.Minus]               = "__operator_unary_minus__"
        };

        private ICollection<string> MemberNames => Value.MemberNames;

        public ObjectOperable() : base(new RuntimeObject(), ObjectType.Object)
        {
        }

        private ObjectOperable(RuntimeObject value) : base(value, ObjectType.Object)
        {
        }

        private IOperable this[string name]
        {
            get => Value[name];
            set => Value[name] = value;
        }

        private static IOperable InvokeBinaryOperator(MethodData op, IOperable left, IOperable right) =>
            op.GetOverload(2).GetFunction().Invoke(new IOperable[] { left, right });

        private static IOperable InvokeUnaryOperator(MethodData op, IOperable operable) =>
            op.GetOverload(1).GetFunction().Invoke(new IOperable[] { operable });

        public bool TryGetMember(string name, out IOperable value) =>
            Value.TryGetMember(name, out value);

        public bool TrySetMember(string name, IOperable value) => Value.TrySetMember(name, value);


        #region Binary Operators

        public override IOperable Add(IOperable operand)
        {
            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.Add], out IOperable value))
                throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Add);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, operand);
        }

        public override IOperable Subtract(IOperable operand)
        {
            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.Sub], out IOperable value))
                throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Sub);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, operand);
        }

        public override IOperable Multiply(IOperable operand)
        {
            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.Mult], out IOperable value))
                throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Mult);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, operand);
        }

        public override IOperable Divide(IOperable operand)
        {
            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.Div], out IOperable value))
                throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Div);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, operand);
        }

        public override IOperable Mod(IOperable operand)
        {
            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.Mod], out IOperable value))
                throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Mod);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, operand);
        }

        public override IOperable Equal(IOperable operand)
        {
            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.Equal], out IOperable value))
                return defaultEqual(operand);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, operand);
        
            IOperable defaultEqual(IOperable operand) => operand.OperableType switch
            {
                ObjectType.Object => BoolOperable.FromBool(Value == operand.Value),
                ObjectType.Void => BoolOperable.False,
                _ => throw new MissingOperatorOverrideException($"Cannot apply operator '{BinaryOperator.Equal}' on types '{OperableType}' and '{operand.OperableType}'. Consider overriding this operator.")
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.NotEqual], out IOperable value))
                return defaultNotEqual(operand);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, operand);
            
            IOperable defaultNotEqual(IOperable operand) => operand.OperableType switch
            {
                ObjectType.Object => BoolOperable.FromBool(Value != operand.Value),
                ObjectType.Void => BoolOperable.True,
                _ => throw new MissingOperatorOverrideException($"Cannot apply operator '{BinaryOperator.NotEqual}' on types '{OperableType}' and '{operand.OperableType}'. Consider overriding this operator.")
            };
        }

        public override IOperable LessThan(IOperable operand)
        {
            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.LessThan], out IOperable value))
                throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LessThan);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, operand);
        }

        public override IOperable LessThanOrEqual(IOperable operand)
        {
            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.LessThanOrEqual], out IOperable value))
                throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LessThanOrEqual);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, operand);
        }

        public override IOperable GreaterThan(IOperable operand)
        {
            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.GreaterThan], out IOperable value))
                throw MissingBinaryOperatorImplementation(operand, BinaryOperator.GreaterThan);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, operand);
        }

        public override IOperable GreaterThanOrEqual(IOperable operand)
        {
            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.GreaterThanOrEqual], out IOperable value))
                throw MissingBinaryOperatorImplementation(operand, BinaryOperator.GreaterThanOrEqual);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, operand);
        }

        public override IOperable LogicalAnd(Func<IOperable> operand)
        {
            IOperable eval = operand();

            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.LogicalAnd], out IOperable value))
                throw MissingBinaryOperatorImplementation(eval, BinaryOperator.LogicalAnd);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, eval);
        }

        public override IOperable LogicalOr(Func<IOperable> operand)
        {
            IOperable eval = operand();

            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.LogicalOr], out IOperable value))
                throw MissingBinaryOperatorImplementation(eval, BinaryOperator.LogicalOr);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, eval);
        }

        public override IOperable LogicalXOr(IOperable operand)
        {
            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.LogicalXOr], out IOperable value))
                throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LogicalXOr);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, operand);
        }

        public override IOperable BitwiseAnd(Func<IOperable> operand)
        {
            IOperable eval = operand();

            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.BitwiseAnd], out IOperable value))
                throw MissingBinaryOperatorImplementation(eval, BinaryOperator.BitwiseAnd);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, eval);
        }

        public override IOperable BitwiseOr(Func<IOperable> operand)
        {
            IOperable eval = operand();

            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.BitwiseOr], out IOperable value))
                throw MissingBinaryOperatorImplementation(eval, BinaryOperator.BitwiseOr);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, eval);
        }

        public override IOperable BitwiseXOr(IOperable operand)
        {
            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.BitwiseXOr], out IOperable value))
                throw MissingBinaryOperatorImplementation(operand, BinaryOperator.BitwiseXOr);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, operand);
        }

        public override IOperable ShiftLeft(IOperable operand)
        {
            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.ShiftLeft], out IOperable value))
                throw MissingBinaryOperatorImplementation(operand, BinaryOperator.ShiftLeft);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, operand);
        }

        public override IOperable ShiftRight(IOperable operand)
        {
            if (!TryGetMember(BINARY_OPERATOR_FUNC_NAMES[BinaryOperator.ShiftRight], out IOperable value))
                throw MissingBinaryOperatorImplementation(operand, BinaryOperator.ShiftRight);

            return InvokeBinaryOperator((value as IOperable<MethodData>).Value, this, operand);
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.False;

            return BoolOperable.FromBool(Value == operand.Value);
        }

        public override IOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.True;

            return BoolOperable.FromBool(Value != operand.Value);
        }

        #endregion

        #region Unary Operators

        public override IOperable UnaryMinus()
        {
            if (!TryGetMember(UNARY_OPERATOR_FUNC_NAMES[UnaryOperator.Minus], out IOperable value))
                throw MissingUnaryOperatorImplementation(UnaryOperator.Minus);

            return InvokeUnaryOperator((value as IOperable<MethodData>).Value, this);
        }

        public override IOperable UnaryNot()
        {
            if (!TryGetMember(UNARY_OPERATOR_FUNC_NAMES[UnaryOperator.Not], out IOperable value))
                throw MissingUnaryOperatorImplementation(UnaryOperator.Not);

            return InvokeUnaryOperator((value as IOperable<MethodData>).Value, this);
        }

        #endregion

        private static string ToString(ObjectOperable obj)
        {
            var stringBuilder = new StringBuilder();
            
            stringBuilder.Append("obj{");

            var memberNames = obj.MemberNames;
            int max = memberNames.Count;
            int i = 0;
            IEnumerator<string> enumerator = obj.MemberNames.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string memberName = enumerator.Current;
                IOperable memberValue = obj[memberName];

                stringBuilder.Append(memberName).Append('=');
                switch (memberValue.OperableType)
                {
                    case ObjectType.String:
                        string str = memberValue.ToString()
                            .Replace("\\", "\\\\")
                            .Replace("\"", "\\\"");
                        stringBuilder.Append('"').Append(str).Append('"');
                        break;
                    default:
                        stringBuilder.Append(memberValue.ToString());
                        break;
                }

                if (i < max - 1)
                    stringBuilder.Append(", ");
                i++;
            }

            stringBuilder.Append('}');

            stringBuilder.Replace("\n", "\\n");
            stringBuilder.Replace("\t", "\\t");

            return stringBuilder.ToString();
        }

        public static implicit operator ObjectOperable(RuntimeObject value) => new ObjectOperable(value);

        public override string ToString()
        {
            if (!TryGetMember(TOSTRING_NAME, out IOperable value))
                return ToString(this);

            MethodData toStringData = (value as IOperable<MethodData>).Value;
            Method toString = toStringData.GetOverload(0);
            return toString.GetProvider().Invoke().ToString();
        }
    }
}
