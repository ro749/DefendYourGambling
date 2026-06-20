using Godot;
using System;

public partial class Data : Node
{
	public enum Bullet { SINGLE, FIRE, MORTAL_EXPLOSION };
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
