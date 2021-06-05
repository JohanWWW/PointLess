using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.NativeImplementations
{
    public class NativeImplementation
    {
        internal IReadOnlyDictionary<string, IFunctionModel> GetImplementationMap()
        {
            Type type = GetType();
            FieldInfo[] fields = type.GetFields();

            var map = new Dictionary<string, IFunctionModel>();

            foreach (FieldInfo field in fields)
            {
                var implementationAttr = field.GetCustomAttribute<ImplementationIdentifierAttribute>();

                if (implementationAttr is null)
                    continue;

                string identifier = implementationAttr.Identifier;
                IFunctionModel function = field.GetValue(this) as IFunctionModel;

                map[identifier] = function;
            }

            return map;
        }
    }
}
