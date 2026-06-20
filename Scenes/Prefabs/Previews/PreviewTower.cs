using Godot;
using System;

public partial class PreviewTower : Node2D
{
    [Export] public PackedScene TowerScene;
    public override void _Ready()
    {
        GlobalPosition = GetViewport().GetMousePosition();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            GlobalPosition = motion.Position;
        }
        
        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && !mb.Pressed)
        {
            if (TowerScene.Instantiate() is Node2D towerInstance)
            {
                towerInstance.GlobalPosition = GlobalPosition;
                GetTree().Root.AddChild(towerInstance);
                QueueFree();
            }
        }
    }
}
