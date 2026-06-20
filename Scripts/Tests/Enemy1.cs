using Godot;
using System;

public partial class Enemy1 : Area2D
{
	private PathFollow2D pathFollow;
	
	private double moveSpeed = 100.0;
	
	private float lastProgressRatio = 0.0f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		pathFollow.Progress += (float)(moveSpeed * delta);
		if (pathFollow.ProgressRatio < lastProgressRatio)  
		{
			QueueFree();
			GD.Print("Damage");
		}
		lastProgressRatio = pathFollow.ProgressRatio;
	}
	
	public void Setup(PathFollow2D newPathFollow, double newMoveSpeed = 100)
	{
		pathFollow = newPathFollow;
		moveSpeed = newMoveSpeed;
	}
}
