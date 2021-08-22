using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models.Enums
{
    public enum ModelTypeCode
    {
        ActionStatement,
        AssignStatement,
        BinaryExpression,
        Block,
        ConditionalStatement,
        ConditionalTernaryExpression,
        ConsumerStatement,
        MethodCallStatement,
        FunctionStatement,
        IdentifierExpression,
        LambdaFunctionStatement,
        LiteralExpression,
        NativeActionStatement,
        NativeConsumerStatement,
        NativeFunctionStatement,
        NativeProviderStatement,
        ObjectInitializationExpression,
        ProviderStatement,
        Root,
        ThrowStatement,
        TryCatchStatement,
        UnaryExpression,
        UseStatement,
        WhileLoopStatement,
        ForeachLoopStatement,
        ArrayLiteralNotation,
        DictionaryLiteralNotation,
        IfClause,
        ElseIfClause,
        ElseClause
    }
}
