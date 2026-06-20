using Godot;
using System;

public partial class PreviewTower : Node2D
{
    [Export] public PackedScene TowerScene;
    private Path2D path;
    public override void _Ready()
    {
        GlobalPosition = GetViewport().GetMousePosition();
        path = GetTree().Root.GetNode<Path2D>("Node2D/Path2D");
    }

    public override void _Process(double delta)
    {
        var closest = path.Curve.GetClosestPoint(GlobalPosition);
        GetChild<Sprite2D>(0).Modulate = GlobalPosition.DistanceSquaredTo(closest) > 1296 ? Colors.Green : Colors.Red;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            GlobalPosition = motion.Position;
        }
        
        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && !mb.Pressed)
        {
            var closest = path.Curve.GetClosestPoint(GlobalPosition);
            if (GlobalPosition.DistanceSquaredTo(closest) > 1296 && TowerScene.Instantiate() is Node2D towerInstance)
            {
                towerInstance.GlobalPosition = GlobalPosition;
                GetTree().Root.AddChild(towerInstance);
                QueueFree();
            }
        }
    }
}
