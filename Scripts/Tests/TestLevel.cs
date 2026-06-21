using Godot;
using System;

public partial class TestLevel : Node
{
    [Export]
    private int enemyCount = 5;
    private int currentEnemyCount = 0;
    
    [Export]
    private double enemyTimeInterval = 1.0;
    private double currentEnemyTime = 1.0;
    
    private PackedScene enemyScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Enemies/enemy.tscn");
    
    public override void _Ready()
    {
        Godot.Collections.Array<Node> foundNodes = GetNode<Node2D>("/root/Map/Towers").FindChildren("*", typeof(Tower).Name);
        foreach (Node node in foundNodes)
        {
            if (node is Tower towerNode)
            {
                towerNode.Shoot += _CreateBullet;
            }
        }
    }
    
    public override void _Process(double delta)
    {
        if (currentEnemyCount < enemyCount)
        {
            currentEnemyTime += delta;

            if (currentEnemyTime >= enemyTimeInterval)
            {
                currentEnemyTime -= enemyTimeInterval;
                
                CreateEnemy(enemyScene);
                
                ++currentEnemyCount;
            }
        }
    }

    public void CreateTower(Data.Tower towerEnum, Vector2 position)
    {
        if (Data.towerStats.TryGetValue(towerEnum, out var towerStats) && towerStats.towerScene.Instantiate() is Tower towerInstance)
        {
            towerInstance.GlobalPosition = position;
            GetNode<Node2D>("Towers").AddChild(towerInstance);
            towerInstance.Shoot += _CreateBullet;
        }
    }
    
    public void CreateEnemy(PackedScene prefab)
    {
        PathFollow2D pathFollow = new PathFollow2D();

        if (prefab.Instantiate() is Enemy enemyInstance)
        {
            enemyInstance.Setup(pathFollow, 50, 100);
            pathFollow.AddChild(enemyInstance);
        }

        GetNode<Path2D>("Path2D").AddChild(pathFollow);
    }
    
    private void _CreateBullet(Vector2 pos, float direction, int bulletEnum)
    {
        if (Data.bulletStats.TryGetValue((Data.Bullet)bulletEnum, out var bulletStats) && bulletStats.bulletScene.Instantiate() is Bullet bulletInstance)
        {
            bulletInstance.Setup(pos + Vector2.FromAngle(direction) * 32, direction, (Data.Bullet)bulletEnum);
            GetNode<Node2D>("Bullets").AddChild(bulletInstance);
        }
    }
}
