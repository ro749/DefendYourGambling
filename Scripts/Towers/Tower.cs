using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Tower : Area2D
{
	[Signal]
	public delegate void ShootEventHandler(Vector2 pos, float direction, int bulletEnum);
	
	protected List<Enemy1> _nearbyEnemies = new List<Enemy1>();
	
	public override void _Process(double delta)
	{
		if (_nearbyEnemies.Count == 0) return;
		GetNode<Sprite2D>("Turret").LookAt(_nearbyEnemies[0].GlobalPosition);
	}
	
	private void _OnReloadTimerTimeout()
	{
		if (_nearbyEnemies.Count == 0) return;
		EmitSignal("Shoot", GlobalPosition, GetNode<Sprite2D>("Turret").Rotation, (int)Data.Bullet.SINGLE);
	}

	private void _onEnemyDetectionAreaAreaEntered(Area2D area)
	{
		if (area is not Enemy1 enemy || _nearbyEnemies.Contains(enemy)) return;
		//enemy.Connect("Died", Callable.From(() => _onEnemyDied(enemy)));
		enemy.Died += _onEnemyDied;
		_nearbyEnemies.Add(enemy);
	}
	
	private void _onEnemyDetectionAreaAreaExited(Area2D area)
	{
		if (area is not Enemy1 enemy || !_nearbyEnemies.Contains(enemy)) return;
		//enemy.Disconnect("Died", Callable.From(() => _onEnemyDied(enemy)));
		enemy.Died -= _onEnemyDied;
		_nearbyEnemies.Remove(enemy);
	}

	private void _onEnemyDied(Enemy1 enemy)
	{
		if (_nearbyEnemies.Contains(enemy))
		{
			GD.Print("Enemy died, removing from nearby enemies list.");
			_nearbyEnemies.Remove(enemy);
		}
	}
}
