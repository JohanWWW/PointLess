using Antlr4.Runtime;
using Interpreter.Models.Enums;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public abstract class ModelBase : IModel
    {
        public IToken StartToken { get; set; }
        public IToken StopToken { get; set; }

        public ModelTypeCode TypeCode { get; }

        protected ModelBase(ModelTypeCode typeCode)
        {
            TypeCode = typeCode;
        }

        protected ModelBase(ModelTypeCode typeCode, IToken start, IToken stop)
        {
            TypeCode = typeCode;
            StartToken = start;
            StopToken = stop;
        }

        public override string ToString() => $"RuntimeModel<{nameof(TypeCode)}: {TypeCode}>";
    }
}
