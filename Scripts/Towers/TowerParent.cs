using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class TowerParent : Area2D
{
	protected List<Enemy1> _nearbyEnemies = new List<Enemy1>();
	[Signal]
	public delegate void ShootEventHandler(Vector2 pos, float direction, Data.Bullet bulletEnum);

	private void _onEnemyDetectionAreaAreaEntered(Area2D area)
	{
		if (area is not Enemy1 enemy || _nearbyEnemies.Contains(enemy)) return;
		_nearbyEnemies.Add(enemy);
	}
	
	private void _onEnemyDetectionAreaAreaExited(Area2D area)
	{
		if (area is not Enemy1 enemy || !_nearbyEnemies.Contains(enemy)) return;
		_nearbyEnemies.Remove(enemy);
	}
}
