using Godot;
using System;

public partial class TowerBasic : TowerParent
{
	public override void _Process(double delta)
	{
		if (_nearbyEnemies.Count == 0) return;
		GetNode<Sprite2D>("Turret").LookAt(_nearbyEnemies[0].GlobalPosition);
	}
	
	private void OnReloadTimerTimeout()
	{
		//EmitSignal(Shoot, GlobalPosition, 0, Data.Bullet.SINGLE);
	}
}
