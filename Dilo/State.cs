using System;
using System.Collections.Generic;
using System.Linq;

namespace Dilo
{
  public class State
  {
    public Dictionary<int, State> Transitions { get; }
    public bool Accepting { get; }

    public State(bool accepting)
    {
      Accepting = accepting;
      Transitions = new Dictionary<int, State>();
    }

    public static State Create(Expr exp, Dictionary<string, int> inputPositionMap)
    {
      var cmax = exp.CMax;
      var imax = Util.KcOf(exp.Items[string.Empty]) + 1;

      var states =
        from carry in Enumerable.Range(-cmax, cmax * 2 + 1)
        from i in Enumerable.Range(1, imax)
        select (State: new State(carry == 0 && i == imax), Carry: carry, I: i);

      var stateMap = states.ToDictionary(ite => (Carry: ite.Carry, I: ite.I), ite => ite.State);

      var inputs = (int) Math.Pow(2, inputPositionMap.Count);

      foreach ((State state, int input, int carry, int i) in
        from state in stateMap
        from input in Enumerable.Range(0, inputs)
        select (state.Value, input, state.Key.Carry, state.Key.I))
      {
        var rComponents =
          from item in exp.Items
          where item.Key != string.Empty
          let inputBit = (input >> inputPositionMap[item.Key]) & 1
          select inputBit * item.Value;

        var rSum = rComponents.Sum();
        var bi = Util.BiOf(i, exp.Items[string.Empty]);
        var r = rSum + bi + carry;

        if ((r & 1) == 0)
        {
          var key = (r / 2, Math.Min(imax, i + 1));
          state.Transitions.Add(input, stateMap[key]);
        }
      }

      return stateMap[(0, 1)];
    }
  }
}
