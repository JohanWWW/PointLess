using Interpreter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public abstract class WrapperBase : IBinaryOperable, IUnaryOperable
    {
        public ObjectType OperableType { get; }

        public object Value { get; set; }

        protected WrapperBase(object value, ObjectType type)
        {
            Value = value;
            OperableType = type;
        }

        protected Exception MissingBinaryOperatorImplementation(IOperable operand, BinaryOperator op)
        {
            return new MissingOperatorOverrideException($"Missing operator implementation for operator '{op}' on types '{OperableType}' and '{operand.OperableType}'");
        }

        protected Exception MissingUnaryOperatorImplementation(UnaryOperator op)
        {
            return new MissingOperatorOverrideException($"Missing operator implementation for unary operator '{op}' on type '{OperableType}'");
        }

        #region Binary Operators

        public virtual IOperable Add(IOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Add);
        public virtual IOperable BitwiseAnd(Func<IOperable> operand) => throw MissingBinaryOperatorImplementation(operand(), BinaryOperator.BitwiseAnd);
        public virtual IOperable BitwiseOr(Func<IOperable> operand) => throw MissingBinaryOperatorImplementation(operand(), BinaryOperator.BitwiseOr);
        public virtual IOperable BitwiseXOr(IOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.BitwiseXOr);
        public virtual IOperable Divide(IOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Div);
        public virtual IOperable Equal(IOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Equal);
        public virtual IOperable GreaterThan(IOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.GreaterThan);
        public virtual IOperable GreaterThanOrEqual(IOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.GreaterThanOrEqual);
        public virtual IOperable LessThan(IOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LessThan);
        public virtual IOperable LessThanOrEqual(IOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LessThanOrEqual);
        public virtual IOperable LogicalAnd(Func<IOperable> operand) => throw MissingBinaryOperatorImplementation(operand(), BinaryOperator.LogicalAnd);
        public virtual IOperable LogicalOr(Func<IOperable> operand) => throw MissingBinaryOperatorImplementation(operand(), BinaryOperator.LogicalOr);
        public virtual IOperable LogicalXOr(IOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LogicalXOr);
        public virtual IOperable Mod(IOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Mod);
        public virtual IOperable Multiply(IOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Mult);
        public virtual IOperable NotEqual(IOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.NotEqual);
        public virtual IOperable ShiftLeft(IOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.ShiftLeft);
        public virtual IOperable ShiftRight(IOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.ShiftRight);
        public virtual IOperable Subtract(IOperable operand) => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Sub);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract IOperable<bool> StrictEqual(IOperable operand);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract IOperable<bool> StrictNotEqual(IOperable operand);

        #endregion

        #region Unary Operators

        public virtual IOperable UnaryMinus() => throw MissingUnaryOperatorImplementation(UnaryOperator.Minus);
        public virtual IOperable UnaryNot() => throw MissingUnaryOperatorImplementation(UnaryOperator.Not);

        #endregion

        public override string ToString() => Value.ToString();

        public bool Equals(IOperable operable) => Value.Equals(operable.Value);

        public override bool Equals(object obj) => Equals((IOperable)obj);

        public override int GetHashCode() => HashCode.Combine(OperableType, Value);

    }

    public abstract class WrapperBase<T> : WrapperBase, IBinaryOperable<T>, IUnaryOperable<T>
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
