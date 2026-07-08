using Godot;
using System;

public partial class PreviewTower : Node2D
{
    [Export] public Texture2D texture;
    [Export] public PackedScene spritesScene;
    [Export] public Area2D area;
	[Export] public Data.TowerType towerType;
    private Path2D path;
    public TextureRect sprite;

    public TowerButton towerButton;
    public override void _Ready()
    {
        GlobalPosition = GetViewport().GetMousePosition();
        path = GetTree().Root.GetNode<Path2D>("Map/Path2D");
        GetTree().Root.GetNode<TestLevel>("Map").towerPreview = this;
        sprite = spritesScene.Instantiate<TextureRect>();
        GetTree().Root.GetNode("Map/CanvasLayer").AddChild(sprite);
        sprite.Texture = texture;
        sprite.GlobalPosition = GlobalPosition-new Vector2(sprite.Size.X/2, sprite.Size.X/2);
    }

    public override void _Process(double delta)
    {
        sprite.Modulate = canPlace() ? Colors.Green : Colors.Red;
        if (Input.IsActionJustPressed("pause"))
        {
            towerButton.Modulate = new Color(1, 1, 1, 1f);
            QueueFree();
            sprite.QueueFree();
        }
    }

    public bool canPlace()
    {
        var closest = path.Curve.GetClosestPoint(GlobalPosition);
        var not_in_path = GlobalPosition.DistanceSquaredTo(closest) > 1296;
        if (!not_in_path) return false;
        var spawnOver = Data.SpawnOver(towerType);
        if(spawnOver == Data.TowerType.NONE)
        {
            return area.GetOverlappingAreas().Count == 1 && area.GetOverlappingAreas()[0].Name == "MapArea";
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
            sprite.GlobalPosition = motion.Position-new Vector2(sprite.Size.X/2, sprite.Size.Y/2);;
        }
        
        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && !mb.Pressed)
        {
            if (canPlace())
            {
                Place();
            }
            else
            {
                Cancel();
            }
            
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
        QueueFree();
        sprite.QueueFree();
    }

    public void Cancel()
    {
        if (!IsInstanceValid(towerButton)) return;
        
        towerButton.Modulate = new Color(1, 1, 1, 1f);
        
        if (!IsInstanceValid(this)) return;
        QueueFree();
        sprite.QueueFree();
    }
}
