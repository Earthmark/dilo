using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Dilo.Tests
{
  [TestFixture]
  public class StateTests
  {
    public static (string expr, int a, int b)[] ConstructorExprs { get; } =
    {
      ("-a-7b+6=0", 6, 0),
      ("-a-6b+7=0", 1, 1)
    };

    [Test]
    public void StateConstructor([ValueSource(nameof(ConstructorExprs))] (string expr, int a, int b) ite)
    {
      var indexMap = new Dictionary<string, int>
      {
        ["a"] = 0,
        ["b"] = 1
      };

      var exprs =  State.Create(Expr.Parse(ite.expr)[0], indexMap);

      var inputs = Enumerable.Range(0, 30).Select(index =>
        (((ite.a >> index) & 1) << 0) |
        (((ite.b >> index) & 1) << 1)).ToList();

      var current = exprs;

      foreach (var input in inputs)
      {
        current = current.Transitions[input];
      }

      Assert.True(current.Accepting);
    }

    public static (string expr, int a, int b, int c, int d)[] RandoConstructorExprs { get; } =
    {
      ("-b-7d+6=0", 4, 6, 200, 0),
      ("-b-6d+7=0", 0, 1, 7, 1)
    };

    [Test]
    public void StateConstructorUnbounded([ValueSource(nameof(RandoConstructorExprs))] (string expr, int a, int b, int c, int d) ite)
    {
      var indexMap = new Dictionary<string, int>
      {
        ["a"] = 0,
        ["b"] = 1,
        ["c"] = 2,
        ["d"] = 3
      };

      var exprs = State.Create(Expr.Parse(ite.expr)[0], indexMap);

      var inputs = Enumerable.Range(0, 30).Select(index =>
        (((ite.a >> index) & 1) << 0) |
        (((ite.b >> index) & 1) << 1) |
        (((ite.c >> index) & 1) << 2) |
        (((ite.d >> index) & 1) << 3)).ToList();

      var current = exprs;

      foreach (var input in inputs)
      {
        current = current.Transitions[input];
      }

      Assert.True(current.Accepting);
    }

    public static (string expr, int input, int kcp1)[] SingleStateExprs { get; } =
    {
      ("-a+1=0", 1, 2),
      ("-2a+6=0", 3, 4),
      ("-a+12=0", 12, 5)
    };

    [Test]
    public void SingleStateConstructor([ValueSource(nameof(SingleStateExprs))] (string expr, int input, int kcp1) ite)
    {
      var indexMap = new Dictionary<string, int>
      {
        ["a"] = 0,
      };

      var exprs = State.Create(Expr.Parse(ite.expr)[0], indexMap);

      var inputs = Enumerable.Range(0, 30).Select(index =>
        ((ite.input >> index) & 1) << 0).ToList();

      var current = exprs;

      foreach (var input in inputs)
      {
        current = current.Transitions[input];
      }

      Assert.True(current.Accepting);
    }

    public static (string expr, int input)[] BadSingleStateExprs { get; } =
    {
      ("-a+1=0", 3),
      ("-a+1=0", 0),
      ("-2a+6=0", 8),
      ("-a+12=0", 64)
    };

    [Test]
    public void BadSingleStateConstructor([ValueSource(nameof(BadSingleStateExprs))] (string expr, int input) ite)
    {
      var indexMap = new Dictionary<string, int>
      {
        ["a"] = 0,
      };

      var exprs = State.Create(Expr.Parse(ite.expr)[0], indexMap);

      var inputs = Enumerable.Range(0, 30).Select(index =>
        ((ite.input >> index) & 1) << 0).ToList();

      var current = exprs;

      Assert.Throws<KeyNotFoundException>(() =>
      {
        foreach (var input in inputs)
        {
          current = current.Transitions[input];
        }
      });
    }

    public static (string expr, (string name, int value)[] pairs)[] Problems { get; } =
    {
      ("-a-2b+7=0\n-a+3=0", new[]{("a", 3), ("b", 2)}),
      ("-a+1=0", new[]{("a", 1)}),
      ("-2a+6=0",new[]{("a", 3)}),
      ("-a+12=0", new[]{("a", 12)})
    };

    [Test]
    public void KnownStatements([ValueSource(nameof(Problems))] (string expr, (string name, int value)[] pairs) ite)
    {
      var indexMap = ite.pairs.ToDictionary(pair => pair.name, pair => pair.value);
      var exprs = StateMachine.Create(Expr.Parse(ite.expr));

      var current = exprs.State;

      foreach (var input in exprs.InputMap(indexMap))
      {
        current = current.Transitions[input];
      }

      exprs.Solutions();

      Assert.True(current.Accepting);
    }

    public static (string expr, (string name, int value)[] pairs)[] BadProblems { get; } =
    {
      ("-a-2b+7=0\n-a+3=0", new[]{("a", 1), ("b", 3)}),
    };

    [Test]
    public void KnownBadStatements([ValueSource(nameof(BadProblems))] (string expr, (string name, int value)[] pairs) ite)
    {
      var indexMap = ite.pairs.ToDictionary(pair => pair.name, pair => pair.value);
      var exprs = StateMachine.Create(Expr.Parse(ite.expr));

      var current = exprs.State;

      foreach (var input in exprs.InputMap(indexMap))
      {
        current?.Transitions.TryGetValue(input, out current);
      }

      Assert.False(current?.Accepting ?? false);
    }


    public static (string expr, int solutions, int depth)[] GuessProblems { get; } =
    {
      ("3a-2b+c+5=0\n6a-4b+2c+9=0", 0, 10),
      ("3a-2b-c+3=0\n6a-4b+c+3=0", 683, 10)
    };

    [Test]
    public void ProvidedExamples([ValueSource(nameof(GuessProblems))](string expr, int solutions, int depth) ite)
    {
      var exprs = StateMachine.Create(Expr.Parse(ite.expr));

      var solution = exprs.Solutions(ite.depth);

      Assert.AreEqual(ite.solutions, solution.Count());
    }
  }
}
