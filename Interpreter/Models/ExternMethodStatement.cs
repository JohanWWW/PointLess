using Interpreter.Models.Interfaces;
using Interpreter.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class ExternMethodStatement : ModelBase, IFunctionModel
    {
        private const byte HAS_RETURN_STATEMENT = 1;
        private const byte METHOD_TYPE_MASK = 0b11;

        /// <summary>
        /// [1]     : HasReturnStatement
        /// [2]     : MethodType (range)
        /// [4]     : MethodType (range)
        /// [8]     : (not in use)          might be used by MethodType if more types are implemented
        /// [16]    : (not in use)
        /// [32]    : (not in use)
        /// [64]    : (not in use)
        /// [128]   : (not in use)
        /// </summary>
        private byte _props = 0b0;

        public string[] Parameters { get; set; }

        public Func<IList<IOperable>, IOperable> Implementation { get; set; }

        public bool HasParameters => Parameters.Length > 0;
        public bool HasReturnStatement
        {
            get => (_props & HAS_RETURN_STATEMENT) == HAS_RETURN_STATEMENT;
            set
            {
                byte b = value ? (byte)0b1 : (byte)0b0;
                _props = (byte)((~0b1 & _props) | b);
            }
        }
        public bool IsFunction => MethodType == Environment.MethodType.Function;
        public bool IsProvider => MethodType == Environment.MethodType.Provider;
        public bool IsConsumer => MethodType == Environment.MethodType.Consumer;
        public bool IsAction => MethodType == Environment.MethodType.Action;
        public int ParameterCount => Parameters.Length;
        public Environment.MethodType MethodType
        {
            get => (Environment.MethodType)((_props >> 1) & METHOD_TYPE_MASK);
            set
            {
                byte b = (byte)value;
                _props = (byte)((~(METHOD_TYPE_MASK << 1) & _props) | (b << 1));
            }
        }

        public ExternMethodStatement() : base(Enums.ModelTypeCode.ExternMethodStatement)
        {

        }

        public ExternMethodStatement(bool hasReturnStatement, params string[] parameters) : this()
        {
            HasReturnStatement = hasReturnStatement;
            Parameters = parameters;
            if (parameters.Length > 0 && hasReturnStatement)
                MethodType = Environment.MethodType.Function;
            else if (parameters.Length is 0 && hasReturnStatement)
                MethodType = Environment.MethodType.Provider;
            else if (parameters.Length > 0 && !hasReturnStatement)
                MethodType = Environment.MethodType.Consumer;
            else
                MethodType = Environment.MethodType.Action;
        }
    }
}
