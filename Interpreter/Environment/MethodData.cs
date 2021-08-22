using Interpreter.Environment.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Environment
{
    public class MethodData
    {
        private readonly IDictionary<int, Method> _overloads;

        public int OverloadCount => _overloads.Count;

        public MethodData(Method initialMethod)
        {
            _overloads = new Dictionary<int, Method>();
            _overloads.Add(initialMethod.ParameterCount, initialMethod);
        }

        private MethodData(MethodData src)
        {
            _overloads = new Dictionary<int, Method>(src._overloads);
        }

        public void AddOverload(Method method)
        {
            if (!_overloads.TryAdd(method.ParameterCount, method))
                throw new MethodOverloadException($"An overload with parameter count {method.ParameterCount} already exists for this method");
        }

        public void RemoveOverload(int parameterCount)
        {
            if (!_overloads.Remove(parameterCount))
                throw new MethodOverloadException($"No overload with parameter count {parameterCount} exist for this method");
        }

        public Method GetOverload(int parameterCount)
        {
            return _overloads[parameterCount];
        }

        public bool TryGetOverload(int parameterCount, out Method value) =>
            _overloads.TryGetValue(parameterCount, out value);

        public Method GetSingle() => _overloads.Single().Value;

        public bool TryGetSingle(out Method value)
        {
            if (OverloadCount != 1)
            {
                value = null;
                return false;
            }

            value = GetSingle();
            return true;
        }

        public bool ContainsOverload(int parameterCount)
        {
            return _overloads.ContainsKey(parameterCount);
        }

        public static MethodData Add(MethodData methodData, Method overload)
        {
            MethodData copiedMd = Copy(methodData);
            copiedMd.AddOverload(overload);
            return copiedMd;
        }

        public static MethodData Subtract(MethodData methodData, int parameterCount)
        {
            MethodData copiedMd = Copy(methodData);
            copiedMd.RemoveOverload(parameterCount);
            return copiedMd;
        }

        public static MethodData operator +(MethodData methodData, Method overload) => Add(methodData, overload);

        public static MethodData operator -(MethodData methodData, int parameterCount) => Subtract(methodData, parameterCount);

        public static MethodData operator -(MethodData methodData, BigInteger parameterCount) => Subtract(methodData, (int)parameterCount);

        private static MethodData Copy(MethodData md) => new MethodData(md);

        public override string ToString() => $"MethodData{{OverloadCount: {OverloadCount}}}";
    }
}
