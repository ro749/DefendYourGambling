using Godot;
using System;

public partial class ReelResult : Control
{
    [Export] public int result = 2;
    public int getResult()
    {
        return result;
    }
}
