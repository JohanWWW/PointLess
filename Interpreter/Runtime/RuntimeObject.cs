using Interpreter.Environment;
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
                else if (memberValue is Method m)
                {
                    stringBuilder.Append(memberName).Append('=');
                    switch (m.MethodType)
                    {
                        case MethodType.Function:
                            stringBuilder.Append($"([{m.ParameterCount}])=>*");
                            break;
                        case MethodType.Action:
                            stringBuilder.Append("()=>{}");
                            break;
                        case MethodType.Consumer:
                            stringBuilder.Append($"([{m.ParameterCount}])=>{{}}");
                            break;
                        case MethodType.Provider:
                            stringBuilder.Append("()=>*");
                            break;
                    }
                }
                else if (memberValue is MethodData md)
                {
                    if (md.OverloadCount is 1)
                    {
                        Method mtd = md.GetSingle();
                        stringBuilder.Append(memberName).Append('=');
                        switch (mtd.MethodType)
                        {
                            case MethodType.Function:
                                stringBuilder.Append($"([{mtd.ParameterCount}])=>*");
                                break;
                            case MethodType.Action:
                                stringBuilder.Append("()=>{}");
                                break;
                            case MethodType.Consumer:
                                stringBuilder.Append($"([{mtd.ParameterCount}])=>{{}}");
                                break;
                            case MethodType.Provider:
                                stringBuilder.Append("()=>*");
                                break;
                        }
                    }
                }
                else if (memberValue is string s)
                {
                    stringBuilder.Append(memberName).Append('=').Append('"').Append(s.Replace("\\", "\\\\").Replace("\"", "\\\"")).Append('"');
                }
                else
                {
                    stringBuilder.Append(memberName).Append('=').Append('\'').Append(memberValue).Append('\'');
                }

                if (i < memberNames.Length - 1)
                    stringBuilder.Append(", ");
            }

            stringBuilder.Append('}');

            stringBuilder.Replace("\n", "\\n");
            stringBuilder.Replace("\t", "\\t");

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
