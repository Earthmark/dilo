using System;
using System.IO;

namespace Dilo
{
  class Program
  {
    static void Main(string[] args)
    {
      var depth = int.Parse(args[0]);
      var text = File.ReadAllText(args[1]);
      var exprs = StateMachine.Create(Expr.Parse(text));
      var count = 0;
      foreach (var solution in exprs.Solutions(depth))
      {
        Console.WriteLine($"{count++}: {solution}");
      }
    }
  }
}
