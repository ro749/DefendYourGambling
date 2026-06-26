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
	public enum BulletType { SINGLE, HEAVY };

	public enum TowerType { BASIC, HEAVY, PIERCING,
	                        MULTI_4, MULTI_8,
							SNIPER, SNIPER_HEAVY,
							EXPLOSIVE, NONE };

	public enum EnemyType { BASIC, TANK, FAST, FLYING, BOSS };

	public enum TowerSeekFnc { NONE, FURTHEST, STRONGEST, WEAKEST };
	
	public static Dictionary<BulletType, BulletStats> bulletStats = new Dictionary<BulletType, BulletStats>()
	{
		{ BulletType.SINGLE, new BulletStats { damage = 20, maxPenetration = 1, bulletScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Bullets/bullet_single.tscn") } },
		{ BulletType.HEAVY, new BulletStats { damage = 30, maxPenetration = 3, bulletScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Bullets/bullet_heavy.tscn") } },
	};

	public static Dictionary<TowerType, TowerStats> towerStats = new Dictionary<TowerType, TowerStats>()
	{
		{ TowerType.BASIC, new TowerStats { towerScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Towers/tower_basic.tscn") } },
		{ TowerType.HEAVY, new TowerStats { towerScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Towers/tower_heavy.tscn") } },
		//{ TowerType.PIERCING, new TowerStats { towerScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Towers/tower_piercing.tscn") } },
		{ TowerType.MULTI_4, new TowerStats { towerScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Towers/tower_multi_4.tscn") } },
		{ TowerType.MULTI_8, new TowerStats { towerScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Towers/tower_multi_8.tscn") } },
		{ TowerType.SNIPER, new TowerStats { towerScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Towers/tower_sniper.tscn") } },
		{ TowerType.SNIPER_HEAVY, new TowerStats { towerScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Towers/tower_sniper_heavy.tscn") } },
		//{ TowerType.EXPLOSIVE, new TowerStats { towerScene = GD.Load<PackedScene>("res://Scenes/Prefabs/Towers/tower_explosive.tscn") } },
	};

	public static Dictionary<EnemyType, PackedScene> enemyScenes = new Dictionary<EnemyType, PackedScene>()
	{
		{ EnemyType.BASIC, GD.Load<PackedScene>("res://Scenes/Prefabs/Enemies/enemy_basic.tscn") },
		{ EnemyType.TANK, GD.Load<PackedScene>("res://Scenes/Prefabs/Enemies/enemy_tank.tscn") },
		{ EnemyType.FAST, GD.Load<PackedScene>("res://Scenes/Prefabs/Enemies/enemy_fast.tscn") },
		//{ EnemyType.FLYING, GD.Load<PackedScene>("res://Scenes/Prefabs/Enemies/enemy_flying.tscn") },
		{ EnemyType.BOSS, GD.Load<PackedScene>("res://Scenes/Prefabs/Enemies/enemy_boss.tscn") },
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
					if (enemy.PathFollow?.Progress > maxProgress)
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

	public static TowerType SpawnOver(TowerType towerType)
	{
		switch (towerType)
		{
			case TowerType.HEAVY:
				return TowerType.BASIC;
			case TowerType.MULTI_8:
				return TowerType.MULTI_4;
			case TowerType.SNIPER_HEAVY:
				return TowerType.SNIPER;
			default:
				return TowerType.NONE;
		}
	}
}
