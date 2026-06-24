using Godot;
using Godot.Collections;
using System;

public partial class Reel : Node
{
    [Export] public Array<PackedScene> TowerButtons;

    public ReelResult SelectedTowerButton;

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
