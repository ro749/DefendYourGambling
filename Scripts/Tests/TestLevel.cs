using Godot;
using System.Linq;
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

    AudioManager audioManager;
    [Export] private AudioSetting _victorySound;

    private SpinToWin spinToWinMachineBasic;
    private SpinToWin spinToWinMachineUpgrade;

    private bool _wavesStarted = false;
    private WaveManager waveManager;
    private Timer waveTimer;

    private TimeMultiplier timeMultiplier;

    private List<Enemy> activeEnemies = new List<Enemy>();

    public PreviewTower towerPreview = null;

    [Export]
    private int _initialMoney = 90;
    [Export]
    private float _maxMoneyMultiply = 0.3f;
    private float _moneyMultiply = 1.0f;
    [Export]
    private double _maxEnemyLifeMultiply = 1.8;
    private double _enemyLifeMultiply = 1.0;

    public override void _Ready()
    {
        audioManager = GetNode<AudioManager>("/root/AudioManager");

        fadeTransition = GetNode<ColorRect>("CanvasLayer/FadeTransition");
        fadeAnimation = GetNode<AnimationPlayer>("CanvasLayer/FadeTransition/AnimationPlayer");
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


        playerState = GetNode<PlayerState>("PlayerState");
        playerState.LivesChanged += (int currentLives) =>
        {
            gameInterface.ChangeLifesText(currentLives);
            if (currentLives <= 0)
            {
                GetTree().Paused = true;
                Engine.TimeScale = 1.0;
                loseScreen.Show();

                if (towerPreview != null)
                {
                    towerPreview.Cancel();
                }
            }
        };
        playerState.MoneyChanged += (int currentMoney) =>
        {
            gameInterface.ChangeMoneyText(currentMoney);
        };
        gameInterface.ChangeLifesText(playerState.Lives);
        playerState.SetMoney(_initialMoney);


        waveManager = GetNode<WaveManager>("WaveManager");
        waveManager.CreateEnemy += CreateEnemy;
        waveManager.WaveChanged += (int currentWave) =>
        {
            gameInterface.ChangeWaveText(currentWave + 1, waveManager.TotalWaves);

            float alpha = currentWave / (float)(waveManager.TotalWaves - 1);
            _moneyMultiply = Mathf.Lerp(1.0f, _maxMoneyMultiply, alpha);
            _enemyLifeMultiply = Mathf.Lerp(1.0f, _maxEnemyLifeMultiply, alpha);
        };
        gameInterface.ChangeWaveText(1, waveManager.TotalWaves);

        waveTimer = GetNode<Timer>("WaveTimer");
        waveTimer.Timeout += _onWaveTimerTimeout;


        spinToWinMachineBasic = GetNode<SpinToWin>("CanvasLayer/TowerSlotMachineUpgrade/TextureButton");
        spinToWinMachineBasic.Setup((money) => { return playerState.SpendMoney(money); });
        spinToWinMachineBasic.EnemyPenalty += waveManager.AddEnemiesPenalty;
        spinToWinMachineBasic.MoneyReward += playerState.AddMoney;
        spinToWinMachineBasic.KillEnemies += OnKillEnemiesReward;

        spinToWinMachineUpgrade = GetNode<SpinToWin>("CanvasLayer/TowerSlotMachine/TextureButton");
        spinToWinMachineUpgrade.Setup((money) => { return playerState.SpendMoney(money); });
        spinToWinMachineUpgrade.EnemyPenalty += waveManager.AddEnemiesPenalty;
        spinToWinMachineUpgrade.MoneyReward += playerState.AddMoney;
        spinToWinMachineUpgrade.KillEnemies += OnKillEnemiesReward;

        // Start the first wave for testing
        waveTimer.Start(6.0f);

        timeMultiplier = GetNode<TimeMultiplier>("CanvasLayer/TimeMultiplier");
        timeMultiplier.onMultiplierChanged += (float multiplier) =>
        {
            Engine.TimeScale = multiplier;
        };
    }
    
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("pause") && GetTree().Paused)
        {
            GetTree().Paused = false;
            Engine.TimeScale = timeMultiplier.multiplier;
            pauseScreen.Hide();
        }
        else if (Input.IsActionJustPressed("pause") && !GetTree().Paused)
        {
            GetTree().Paused = true;
            Engine.TimeScale = 1.0;
            pauseScreen.Show();

            if (towerPreview != null)
            {
                towerPreview.Cancel();
            }
        }
        
    }

    public override void _ExitTree()
    {
        pauseScreen.OnResume -= OnResume;
        waveManager.CreateEnemy -= CreateEnemy;
        waveTimer.Timeout -= _onWaveTimerTimeout;
        Engine.TimeScale = 1.0;
    }

    private void OnResume()
    {
        GetTree().Paused = false;
        pauseScreen.Hide();
        Engine.TimeScale = timeMultiplier.multiplier;
    }

    public void _onWaveTimerTimeout()
    {
        waveTimer.Stop();
        if (!_wavesStarted)
        {
            _wavesStarted = true;
            waveManager.StartWaves();
            return;
        }

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
            towerInstance.OnClicked += _TowerSelected;
            towerInstance.OnSelled += _TowerSelled;
        }
    }

    private void _TowerSelected(Tower tower)
    {
        Node2D towersParent = GetNode<Node2D>("Towers");
        Godot.Collections.Array<Node> foundNodes = towersParent.GetChildren();
        foreach (Node node in foundNodes)
        {
            if (node is Tower towerNode && towerNode != tower)
            {
                towerNode.SetSelected(false);
            }
        }
        tower.SetSelected(!tower.Selected);
    }

    private void _TowerSelled(Tower tower)
    {
        playerState.AddMoney(tower.SellValue);
        tower.QueueFree();
    }

    public void CreateEnemy(Data.EnemyType enemyEnum)
    {
        if (Data.enemyScenes.TryGetValue(enemyEnum, out var prefab))
        {
            if (prefab.Instantiate() is Enemy enemyInstance)
            {
                PathFollow2D pathFollow = new PathFollow2D();

                enemyInstance.Setup(pathFollow, _enemyLifeMultiply);
                pathFollow.AddChild(enemyInstance);
                pathFollow.Rotates = false;

                GetNode<Path2D>("Path2D").AddChild(pathFollow);

                activeEnemies.Add(enemyInstance);
                enemyInstance.Died += _onEnemyDied;
                enemyInstance.Damaged += _onEnemyDamaged;
                enemyInstance.DoDamageToPlayer += EnemyDamagePlayer;
            }
        }
    }

    private void _onEnemyDied(Enemy enemy)
    {
        audioManager.PlayNormalEnemyDeadSound();
        
        playerState.AddMoney(Mathf.Max(Mathf.RoundToInt(enemy.Reward * _moneyMultiply), 1));

        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }

        CheckForNewWave();
    }

    private void _onEnemyDamaged(Enemy enemy)
    {
        audioManager.PlayNormalEnemyDamageSound();
    }
    
    private void _CreateBullet(Vector2 pos, float direction, float speed, float damage, int maxPenetration, int bulletEnum)
    {
        if (Data.bulletStats.TryGetValue((Data.BulletType)bulletEnum, out var bulletStats) && bulletStats.bulletScene.Instantiate() is Bullet bulletInstance)
        {
            bulletInstance.Setup(pos + Vector2.FromAngle(direction) * 32, direction, speed, damage, maxPenetration, (Data.BulletType)bulletEnum);
            GetNode<Node2D>("Bullets").AddChild(bulletInstance);
        }
    }

    private void EnemyDamagePlayer(Enemy enemy, int damage)
    {
        DamagePlayer(damage);

        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }

        if (playerState.Lives <= 0) return;

        CheckForNewWave();
    }
    private void DamagePlayer(int damage)
    {
        playerState.LoseLife(damage);
    }

    private void CheckForNewWave()
    {
        if (activeEnemies.Count == 0 && waveManager.IsWaveActive == false)
        {
            if (waveManager.HasMoreWaves())
            {
                waveTimer.Start(5.0f); // Start a timer to trigger the next wave after 2 seconds
            }
            else
            {
                victoryScreen.Show();
                Engine.TimeScale = 1.0;
                audioManager.PlaySound(_victorySound);

                if (towerPreview != null)
                {
                    towerPreview.Cancel();
                }

                GetTree().Paused = true;
            }
        }
    }


    private void OnKillEnemiesReward(int tier)
    {
        GD.Print($"Destroying enemies tier {tier}");
        List<Enemy> cloneActiveEnemies = [];
        foreach (Enemy child in activeEnemies)
        {
            if (child.Type == Data.EnemyType.BOSS) continue;

            cloneActiveEnemies.Add(child);
        }

        if (tier == 3)
        {
            foreach (Enemy child in cloneActiveEnemies)
            {
                _onEnemyDied(child);
                child.QueueFree();
            }
            return;
        }

        int enemyCount = cloneActiveEnemies.Count;
        int killEnemyCount = tier == 1 ? enemyCount / 5 : enemyCount / 2;
        killEnemyCount = tier == 1 ? Mathf.Max(killEnemyCount, 3) : Mathf.Max(killEnemyCount, 10);
        GD.Print($"Kill enemy count {killEnemyCount}");

        List<int> enemyIndices = Enumerable.Range(0, enemyCount).ToList();

        while (killEnemyCount > 0 && enemyIndices.Count > 0)
        {
            var v = (int)(GD.Randi() % enemyIndices.Count);
            var e = enemyIndices[v];
            
            Enemy child = cloneActiveEnemies[e];
            _onEnemyDied(child);
            child.QueueFree();

            enemyIndices.RemoveAt(v);
            --killEnemyCount;
        }
    }
}