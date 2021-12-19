using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Framework.Extern
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExternAPIAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
