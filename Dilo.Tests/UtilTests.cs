using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Dilo.Tests
{
  [TestFixture]
  class UtilTests
  {
    public static (int n, int kc)[] KcData { get; } =
    {
      (0, 0),
      (1, 1),
      (2, 2),
      (3, 2),
      (4, 3),
      (5, 3),
      (6, 3),
      (7, 3),
      (15, 4),
      (16, 5),
      (30, 5),
      (32, 6),
      (34, 6)
    };

    [Test]
    public void TestKcOf([ValueSource(nameof(KcData))] (int n, int kc) ite)
    {
      Assert.AreEqual(ite.kc, Util.KcOf(ite.n));
    }

    public static (int i, int c, int bi)[] BiData { get; } =
    {
      (1, 1, 1),
      (1, 2, 0),
      (2, 2, 1),
      (1, 4, 0),
      (2, 4, 0),
      (3, 4, 1),
      (4, 4, 0)
    };

    [Test]
    public void TestBiOf([ValueSource(nameof(BiData))] (int i, int c, int bi) ite)
    {
      Assert.AreEqual(ite.bi, Util.BiOf(ite.i, ite.c));
    }

    public static (string expr, int inputs)[] InputsExpressions { get; } =
    {
      ("1x+2y-3z+6q+7=0", 16),
      ("-10x+6q+7=0", 4),
      ("1xyz+6q+7=0", 4),
      ("18x+0=0", 2),
      ("100x+600q+700=0", 4),
      ("-5q+0=0", 2),
      ("1a+2b+3c+4=0\n5a+6b+7c+8=0", 8),
      ("1a+2b+3c+4=0\n5a+6b+7d+8=0", 16),
      ("1a+2b+3c+4=0\n5d+6e+7f+8=0", 64)
    };


    [Test]
    public void TestInputs([ValueSource(nameof(InputsExpressions))] (string expr, int inputs) ite)
    {
      Assert.AreEqual(ite.inputs, Expr.Parse(ite.expr).Inputs());
    }
  }
}
