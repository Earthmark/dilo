using System;
using System.Collections.Generic;
using System.Linq;

namespace Dilo
{
  public static class Util
  {
    public static int KcOf(int n) => (int)Math.Ceiling(Math.Log(n + 1, 2.0));

    public static int BiOf(int i, int c) => (c >> (i - 1)) & 1;

    public static int Inputs(this IEnumerable<Expr> exprs) =>
      (int) Math.Pow(2,
      exprs.SelectMany(ite => ite.Items.Keys).Where(ite => ite != string.Empty).Distinct().Count());
  }
}
