using Godot;
using System;

public partial class Enemy : Area2D
{
	[Signal]
	public delegate void DiedEventHandler(Enemy enemy);

	public PathFollow2D PathFollow{ get; private set; }

	private double _maxHealth = 50.0;
	public double Health { get; private set; } = 50.0;
	
	private double _moveSpeed = 100.0;
	
	private float _lastProgressRatio = 0.0f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		PathFollow.Progress += (float)(_moveSpeed * delta);
		if (PathFollow.ProgressRatio < _lastProgressRatio)  
		{
			DoDamageToPlayer();
			QueueFree();
		}
		_lastProgressRatio = PathFollow.ProgressRatio;
	}
	
	public void Setup(PathFollow2D newPathFollow, float newMaxHealth = 50, double newMoveSpeed = 100)
	{
		PathFollow = newPathFollow;
		_moveSpeed = newMoveSpeed;
		_maxHealth = newMaxHealth;
		Health = newMaxHealth;
	}

	public void _OnAreaEntered(Area2D area)
	{
		if (area is Bullet bullet)
		{
			Health -= Data.bulletStats[bullet.bulletEnum].damage;
			bullet.QueueFree();
			
			if (Health <= 0)
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

	public void DoDamageToPlayer()
	{
		PlayerState playerState = GetNode<PlayerState>("/root/PlayerState");
		playerState.LoseLife();
	}
}
