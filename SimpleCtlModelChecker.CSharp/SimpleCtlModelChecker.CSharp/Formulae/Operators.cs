// ReSharper disable InconsistentNaming
namespace SimpleCtlModelChecker.CSharp.Formulae;

public enum BinaryOperator
{
    And = 1,
    Or,
    Implies,
    AU,
    EU,
}

public enum UnaryOperator
{
    Not = 1,
    AG,
    EG,
    AF,
    EF,
    AX,
    EX,
}