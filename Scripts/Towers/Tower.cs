using Godot;
using System;
using System.Collections.Generic;

#nullable enable
[GlobalClass]
public partial class Tower : Area2D
{
    [Signal]
    public delegate void ShootEventHandler(Vector2 pos, float direction, float speed, int bulletEnum);

    public Action<Tower>? OnClicked;
    public Action<Tower>? OnSelled;
    public bool Selected;

    private Node2D? DetectionVisibleArea;
    private ColorRect? SelectedMenu;

    private Node2D? turretNode;
    private Timer? _reloadTimer;
    
    protected List<Enemy> _nearbyEnemies = new List<Enemy>();

    [Export]
    public Data.BulletType bulletType = Data.BulletType.SINGLE;
    [Export]
    public int bulletsCount = 1;
    [Export]
    public float bulletsAngle = 0.0f;
    [Export]
    public float bulletsAngleOffset = 0.0f;
    [Export]
    public float bulletSpeed = 100.0f;
    [Export]
    public float shootSpeed = 1.0f;
    
    [Export]
    public Data.TowerSeekFnc towerSeekFunction = Data.TowerSeekFnc.FURTHEST;
    
    [Export]
    public Data.TowerType towerType = Data.TowerType.NONE;
    
    [Export]
    public int SellValue { get; private set; } = 150;

    [Export] public bool debug = false;

    public override void _Ready()
    {
        turretNode = GetNode<Node2D>("Turret");
        SelectedMenu = GetNode<ColorRect>("SelectedMenu");
        DetectionVisibleArea = GetNode<Node2D>("EnemyDetectionArea/VisibleArea");

        _reloadTimer = GetNode<Timer>("ReloadTimer");
        _reloadTimer.Timeout += _OnReloadTimerTimeout;
    }
    
    public override void _Process(double delta)
    {
        if (_nearbyEnemies.Count == 0) return;
        if (towerSeekFunction != Data.TowerSeekFnc.NONE)
        {
            Enemy? selectedEnemy = Data.SelectEnemy(_nearbyEnemies, towerSeekFunction);
            if (selectedEnemy == null) return;
            turretNode?.LookAt(selectedEnemy.GlobalPosition);
        }

        if (!(_reloadTimer?.IsStopped()?? false)) return;
        for (int i = 0; i < bulletsCount; ++i)
        {
            float angleOffset = ((1 - bulletsCount) / 2.0f + i) * bulletsAngle + bulletsAngleOffset;
            angleOffset = angleOffset * Mathf.Pi / 180.0f;
            EmitSignal("Shoot", GlobalPosition, (turretNode?.Rotation?? 0.0f) + angleOffset, bulletSpeed, (int)bulletType);
        }
        _reloadTimer.Start(shootSpeed);
    }
    
    private void _OnReloadTimerTimeout()
    {
        _reloadTimer?.Stop();
    }

    private void _onEnemyDetectionAreaAreaEntered(Area2D area)
    {
        if (area is not Enemy enemy || _nearbyEnemies.Contains(enemy)) return;
        enemy.Died += _onEnemyDied;
        _nearbyEnemies.Add(enemy);
    }
    
    private void _onEnemyDetectionAreaAreaExited(Area2D area)
    {
        if (area is not Enemy enemy || !_nearbyEnemies.Contains(enemy)) return;
        enemy.Died -= _onEnemyDied;
        _nearbyEnemies.Remove(enemy);
    }

    private void _onEnemyDied(Enemy enemy)
    {
        if (_nearbyEnemies.Contains(enemy))
        {
            _nearbyEnemies.Remove(enemy);
        }
    }

    private void _onInputEvent(Node viewport, InputEvent inputEvent, int shapeIndex)
    {
        if (inputEvent is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
            {
                OnClicked?.Invoke(this);
            }
        }
    }

    public void SetSelected(bool selected)
    {
        Selected = selected;

        if (selected) {
            DetectionVisibleArea?.Show();
            SelectedMenu?.Show();
        }
        else {
            DetectionVisibleArea?.Hide();
            SelectedMenu?.Hide();
        }
    }

    private void _onSellButtonPressed()
    {
        OnSelled?.Invoke(this);
    }
}