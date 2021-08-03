using Interpreter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public abstract class WrapperBase : IBinaryOperable
    {
        public ObjectType OperableType { get; }

        public object Value { get; set; }

        protected WrapperBase(object value, ObjectType type)
        {
            Value = value;
            OperableType = type;
        }

        protected Exception MissingBinaryOperatorImplementation(IBinaryOperable operand, BinaryOperator op)
        {
            return new MissingBinaryOperatorOverrideException($"Missing operator implementation for operator '{op}' on types '{OperableType}' and '{operand.OperableType}'");
        }

        #region Optional overrides

        public virtual IBinaryOperable Add(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Add);
        public virtual IBinaryOperable BitwiseAnd(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.BitwiseAnd);
        public virtual IBinaryOperable BitwiseOr(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.BitwiseOr);
        public virtual IBinaryOperable BitwiseXOr(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.BitwiseXOr);
        public virtual IBinaryOperable Divide(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Div);
        public virtual IBinaryOperable Equal(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Equal);
        public virtual IBinaryOperable GreaterThan(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.GreaterThan);
        public virtual IBinaryOperable GreaterThanOrEqual(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.GreaterThanOrEqual);
        public virtual IBinaryOperable LessThan(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LessThan);
        public virtual IBinaryOperable LessThanOrEqual(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LessThanOrEqual);
        public virtual IBinaryOperable LogicalAnd(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LogicalAnd);
        public virtual IBinaryOperable LogicalOr(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LogicalOr);
        public virtual IBinaryOperable LogicalXOr(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LogicalXOr);
        public virtual IBinaryOperable Mod(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Mod);
        public virtual IBinaryOperable Multiply(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Mult);
        public virtual IBinaryOperable NotEqual(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.NotEqual);
        public virtual IBinaryOperable ShiftLeft(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.ShiftLeft);
        public virtual IBinaryOperable ShiftRight(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.ShiftRight);
        public virtual IBinaryOperable Subtract(IBinaryOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Sub);

        #endregion

        #region Mandatory overrides

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract IBinaryOperable<bool> StrictEqual(IBinaryOperable operand);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract IBinaryOperable<bool> StrictNotEqual(IBinaryOperable operand);

        #endregion

        public override string ToString() => Value.ToString();

        public bool Equals(IBinaryOperable operable) => Value.Equals(operable.Value);

        public override bool Equals(object obj) => Equals((IBinaryOperable)obj);

        public override int GetHashCode() => HashCode.Combine(OperableType, Value);
    }

    public abstract class WrapperBase<T> : WrapperBase, IBinaryOperable<T>
    {
        public new T Value
        {
            get => (T)(this as WrapperBase).Value;
            set => (this as WrapperBase).Value = value;
        }

        protected WrapperBase(T value, ObjectType type) : base(value, type)
        {
        }
    }
}
