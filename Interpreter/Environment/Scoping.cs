using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Environment
{
    public class Scoping
    {
        private readonly IDictionary<string, dynamic> _variables;

        private Scoping _previous = null;

        public Scoping() =>
            _variables = new Dictionary<string, dynamic>();

        public bool ContainsLocal(string identifier) => _variables.ContainsKey(identifier);

        public bool ContainsBacktrack(string identifier)
        {
            Scoping currentScope = this;
            while (currentScope != null)
            {
                if (currentScope.ContainsLocal(identifier))
                    return true;

                currentScope = currentScope._previous;
            }
            return false;
        }

        public dynamic GetLocalVariable(string identifier) => _variables[identifier];

        public dynamic GetBacktrackedVariable(string identifier)
        {
            Scoping currentScope = this;
            while (currentScope != null)
            {
                if (currentScope.ContainsLocal(identifier))
                    return currentScope.GetLocalVariable(identifier);

                currentScope = currentScope._previous;
            }

            throw new KeyNotFoundException($"Could not find variable named '{identifier}'");
        }

        public void SetBacktrackedVariable(string identifier, dynamic value)
        {
            Scoping currentScope = this;
            while (currentScope != null)
            {
                if (currentScope.ContainsLocal(identifier))
                {
                    currentScope._variables[identifier] = value;
                    return;
                }

                currentScope = currentScope._previous;
            }

            throw new KeyNotFoundException($"Could not find variable named '{identifier}'");
        }

        public void AddLocalVariable(string identifier, dynamic value)
        {
            _variables.Add(identifier, value);
        }

        public void ConsumeScope(Scoping scope)
        {
            foreach (var kvp in scope.GetBindings())
            {
                AddLocalVariable(kvp.Key, kvp.Value);
            }
        }

        public void SetLeftScope(Scoping scope)
        {
            _previous = scope;
        }

        public IDictionary<string, dynamic> GetBindings() => _variables;
    }
}
