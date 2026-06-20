using Godot;
using System;

public partial class Enemy1 : Area2D
{
	[Signal]
	public delegate void DiedEventHandler(Enemy1 enemy);

	private PathFollow2D _pathFollow;

	private double _maxHealth = 50.0;
	private double _health = 50.0;
	
	private double _moveSpeed = 100.0;
	
	private float _lastProgressRatio = 0.0f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_pathFollow.Progress += (float)(_moveSpeed * delta);
		if (_pathFollow.ProgressRatio < _lastProgressRatio)  
		{
			QueueFree();
			GD.Print("Damage");
		}
		_lastProgressRatio = _pathFollow.ProgressRatio;
	}
	
	public void Setup(PathFollow2D newPathFollow, float newMaxHealth = 50, double newMoveSpeed = 100)
	{
		_pathFollow = newPathFollow;
		_moveSpeed = newMoveSpeed;
		_maxHealth = newMaxHealth;
		_health = newMaxHealth;
	}

	public void _OnAreaEntered(Area2D area)
	{
		if (area is Bullet bullet)
		{
			_health -= Data.bulletStats[bullet.bulletEnum].damage;
			bullet.QueueFree();
			GD.Print("Current health: " + _health);
			if (_health <= 0)
			{
				Die();
			}
		}
	}

	public void Die()
	{
		EmitSignal("Died", this);
		QueueFree();
	}
}
