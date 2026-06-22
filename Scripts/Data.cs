using Godot;
using System;
using System.Collections.Generic;

public struct BulletStats
{
	public float damage;
	public int maxPenetration;
	public PackedScene bulletScene;
}

public struct TowerStats
{
	public PackedScene towerScene;
}

#nullable enable
public partial class Data : Node
{
	public enum Bullet { SINGLE, HEAVY };

	public enum Tower { BASIC, HEAVY };

	public enum TowerSeekFnc { FURTHEST, STRONGEST, WEAKEST };
	
	public static Dictionary<Bullet, BulletStats> bulletStats = new Dictionary<Bullet, BulletStats>()
	{
		{ Bullet.SINGLE, new BulletStats { damage = 20, maxPenetration = 1, bulletScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Bullets/bullet_single.tscn") } },
		{ Bullet.HEAVY, new BulletStats { damage = 30, maxPenetration = 3, bulletScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Bullets/bullet_heavy.tscn") } }
	};

	public static Dictionary<Tower, TowerStats> towerStats = new Dictionary<Tower, TowerStats>()
	{
		{ Tower.BASIC, new TowerStats { towerScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Towers/tower_basic.tscn") } },
		{ Tower.HEAVY, new TowerStats { towerScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Towers/tower_heavy.tscn") } }
	};

	public static Enemy? SelectEnemy(List<Enemy> enemies, TowerSeekFnc seekFnc)
	{
		Enemy? selectedEnemy = null;
		if (enemies.Count == 0) return null;

		switch (seekFnc)
		{
			case TowerSeekFnc.FURTHEST:
				float maxProgress = -1f;
				foreach (var enemy in enemies)
				{
					if (enemy.PathFollow.Progress > maxProgress)
					{
						maxProgress = enemy.PathFollow.Progress;
						selectedEnemy = enemy;
					}
				}
				break;

			case TowerSeekFnc.STRONGEST:
				double maxHealth = -1.0;
				foreach (var enemy in enemies)
				{
					if (enemy.Health > maxHealth)
					{
						maxHealth = enemy.Health;
						selectedEnemy = enemy;
					}
				}
				break;

			case TowerSeekFnc.WEAKEST:
				double minHealth = double.MaxValue;
				foreach (var enemy in enemies)
				{
					if (enemy.Health < minHealth)
					{
						minHealth = enemy.Health;
						selectedEnemy = enemy;
					}
				}
				break;
		}

		return selectedEnemy;
	}
}
