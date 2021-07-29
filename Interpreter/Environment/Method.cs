using Interpreter.Models.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Environment
{
    public class Method
    {
        private readonly object _method;

        public int ParameterCount { get; }
        public MethodType MethodType { get; }

        public Method(int parameterCount, object method, MethodType type)
        {
            ParameterCount = parameterCount;
            MethodType = type;
            _method = method;
        }

        public FunctionMethod GetFunction()
        {
            return (FunctionMethod)_method;
        }

        public ActionMethod GetAction()
        {
            return (ActionMethod)_method;
        }

        public ConsumerMethod GetConsumer()
        {
            return (ConsumerMethod)_method;
        }

        public ProviderMethod GetProvider()
        {
            return (ProviderMethod)_method;
        }
    }
}
