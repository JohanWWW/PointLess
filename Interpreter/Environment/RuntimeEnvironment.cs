using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Environment
{
    public class RuntimeEnvironment
    {
        private readonly IDictionary<string, Namespace> _namespaces;

        public RuntimeEnvironment()
        {
            _namespaces = new Dictionary<string, Namespace>();
        }

        public void AddNamespace(Namespace ns)
        {
            _namespaces[ns.Name] = ns;
        }

        public Namespace GetNamespace(string name) => _namespaces[name];

        public bool TryGetNamespace(string name, out Namespace value) =>
            _namespaces.TryGetValue(name, out value);
    }
}
