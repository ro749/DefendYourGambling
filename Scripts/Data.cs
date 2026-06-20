using Godot;
using System;
using System.Collections.Generic;

public struct BulletStats
{
	public float damage;
}

public partial class Data : Node
{
	public enum Bullet { SINGLE, HEAVY };
	
	public static Dictionary<Bullet, BulletStats> bulletStats = new Dictionary<Bullet, BulletStats>()
	{
		{ Bullet.SINGLE, new BulletStats { damage = 20 } },
		{ Bullet.HEAVY, new BulletStats { damage = 30 } }
	};
}
