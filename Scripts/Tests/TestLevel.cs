using Godot;
using System.Collections.Generic;

public partial class TestLevel : Node
{
    ColorRect fadeTransition;
    AnimationPlayer fadeAnimation;

    PlayerState playerState;

    VictoryScreen victoryScreen;
    LoseScreen loseScreen;
    PauseScreen pauseScreen;
    GameInterface gameInterface;

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

        victoryScreen = GetNode<VictoryScreen>("CanvasLayer/Menus/VictoryScreen");
        loseScreen = GetNode<LoseScreen>("CanvasLayer/Menus/LoseScreen");
        pauseScreen = GetNode<PauseScreen>("CanvasLayer/Menus/PauseScreen");
        pauseScreen.OnResume += OnResume;
        gameInterface = GetNode<GameInterface>("CanvasLayer/GameInterface");

        waveManager = GetNode<WaveManager>("WaveManager");
        waveManager.CreateEnemy += CreateEnemy;
        waveManager.WaveChanged += (int currentWave) =>
        {
            gameInterface.ChangeWaveText(currentWave + 1, waveManager.TotalWaves);
        };

        waveTimer = GetNode<Timer>("WaveTimer");
        waveTimer.Timeout += _onWaveTimerTimeout;

        playerState = GetNode<PlayerState>("PlayerState");
        playerState.LivesChanged += (int currentLives) =>
        {
            gameInterface.ChangeLifesText(currentLives);
            if (currentLives <= 0)
            {
                GetTree().Paused = true;
                loseScreen.Show();
            }
        };
        playerState.MoneyChanged += (int currentMoney) =>
        {
            gameInterface.ChangeMoneyText(currentMoney);
        };
        gameInterface.ChangeLifesText(playerState.Lives);
        playerState.SetMoney(1000);

        // Start the first wave for testing
        waveManager.StartWaves();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("pause") && GetTree().Paused)
        {
            GetTree().Paused = false;
            pauseScreen.Hide();
        }
        else if (Input.IsActionJustPressed("pause") && !GetTree().Paused)
        {
            GetTree().Paused = true;
            pauseScreen.Show();
        }
    }

    public override void _ExitTree()
    {
        pauseScreen.OnResume -= OnResume;
        waveManager.CreateEnemy -= CreateEnemy;
        waveTimer.Timeout -= _onWaveTimerTimeout;
    }

    private void OnResume()
    {
        GetTree().Paused = false;
        pauseScreen.Hide();
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
            towerInstance.Clicked += _TowerSelected;
        }
    }

    private void _TowerSelected(Tower tower)
    {
        Node2D towersParent = GetNode<Node2D>("/root/Map/Towers");
        Godot.Collections.Array<Node> foundNodes = towersParent.GetChildren();
        foreach (Node node in foundNodes)
        {
            if (node is Tower towerNode)
            {
                towerNode.SetSelected(false);
            }
        }
        tower.SetSelected(!tower.Selected);
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
                pathFollow.Rotates = false;

                GetNode<Path2D>("Path2D").AddChild(pathFollow);

                activeEnemies.Add(enemyInstance);
                enemyInstance.Died += _onEnemyDied;
                enemyInstance.DoDamageToPlayer += DamagePlayer;
            }
        }
    }

    private void _onEnemyDied(Enemy enemy)
    {
        playerState.AddMoney(enemy.Reward);

        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }

        if (activeEnemies.Count == 0 && waveManager.IsWaveActive == false)
        {
            if (waveManager.HasMoreWaves())
            {
                waveTimer.Start(5.0f); // Start a timer to trigger the next wave after 2 seconds
            }
            else
            {
                GetTree().Paused = true;
                victoryScreen.Show();
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

    private void DamagePlayer(int damage)
    {
        playerState.LoseLife(damage);
    }
}