using Godot;
using System;

public partial class PreviewTower : Node2D
{
    [Export] public PackedScene TowerScene;
    [Export] public Sprite2D sprite;
    [Export] public Area2D area;
    private Path2D path;
    public override void _Ready()
    {
        GlobalPosition = GetViewport().GetMousePosition();
        path = GetTree().Root.GetNode<Path2D>("Node2D/Path2D");
    }

    public override void _Process(double delta)
    {
        var closest = path.Curve.GetClosestPoint(GlobalPosition);
        sprite.Modulate = canPlace() ? Colors.Green : Colors.Red;
    }

    public bool canPlace()
    {
        var closest = path.Curve.GetClosestPoint(GlobalPosition);
        return GlobalPosition.DistanceSquaredTo(closest) > 1296 && area.GetOverlappingAreas().Count == 0;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            GlobalPosition = motion.Position;
        }
        
        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && !mb.Pressed)
        {
            if (canPlace() && TowerScene.Instantiate() is Node2D towerInstance)
            {
                towerInstance.GlobalPosition = GlobalPosition;
                GetTree().Root.AddChild(towerInstance);
                QueueFree();
            }
        }
    }
}
