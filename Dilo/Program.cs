using System;
using System.Linq;
using System.IO;

namespace Dilo
{
  class Program
  {
    static void Main(string[] args)
    {
      var text = File.ReadAllText(args[0]);
      var exprs = StateMachine.Create(Expr.Parse(text));
      var solutions = exprs.Solutions();
      var resultStr = string.Join(Environment.NewLine,
        from solution in solutions
        select string.Join(",",
          from variable in solution
          select $"{variable.Key}:{variable.Value}")
      );

      Console.WriteLine(resultStr);
    }
  }
}
