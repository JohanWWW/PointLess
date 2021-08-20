using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class RuntimeObject
    {
        protected readonly IDictionary<string, IOperable> _bindings;

        public ICollection<string> MemberNames => _bindings.Keys;

        public RuntimeObject()
        {
            _bindings = new Dictionary<string, IOperable>();
        }

        public RuntimeObject(IDictionary<string, IOperable> initialBindings) // Note by ref
        {
            _bindings = initialBindings;
        }

        public IOperable this[string name]
        {
            get => _bindings[name];
            set => _bindings[name] = value;
        }

        public bool ContainsMember(string name) => 
            _bindings.ContainsKey(name);

        public bool TryGetMember(string name, out IOperable value) =>
            _bindings.TryGetValue(name, out value);

        public bool TrySetMember(string name, IOperable value)
        {
            if (_bindings.ContainsKey(name))
            {
                this[name] = value;
                return true;
            }

            return _bindings.TryAdd(name, value);
        }

        public bool TryAddMember(string name, IOperable value) =>
            _bindings.TryAdd(name, value);
    }
}
