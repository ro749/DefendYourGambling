using Godot;
using System;
using Godot.Collections;

public partial class Reel : Node
{
    public ReelResult SelectedTowerButton;

    [Export] public Array<PackedScene> Results;
    [Export] public Array<int> Probabilities;

    public override void _Ready()
    {
        Results = GetParent<SlotMachine>().Results;
        Probabilities = GetParent<SlotMachine>().Probabilities;
    }

    public int roll()
    {
        if(IsInstanceValid(SelectedTowerButton))
        {
            SelectedTowerButton.QueueFree();
        }
        var total = 0;
        foreach (var n in Probabilities)
        {
            total += n;
        }
        var v = (int)(GD.Randi() % total);
        var i = 0;
        foreach (var n in Probabilities)
        {
            v -= n;
            if (v < 0)
            {
                break;
            }
            i++;
        }
        SelectedTowerButton = (ReelResult)Results[i].Instantiate();
        AddChild(SelectedTowerButton);
        SelectedTowerButton.AnchorBottom = .5f;
        SelectedTowerButton.AnchorTop = .5f;
        SelectedTowerButton.AnchorLeft = .5f;
        SelectedTowerButton.AnchorRight = .5f;
        return SelectedTowerButton.getResult();
    }
}
