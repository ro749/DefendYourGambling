using Godot;
using System;
using System.Collections.Generic;

#nullable enable
[GlobalClass]
public partial class Tower : Area2D
{
	[Signal]
	public delegate void ShootEventHandler(Vector2 pos, float direction, int bulletEnum);
	
	protected List<Enemy> _nearbyEnemies = new List<Enemy>();

	[Export]
	public Data.Bullet bulletType = Data.Bullet.SINGLE;
	
	[Export]
    public Data.TowerSeekFnc towerSeekFunction = Data.TowerSeekFnc.FURTHEST;
	
	public override void _Process(double delta)
	{
		if (_nearbyEnemies.Count == 0) return;
		Enemy? selectedEnemy = Data.SelectEnemy(_nearbyEnemies, towerSeekFunction);
		if (selectedEnemy == null) return;
		GetNode<Sprite2D>("Turret").LookAt(selectedEnemy.GlobalPosition);
	}
	
	private void _OnReloadTimerTimeout()
	{
		if (_nearbyEnemies.Count == 0) return;
		EmitSignal("Shoot", GlobalPosition, GetNode<Sprite2D>("Turret").Rotation, (int)bulletType);
	}

	private void _onEnemyDetectionAreaAreaEntered(Area2D area)
	{
		if (area is not Enemy enemy || _nearbyEnemies.Contains(enemy)) return;
		enemy.Died += _onEnemyDied;
		_nearbyEnemies.Add(enemy);
	}
	
	private void _onEnemyDetectionAreaAreaExited(Area2D area)
	{
		if (area is not Enemy enemy || !_nearbyEnemies.Contains(enemy)) return;
		enemy.Died -= _onEnemyDied;
		_nearbyEnemies.Remove(enemy);
	}

	private void _onEnemyDied(Enemy enemy)
	{
		if (_nearbyEnemies.Contains(enemy))
		{
			_nearbyEnemies.Remove(enemy);
		}
	}
}
