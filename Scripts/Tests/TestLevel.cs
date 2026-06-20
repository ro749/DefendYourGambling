using Godot;
using System;

public partial class TestLevel : Node
{
	private int enemyCount = 5;
	private int currentEnemyCount = 0;
	
	private double enemyTimeInterval = 1.0;
	private double currentEnemyTime = 1.0;
	
	private PackedScene enemyScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Enemies/enemy_1.tscn");
	private PackedScene bulletScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Bullets/bullet.tscn");
	
	public override void _Ready()
	{
		Godot.Collections.Array<Node> foundNodes = GetNode<Node2D>("Towers").FindChildren("*", typeof(Tower).Name);
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
	
	public void CreateEnemy(PackedScene prefab)
	{
		PathFollow2D pathFollow = new PathFollow2D();

		if (prefab.Instantiate() is Enemy1 enemyInstance)
		{
			enemyInstance.Setup(pathFollow, 50, 100);
			pathFollow.AddChild(enemyInstance);
		}

		GetNode<Path2D>("Path2D").AddChild(pathFollow);
	}
	
	private void _CreateBullet(Vector2 pos, float direction, int bulletEnum)
	{
		if (bulletScene.Instantiate() is Bullet bulletInstance)
		{
			bulletInstance.Setup(pos + Vector2.FromAngle(direction) * 32, direction, (Data.Bullet)bulletEnum);
			GetNode<Node2D>("Bullets").AddChild(bulletInstance);
		}
	}
}
