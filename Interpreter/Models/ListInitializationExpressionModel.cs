using Antlr4.Runtime;
using Interpreter.Models.Interfaces;

namespace Interpreter.Models
{
    public class ListInitializationExpressionModel : IExpressionModel
    {
        public IExpressionModel[] Elements { get; set; }
        public IToken StartToken { get; set; }
        public IToken StopToken { get; set; }

        public override string ToString() => $"[{string.Join<IExpressionModel>(",", Elements)}]";
    }
}