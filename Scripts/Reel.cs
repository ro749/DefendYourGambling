using Godot;
using System;
using Godot.Collections;

public partial class Reel : Node
{
    public ReelResult SelectedTowerButton;

    private Array<PackedScene> TowerButtons;

    public override void _Ready()
    {
        TowerButtons = GetParent<SlotMachine>().TowerButtons;
    }

    public int roll()
    {
        if(IsInstanceValid(SelectedTowerButton))
        {
            SelectedTowerButton.QueueFree();
        }
        
        var v = (int)(GD.Randi() % TowerButtons.Count);
        
        SelectedTowerButton = (ReelResult)TowerButtons[v].Instantiate();
        AddChild(SelectedTowerButton);
        return SelectedTowerButton.getResult();
    }

}
