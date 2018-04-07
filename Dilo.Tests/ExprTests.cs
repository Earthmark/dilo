using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;

namespace Dilo.Tests
{
  [TestFixture]
  public class ExprTests
  {
    public static (string expr, List<Expr> items)[] Expressions { get; } =
    {
      ("1x+2y-3z+6q+7=0", new List<Expr> {new Expr(new[] {(1, "x"), (2, "y"), (-3, "z"), (6, "q"), (7, string.Empty)})}),
      ("-10x+6q+7=0", new List<Expr> {new Expr(new[] {(-10, "x"), (6, "q"), (7, string.Empty) })}),
      ("1xyz+6q+7=0\n-1231x+2y-3z+6q+7=0", new List<Expr>
      {
        new Expr(new[] {(1, "xyz"), (6, "q"), (7, string.Empty) }),
        new Expr(new[] {(-1231, "x"), (2, "y"), (-3, "z"), (6, "q"), (7, string.Empty) })
      })
    };

    [Test]
    public void TestParse([ValueSource(nameof(Expressions))](string expr, List<Expr> items) ite)
    {
      var items = Expr.Parse(ite.expr);
      var logic = new CompareLogic();
      var result = logic.Compare(items, ite.items);
      Assert.True(result.AreEqual, result.DifferencesString);
    }

    public static (string expr, int cmax)[] CMaxExpressions { get; } =
    {
      ("1a+2b-3c+6d+7=0", 16),
      ("-10a+6b+7=0", 13),
      ("1a+6b+7=0", 14),
      ("-1a-7b+6=0", 8),
      ("-1a-6b+7=0", 7)
    };

    [Test]
    public void TestCMax([ValueSource(nameof(CMaxExpressions))] (string expr, int cmax) ite)
    {
      Assert.AreEqual(ite.cmax, Expr.Parse(ite.expr)[0].CMax);
    }
  }
}
