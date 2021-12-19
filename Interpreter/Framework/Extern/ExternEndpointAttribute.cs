using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClientType = Interpreter.Runtime.ObjectType;

namespace Interpreter.Framework.Extern
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ExternEndpointAttribute : Attribute
    {
        public string Name { get; set; }
        public ClientType? ReturnType { get; set; }
    }
}
