using Godot;
using Godot.Collections;
using System;

public partial class Reel : Node
{
    [Export] public Array<PackedScene> TowerButtons;

    public TowerButton SelectedTowerButton;

    public void roll()
    {
        if(GodotObject.IsInstanceValid(SelectedTowerButton))
        {
            SelectedTowerButton.QueueFree();
        }
        SelectedTowerButton = (TowerButton)TowerButtons[(int)(GD.Randi() % TowerButtons.Count)].Instantiate();
        AddChild(SelectedTowerButton);
    }

}
