using Godot;
using System.Collections.Generic;

public partial class TestLevel : Node
{
    ColorRect fadeTransition;
    AnimationPlayer fadeAnimation;

    private WaveManager waveManager;
    private Timer waveTimer;

    private List<Enemy> activeEnemies = new List<Enemy>();

    public override void _Ready()
    {
        fadeTransition = GetNode<ColorRect>("FadeTransition");
        fadeAnimation = GetNode<AnimationPlayer>("FadeTransition/AnimationPlayer");
        fadeTransition.Show();
        fadeAnimation.Play("fade_out");
        fadeAnimation.AnimationFinished += (StringName animName) =>
        {
            if (animName == "fade_out")
            {
                fadeTransition.Hide();
            }
        };


        waveManager = GetNode<WaveManager>("/root/WaveManager");
        waveManager.CreateEnemy += CreateEnemy;

        waveTimer = GetNode<Timer>("WaveTimer");
        waveTimer.Timeout += _onWaveTimerTimeout;

        // Start the first wave for testing
        waveManager.StartWaves();
    }

    public void _onWaveTimerTimeout()
    {
        waveTimer.Stop();
        if (waveManager.StartNextWave())
        {
            GD.Print("Starting next wave: " + waveManager.CurrentWave);
        }
        else
        {
            GD.Print("No more waves to start.");
        }
    }

    public void CreateTower(Data.TowerType towerEnum, Vector2 position)
    {
        if (Data.towerStats.TryGetValue(towerEnum, out var towerStats) && towerStats.towerScene.Instantiate() is Tower towerInstance)
        {
            towerInstance.GlobalPosition = position;
            GetNode<Node2D>("Towers").AddChild(towerInstance);
            towerInstance.Shoot += _CreateBullet;
        }
    }
    
    public void CreateEnemy(Data.EnemyType enemyEnum)
    {
        if (Data.enemyScenes.TryGetValue(enemyEnum, out var prefab))
        {
            if (prefab.Instantiate() is Enemy enemyInstance)
            {
                PathFollow2D pathFollow = new PathFollow2D();

                enemyInstance.Setup(pathFollow);
                pathFollow.AddChild(enemyInstance);

                GetNode<Path2D>("Path2D").AddChild(pathFollow);

                activeEnemies.Add(enemyInstance);
                enemyInstance.Died += _onEnemyDied;
            }
        }
    }

    private void _onEnemyDied(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }

        if (activeEnemies.Count == 0 && waveManager.IsWaveActive == false)
        {
            if (waveManager.HasMoreWaves())
            {
                GD.Print("All enemies defeated. Starting next wave in 5 seconds.");
                waveTimer.Start(5.0f); // Start a timer to trigger the next wave after 2 seconds
            }
            else
            {
                GD.Print("All enemies defeated. No more waves to start.");
            }
        }
    }
    
    private void _CreateBullet(Vector2 pos, float direction, int bulletEnum)
    {
        if (Data.bulletStats.TryGetValue((Data.BulletType)bulletEnum, out var bulletStats) && bulletStats.bulletScene.Instantiate() is Bullet bulletInstance)
        {
            bulletInstance.Setup(pos + Vector2.FromAngle(direction) * 32, direction, (Data.BulletType)bulletEnum);
            GetNode<Node2D>("Bullets").AddChild(bulletInstance);
        }
    }
}
