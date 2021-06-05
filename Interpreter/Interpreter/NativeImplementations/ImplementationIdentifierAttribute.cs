using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.NativeImplementations
{
    public class ImplementationIdentifierAttribute : Attribute
    {
        public string Identifier { get; private set; }

        public ImplementationIdentifierAttribute(string identifier)
        {
            Identifier = identifier;
        }
    }
}
