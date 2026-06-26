using Godot;
using System;

public partial class PreviewTower : Node2D
{
    [Export] public Sprite2D sprite;
    [Export] public Area2D area;
	[Export] public Data.TowerType towerType;
    private Path2D path;

    public TowerButton towerButton;
    public override void _Ready()
    {
        GlobalPosition = GetViewport().GetMousePosition();
        path = GetTree().Root.GetNode<Path2D>("Map/Path2D");
    }

    public override void _Process(double delta)
    {
        var closest = path.Curve.GetClosestPoint(GlobalPosition);
        sprite.Modulate = canPlace() ? Colors.Green : Colors.Red;
    }

    public bool canPlace()
    {
        var closest = path.Curve.GetClosestPoint(GlobalPosition);
        var not_in_path = GlobalPosition.DistanceSquaredTo(closest) > 1296;
        if (!not_in_path) return false;
        var spawnOver = Data.SpawnOver(towerType);
        if(spawnOver == Data.TowerType.NONE)
        {
            return area.GetOverlappingAreas().Count == 0;
        }
        foreach(var area in area.GetOverlappingAreas())
        {
            if(area is Tower tower && tower.towerType == spawnOver)
            {
                return true;
            }
        }
        return false;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            GlobalPosition = motion.Position;
        }
        
        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && !mb.Pressed)
        {
            if (canPlace())
            {
                Place();
            }
            else
            {
                towerButton.Modulate = new Color(1, 1, 1, 1f);
            }
            QueueFree();
        }
    }

    public void Place()
    {
        var spawnOver = Data.SpawnOver(towerType);
        if(spawnOver == Data.TowerType.NONE)
        {
            GetTree().Root.GetNode<TestLevel>("Map").CreateTower(towerType, GlobalPosition);
        }
        foreach(var area in area.GetOverlappingAreas())
        {
            if(area is Tower tower && tower.towerType == spawnOver)
            {
                GetTree().Root.GetNode<TestLevel>("Map").CreateTower(towerType, tower.GlobalPosition);
                tower.QueueFree();
                break;
            }
        }
        towerButton.QueueFree();
    }
}
