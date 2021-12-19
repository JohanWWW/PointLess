using Interpreter.Framework.Extern.Utils;
using Interpreter.Models;
using Interpreter.Models.Interfaces;
using Interpreter.Runtime;
using Interpreter.Types;
using Singulink.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using ClientType = Interpreter.Runtime.ObjectType;

namespace Interpreter.Framework.Extern
{
    // TODO: Implement two way mapping for arrays (input / output)
    // TODO: Implement two way mapping for objects (input / output)
    public abstract class ExternAPI
    {
        internal IReadOnlyDictionary<string, IFunctionModel> GetImplementations()
        {
            string apiName;
            Type type;
            ExternAPIAttribute apiAttr;
            MethodInfo[] methods;
            Dictionary<string, IFunctionModel> map = new();

            type = GetType();
            apiAttr = type.GetCustomAttribute<ExternAPIAttribute>();
            apiName = apiAttr?.Name ?? type.Name;

            methods = type.GetMethods();
            foreach (MethodInfo m in methods)
            {
                string endpointName;
                ExternEndpointAttribute endpointAttr;
                ClientType returnType = default;
                ClientType[] paramClientTypes = null;
                ExternParameterAttribute[] paramAttributes;

                endpointAttr = m.GetCustomAttribute<ExternEndpointAttribute>();

                // Is attribute present?
                if (endpointAttr is null)
                    continue;

                // Get the name of the externally implemented method
                endpointName = $"{apiName}.{endpointAttr.Name ?? m.Name}";

                // Get return type
                if (endpointAttr.ReturnType is null)
                {
                    Type mrt = m.ReturnType;
                    if (!TypeMappingHelper.TryGetClientType(mrt, out returnType))
                        throw new Exception($"invalid return type in extern method which had type {mrt.GetType().Name}");
                }
                else
                {
                    returnType = (ClientType)endpointAttr.ReturnType;
                }

                // Get parameter types
                ParameterInfo[] parameters = m.GetParameters();
                paramClientTypes = new ClientType[parameters.Length];
                paramAttributes = new ExternParameterAttribute[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    ParameterInfo param;
                    Type mpt;

                    param = parameters[i];
                    mpt = param.ParameterType;
                    paramAttributes[i] = param.GetCustomAttribute<ExternParameterAttribute>();

                    if (!TypeMappingHelper.TryGetClientType(mpt, out paramClientTypes[i]))
                        throw new Exception($"invalid parameter type for ${param.Name} which had type {mpt.Name}");
                }
                
                map.Add(endpointName, new ExternMethodStatement(returnType != ClientType.Void, Enumerable.Repeat<string>(null, paramClientTypes.Length).ToArray())
                {
                    Implementation = clientargs =>
                    {
                        object[] args = new object[clientargs?.Count ?? 0];

                        for (int i = 0; i < args.Length; i++)
                        {
                            if (paramAttributes[i] is not null)
                            {
                                if (paramAttributes[i].ImplicitResolve)
                                    args[i] = TypeMappingHelper.ToExternValue(clientargs[i], parameters[i].ParameterType);
                                else
                                {
                                    if (!TypeMappingHelper.TryGetMasterType(clientargs[i].OperableType, out Type implicitType))
                                        throw new Exception();
                                    args[i] = TypeMappingHelper.ToExternValue(clientargs[i], parameters[i].ParameterType, implicitType);
                                }

                                continue;
                            }
                            args[i] = TypeMappingHelper.ToExternValue(clientargs[i], parameters[i].ParameterType);
                        }

                        return TypeMappingHelper.ToClientValue(m.Invoke(this, args));
                    }
                });
            }

            return map;
        }


    }
}
