using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class RuntimeObject
    {
        private readonly IDictionary<string, IOperable> _bindings;

        public ICollection<string> MemberNames => _bindings.Keys;

        public RuntimeObject()
        {
            _bindings = new Dictionary<string, IOperable>();
        }

        public IOperable this[string name]
        {
            get => _bindings[name];
            set => _bindings[name] = value;
        }

        public bool TryGetMember(string name, out IOperable value) =>
            _bindings.TryGetValue(name, out value);

        public bool TrySetMember(string name, IOperable value) =>
            _bindings.TryAdd(name, value);
    }
}
