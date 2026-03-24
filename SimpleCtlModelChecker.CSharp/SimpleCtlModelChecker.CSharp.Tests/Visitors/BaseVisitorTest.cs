using System.Net;
using SimpleCtlModelChecker.CSharp.Formulae;
using SimpleCtlModelChecker.CSharp.Visitors.Abstractions;
using Xunit;

namespace SimpleCtlModelChecker.CSharp.Tests.Visitors;

public abstract class BaseVisitorTest
{
    protected static readonly CtlFormula Safety = CtlFormula.Safety(CtlFormula.Atomic("ReactorGoesCritical"));
    protected static readonly CtlFormula Persistence = CtlFormula.Persistence(CtlFormula.Atomic("BossIsDead"));
    protected static readonly CtlFormula Recurrence = CtlFormula.Recurrence(CtlFormula.Atomic("GoodThing"));
    protected static readonly CtlFormula Response = CtlFormula.Response(CtlFormula.Atomic("RequestSent"), CtlFormula.Atomic("ResponseReceived"));

    // TODO I would like to have an easy way to define here
    // the simple formulae too, so that derived classes just have to
    // say what the expected results are
    protected static void TestVisitor<T>(CtlFormulaVisitor<T> visitor, CtlFormula formula, T expected)
    {
        var result = visitor.Visit(formula);
        Assert.Equal(expected, result);
    }
}
