using Singulink.Numerics;
using System;
using System.Numerics;

namespace Interpreter.Runtime
{
    public interface IValueConvertible : IConvertible
    {
        BigInteger ToBigInteger(IFormatProvider provider);
        BigDecimal ToBigDecimal(IFormatProvider provider);
    }
}
