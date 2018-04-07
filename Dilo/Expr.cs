using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

namespace Dilo
{
  public class Expr
  {
    public Dictionary<string, int> Items { get; }

    public int CMax => Math.Max(
      Items.Values.Where(ite => ite > 0).Sum(),
      -Items.Values.Where(ite => ite < 0).Sum());

    public Expr(IEnumerable<(int mul, string name)> items)
    {
      Items = items.ToDictionary(tup => tup.name, tup => tup.mul);
    }

    public static List<Expr> Parse(string expression)
    {
      var lexer = new ExprsLexer(new AntlrInputStream(expression));
      var parser = new ExprsParser(new CommonTokenStream(lexer));
      var exprs = parser.expressions();
      var visits = new ExprsVisitor();
      return visits.Visit(exprs).ToList();
    }

    private class ExprsVisitor : ExprsBaseVisitor<IEnumerable<Expr>>
    {
      public override IEnumerable<Expr> VisitExpressions(ExprsParser.ExpressionsContext context)
      {
        return context.expression().SelectMany(Visit);
      }

      public override IEnumerable<Expr> VisitExpression(ExprsParser.ExpressionContext context)
      {
        var exprv = new ExprVisitor();
        var items = exprv.VisitExpression(context);
        return new[] {new Expr(items)};
      }
    }

    private class ExprVisitor : ExprsBaseVisitor<IEnumerable<(int, string)>>
    {
      public override IEnumerable<(int, string)> VisitExpression(ExprsParser.ExpressionContext context)
      {
        return VisitFirstExpr(context.firstExpr()).Concat(
          context.expr().SelectMany(VisitExpr)).Concat(
          VisitLastExpr(context.lastExpr()));
      }

      public override IEnumerable<(int, string)> VisitFirstExpr(ExprsParser.FirstExprContext context)
      {
        var num = int.Parse(context.Number()?.GetText() ?? "1");
        if (context.Neg() != null)
        {
          num = -num;
        }
        yield return (num, context.Token().GetText());
      }

      public override IEnumerable<(int, string)> VisitExpr(ExprsParser.ExprContext context)
      {
        var num = int.Parse(context.Number()?.GetText() ?? "1");
        if (context.Neg() != null)
        {
          num = -num;
        }
        yield return (num, context.Token().GetText());
      }

      public override IEnumerable<(int, string)> VisitLastExpr(ExprsParser.LastExprContext context)
      {
        var num = int.Parse(context.Number().GetText());
        yield return (num, string.Empty);
      }
    }
  }
}
