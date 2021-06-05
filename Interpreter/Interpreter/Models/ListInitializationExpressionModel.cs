using Interpreter.Models.Interfaces;

namespace Interpreter.Models
{
    public class ListInitializationExpressionModel : IExpressionModel
    {
        public IExpressionModel[] Elements { get; set; }

        public override string ToString() => $"[{string.Join<IExpressionModel>(",", Elements)}]";
    }
}