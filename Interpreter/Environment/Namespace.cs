using Interpreter.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Environment
{
    // TODO: Implement TryGet
    public class Namespace
    {
        private IDictionary<string, IBinaryOperable> _importedBindings;

        public string Name { get; private set; }
        public Scoping Scope { get; private set; }

        public Namespace(string name)
        {
            Name = name;
            Scope = new Scoping();
            _importedBindings = new Dictionary<string, IBinaryOperable>();
        }

        public Namespace(string name, Namespace importNs)
        {
            Name = name;
            Scope = new Scoping();
            _importedBindings = new Dictionary<string, IBinaryOperable>();
            foreach (var kvp in importNs.Scope.GetBindings())
            {
                _importedBindings.Add(kvp);
            }
        }

        public void Import(Namespace ns)
        {
            foreach (var kvp in ns.Scope.GetBindings())
            {
                _importedBindings.Add(kvp);
            }
        }

        public IDictionary<string, IBinaryOperable> GetImportedBindings() => _importedBindings;

        public IBinaryOperable GetImportedValue(string identifier) => _importedBindings[identifier];

        public void AddOrUpdateBinding(string identifier, IBinaryOperable value) => _importedBindings[identifier] = value;

        public override string ToString() => $"{nameof(Namespace)}(\"{Name}\")";
    }
}
