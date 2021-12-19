using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClientType = Interpreter.Runtime.ObjectType;

namespace Interpreter.Framework.Extern
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ExternParameterAttribute : Attribute
    {
        internal ClientType[] FromType { get; }
        internal bool ImplicitResolve { get; }

        public ExternParameterAttribute(bool implicitResolve = true)
        {
            ImplicitResolve = implicitResolve;
        }

        public ExternParameterAttribute(params ClientType[] fromType) : this(implicitResolve: false)
        {
            FromType = fromType;
        }
    }
}
