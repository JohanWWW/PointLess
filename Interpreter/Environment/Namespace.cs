using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Environment
{
    public class Namespace
    {
        private IDictionary<string, dynamic> _importedBindings;

        public string Name { get; private set; }
        public Scoping Scope { get; private set; }

        public Namespace(string name)
        {
            Name = name;
            Scope = new Scoping();
            _importedBindings = new Dictionary<string, dynamic>();
        }

        public Namespace(string name, Namespace importNs)
        {
            Name = name;
            Scope = new Scoping();
            _importedBindings = new Dictionary<string, dynamic>();
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

        public IDictionary<string, dynamic> GetImportedBindings() => _importedBindings;

        public dynamic GetImportedValue(string identifier) => _importedBindings[identifier];

        public void AddOrUpdateBinding(string identifier, dynamic value) => _importedBindings[identifier] = value;

        public override string ToString() => $"{nameof(Namespace)}(\"{Name}\")";
    }
}
