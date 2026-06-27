using Godot;
using System;

public enum ReelResultType { 
    WIN, 
    JACKPOT, 
    LOOSE,
    TOWER_BASIC,
    TOWER_HEAVY,
    MULTY_4,
    MULTY_8,
    SNIPER,
    SNIPER_HEAVY
}

public partial class ReelResult : Control
{
    [Export] public int result = 2;
    [Export] public ReelResultType type;

    public int getResult()
    {
        return result;
    }
}
