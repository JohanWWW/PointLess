using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class RuntimeObject : DynamicObject
    {
        private IDictionary<string, object> _properties = new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string identifier = binder.Name;
            return _properties.TryGetValue(identifier, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string identifier = binder.Name;
            _properties[identifier] = value;
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (var member in _properties.Keys)
            {
                yield return member;
            }
        }

        public override string ToString() => GetTree(this);

        private string GetTree(RuntimeObject obj)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("obj{");

            var memberNames = obj.GetDynamicMemberNames().ToArray();
            for (int i = 0; i < memberNames.Length; i++)
            {
                string memberName = memberNames[i];
                object memberValue = obj._properties[memberName];

                if (memberValue is RuntimeObject)
                {
                    string tree = GetTree(memberValue as RuntimeObject);
                    stringBuilder.Append(memberName).Append('=').Append(tree);
                }
                else if (memberValue is Func<IList<dynamic>, dynamic>)
                {
                    string[] parameters = (memberValue as Func<IList<dynamic>, dynamic>).Method.GetParameters().Select(p => p.Name).ToArray();
                    stringBuilder.Append(memberName).Append('=').Append($"({string.Join(",", parameters)})=>*");
                }
                else if (memberValue is Func<dynamic>)
                {
                    stringBuilder.Append(memberName).Append('=').Append("()=>*");
                }
                else if (memberValue is Action)
                {
                    stringBuilder.Append(memberName).Append('=').Append("()=>{}");
                }
                else if (memberValue is Action<IList<dynamic>>)
                {
                    string[] parameters = (memberValue as Action<IList<dynamic>>).Method.GetParameters().Select(p => p.Name).ToArray();
                    stringBuilder.Append(memberName).Append('=').Append($"({string.Join(",", parameters)})=>{{}}");
                }
                else
                {
                    stringBuilder.Append(memberName).Append('=').Append("'").Append(memberValue).Append("'");
                }

                if (i < memberNames.Length - 1)
                    stringBuilder.Append(", ");
            }

            stringBuilder.Append('}');

            return stringBuilder.ToString();
        }

        public class SetterBinder : SetMemberBinder
        {
            public SetterBinder(string identifier) : base(identifier, false)
            {
            }

            public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion) =>
                throw new NotImplementedException();
        }

        public class GetterBinder : GetMemberBinder
        {
            public GetterBinder(string identifier) : base(identifier, false)
            {
            }

            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion) =>
                throw new NotImplementedException();
        }
    }
}
