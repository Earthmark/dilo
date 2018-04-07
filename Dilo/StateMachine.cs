using System;
using System.Collections.Generic;
using System.Linq;

namespace Dilo
{
  public class StateMachine
  {
    private readonly Dictionary<string, int> _inputPositionMap;
    public State State { get; }

    public StateMachine(State state, Dictionary<string, int> inputPositionMap)
    {
      _inputPositionMap = inputPositionMap;
      State = state;
    }

    public IEnumerable<Dictionary<string, int>> Solutions()
    {
      var traversed = new HashSet<State>();
      IEnumerable<IEnumerable<int>> InputSolutions(State state)
      {
        if (state.Accepting)
        {
          return new[] {Enumerable.Empty<int>()};
        }
        if (traversed.Contains(state))
        {
          return Enumerable.Empty<IEnumerable<int>>();
        }
        traversed.Add(state);

        return
          from transition in state.Transitions
          from set in InputSolutions(transition.Value)
          select new[] {transition.Key}.Concat(set);
      }

      var foundSolutions = InputSolutions(State).ToList();

      return
        from viable in foundSolutions
        let mapped = _inputPositionMap.ToDictionary(kvp => kvp.Key, kvp =>
          (from input in viable.Zip(InfiniteLoop(0), (val, index) => (Val: val, Index: index))
            let bitPosition = kvp.Value
            let bit = ((input.Val >> bitPosition) & 1) << input.Index
            select bit).Sum()
        )
        select mapped;
    }


    public IEnumerable<int> InputMap(Dictionary<string, int> inputs) =>
      from index in Enumerable.Range(0, 30)
      let aggregated = (from input in inputs
        let bit = (input.Value >> index) & 1
        select bit << _inputPositionMap[input.Key]).Aggregate((a, b) => a | b)
      select aggregated;

    public static StateMachine Create(IEnumerable<Expr> expressions)
    {
      expressions = expressions.ToList();

      var variables =
        from expr in expressions
        from variable in expr.Items.Keys
        where !string.IsNullOrWhiteSpace(variable)
        select variable;

      var inputs = variables.Distinct().Zip(InfiniteLoop(0), (name, index) => (Name: name, Index: index))
        .ToDictionary(ite => ite.Name, ite => ite.Index);

      var compiled = from expr in expressions
        let state = State.Create(expr, inputs)
        select state;

      var inputCount = (int)Math.Pow(2, inputs.Count);

      var result = compiled.Aggregate((s1, s2) => ProductOfStates(s1, s2, inputCount));

      return new StateMachine(result, inputs);
    }

    public static State ProductOfStates(State state1, State state2, int inputs)
    {
      var set1 = Traverse(state1);
      var set2 = Traverse(state2);

      var pairs =
        from s1 in set1
        from s2 in set2
        select (S1: s1, S2: s2);

      var destStates = pairs.ToDictionary(ite => ite, ite => new State(ite.S1.Accepting && ite.S2.Accepting));

      foreach (var (target, state, input) in
        from kvp in destStates
        let s1 = kvp.Key.S1
        let s2 = kvp.Key.S2
        let state = kvp.Value
        from input in Enumerable.Range(0, inputs)
        where s1.Transitions.ContainsKey(input) && s2.Transitions.ContainsKey(input)
        let target = (s1.Transitions[input], s2.Transitions[input])
        select (target, state, input))
      {
        state.Transitions.Add(input, destStates[target]);
      }

      return destStates[(state1, state2)];
    }

    public static HashSet<State> Traverse(State state)
    {
      var results = new HashSet<State>();
      void Traverser(State self)
      {
        if (!results.Contains(self))
        {
          results.Add(self);
          foreach (var child in self.Transitions.Values)
          {
            Traverser(child);
          }
        }
      }
      Traverser(state);
      return results;
    }


    private static IEnumerable<int> InfiniteLoop(int start)
    {
      while (true)
      {
        yield return start++;
      }
    }
  }
}
