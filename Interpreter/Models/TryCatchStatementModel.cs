using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class TryCatchStatementModel : IStatementModel
    {
        public TryStatement Try { get; set; }
        public CatchStatement Catch { get; set; }
    }

    public class TryStatement
    {
        public BlockModel Body { get; set; }
    }

    public class CatchStatement
    {
        public string ArgumentName { get; set; }
        public BlockModel Body { get; set; }
    }
}
