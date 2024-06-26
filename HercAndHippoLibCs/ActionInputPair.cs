﻿using System.Collections;
using System.Numerics;
namespace HercAndHippoLibCs;

public class ActionInputPair : IEquatable<ActionInputPair>, IEquatable<ActionInput>, IEnumerable<ActionInput>
{
    private readonly ActionInput[] inputs;
    public ActionInputPair(ActionInput first, ActionInput? secondOrNull = null)
    {
        ActionInput second = secondOrNull ?? ActionInput.NoAction;
        if (second == first)
            second = ActionInput.NoAction;
        // Put these in some deterministic order.
        // If NoAction is one of the two, it's second.
        bool firstFirst = second == ActionInput.NoAction || (int)first < (int)second;
        inputs = firstFirst ?
            new ActionInput[] { first, second } :
            new ActionInput[] { second, first };
    }
    public override int GetHashCode() => 19 * (int)inputs[0] + 31 * (int)inputs[1];
    public bool Equals(ActionInputPair? other)
        => other != null && inputs[0] == other.inputs[0] && inputs[1] == other.inputs[1];
    public override bool Equals(object? obj)  
    {
        if (obj == null)
            return false;
        else if (obj is ActionInput ai)
            return this.Equals(ai);
        else if (obj is ActionInputPair aip)
            return this.Equals(aip);
        else
            return false;
    }

    public IEnumerator<ActionInput> GetEnumerator()
        => ((IEnumerable<ActionInput>)inputs).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => inputs.GetEnumerator();
    public ActionInput First => inputs[0];
    public ActionInput Second => inputs[1];
    public bool IsComboAction => Second != ActionInput.NoAction;
    public override string ToString()
    {
        if (First == ActionInput.NoAction && Second == ActionInput.NoAction)
            return "No action.";
        else if (First == ActionInput.NoAction)
            return Second.ToString();
        else if (Second == ActionInput.NoAction)
            return First.ToString();
        else
            return ($"{First} and {Second}");
    }

    public bool Equals(ActionInput ai)
        => this.Second == ActionInput.NoAction && this.First == ai;
    

    public static implicit operator ActionInputPair(ActionInput input) => new(input, null);
    public static readonly ActionInputPair[] PossiblePairs = new ActionInputPair[]
    {
        new(ActionInput.NoAction),
        new(ActionInput.MoveWest),
        new(ActionInput.MoveEast),
        new(ActionInput.MoveNorth),
        new(ActionInput.MoveSouth),
        new(ActionInput.ShootWest),
        new(ActionInput.ShootEast),
        new(ActionInput.ShootNorth),
        new(ActionInput.ShootSouth),
        new(ActionInput.DropHippo),

        // Can east/west and north/south simultaneously
        new(ActionInput.MoveWest, ActionInput.MoveNorth),
        new(ActionInput.MoveWest, ActionInput.MoveSouth),
        new(ActionInput.MoveEast, ActionInput.MoveNorth),
        new(ActionInput.MoveEast, ActionInput.MoveSouth),

        // Can move and drop hippo simultaneously
        new(ActionInput.MoveWest, ActionInput.DropHippo),
        new(ActionInput.MoveEast, ActionInput.DropHippo),
        new(ActionInput.MoveNorth, ActionInput.DropHippo),
        new(ActionInput.MoveSouth, ActionInput.DropHippo),
    };
}
