using Godot;
using System;
using Godot.Collections;
public partial class SlotMachine : HBoxContainer
{
    [Export] public Array<PackedScene> Results;
    [Export] public Array<int> Probabilities;
    
}
