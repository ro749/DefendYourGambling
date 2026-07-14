using Godot;
using System;

public partial class UpgradePreview : PreviewTower
{
    public override void _Ready()
    {
        base._Ready();
        foreach (Tower tower in GetTree().Root.GetNode<Node2D>("Map/Towers").GetChildren()) {
            if (tower.towerType == Data.SpawnOver(towerType))
            {
                tower.Modulate = new Color(1.5f, 1.5f, 1.5f, 1f);
            }
            else
            {
                tower.Modulate = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
            
        }
    }

    public override void _ExitTree()
    {
        foreach (Tower tower in GetTree().Root.GetNode<Node2D>("Map/Towers").GetChildren()) {
            tower.Modulate = new Color(1f, 1f, 1f, 1f);
        }
    }
}