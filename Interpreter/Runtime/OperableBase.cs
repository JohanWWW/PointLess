using Interpreter.Models.Enums;
using Singulink.Numerics;
using System;
using System.Numerics;

namespace Interpreter.Runtime
{
    public abstract class OperableBase : IBinaryOperable, IUnaryOperable
    {
        public ObjectType OperableType { get; }

        public object Value { get; set; }

        protected OperableBase(object value, ObjectType type)
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

        protected Exception NotImplementedConvertible(Type targetType)
        {
            return new InvalidCastException($"Cannot convert value of type '{GetType().Name}' to '{targetType.Name}' due to missing implementation");
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

        public abstract TypeCode GetTypeCode();
        public virtual object ToType(Type conversionType, IFormatProvider provider) => throw NotImplementedConvertible(conversionType);
        public virtual bool ToBoolean(IFormatProvider provider)                     => throw NotImplementedConvertible(typeof(bool));
        public virtual BigDecimal ToBigDecimal(IFormatProvider provider)            => throw NotImplementedConvertible(typeof(BigDecimal));
        public virtual BigInteger ToBigInteger(IFormatProvider provider)            => throw NotImplementedConvertible(typeof(BigInteger));
        public virtual byte ToByte(IFormatProvider provider)                        => throw NotImplementedConvertible(typeof(byte));
        public virtual char ToChar(IFormatProvider provider)                        => throw NotImplementedConvertible(typeof(char));
        public virtual DateTime ToDateTime(IFormatProvider provider)                => throw NotImplementedConvertible(typeof(DateTime));
        public virtual decimal ToDecimal(IFormatProvider provider)                  => throw NotImplementedConvertible(typeof(decimal));
        public virtual double ToDouble(IFormatProvider provider)                    => throw NotImplementedConvertible(typeof(double));
        public virtual short ToInt16(IFormatProvider provider)                      => throw NotImplementedConvertible(typeof(short));
        public virtual int ToInt32(IFormatProvider provider)                        => throw NotImplementedConvertible(typeof(int));
        public virtual long ToInt64(IFormatProvider provider)                       => throw NotImplementedConvertible(typeof(long));
        public virtual sbyte ToSByte(IFormatProvider provider)                      => throw NotImplementedConvertible(typeof(sbyte));
        public virtual float ToSingle(IFormatProvider provider)                     => throw NotImplementedConvertible(typeof(float));
        public virtual ushort ToUInt16(IFormatProvider provider)                    => throw NotImplementedConvertible(typeof(ushort));
        public virtual uint ToUInt32(IFormatProvider provider)                      => throw NotImplementedConvertible(typeof(uint));
        public virtual ulong ToUInt64(IFormatProvider provider)                     => throw NotImplementedConvertible(typeof(ulong));
        public virtual string ToString(IFormatProvider provider)                    => throw NotImplementedConvertible(typeof(string));
    }

    public abstract class OperableBase<T> : OperableBase, IBinaryOperable<T>, IUnaryOperable<T>
    {
        public new T Value
        {
            get => (T)(this as OperableBase).Value;
            set => (this as OperableBase).Value = value;
        }

        protected OperableBase(T value, ObjectType type) : base(value, type)
        {
        }
    }
}
