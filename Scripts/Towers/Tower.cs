using Godot;
using System;
using System.Collections.Generic;

#nullable enable
[GlobalClass]
public partial class Tower : Area2D
{
    [Signal]
    public delegate void ShootEventHandler(Vector2 pos, float direction, float speed, float damage, int maxPenetration, int bulletEnum);

    public Action<Tower>? OnClicked;
    public Action<Tower>? OnSelled;
    public bool Selected;

    private Node2D? DetectionVisibleArea;
    private ColorRect? SelectedMenu;

    private Node2D? turretNode;
    private float _reloadTimer = 0.0f;
    
    protected List<Enemy> _nearbyEnemies = new List<Enemy>();

    [Export]
    public Data.BulletType bulletType = Data.BulletType.BASIC;
    [Export]
    public int bulletsCount = 1;
    [Export]
    public float bulletsAngle = 0.0f;
    [Export]
    public float bulletsAngleOffset = 0.0f;
    [Export]
    public float shootSpeed = 1.0f;
    [Export]
    public float bulletSpeed = 100.0f;
    [Export]
    public float bulletDamage = 20.0f;
    [Export]
    public int bulletMaxPenetration = 1;
    
    [Export]
    public Data.TowerSeekFnc towerSeekFunction = Data.TowerSeekFnc.FURTHEST;
    
    [Export]
    public Data.TowerType towerType = Data.TowerType.NONE;
    
    [Export]
    public int SellValue { get; private set; } = 150;

    [Export] public bool debug = false;

    Vector2 draggedInitialPos;
    Vector2 draggedInitialMousePos;
    private bool startedDrag = false;
    private bool dragged = false;


    public override void _Ready()
    {
        turretNode = GetNode<Node2D>("Turret");
        SelectedMenu = GetNode<ColorRect>("SelectedMenu");
        DetectionVisibleArea = GetNode<Node2D>("EnemyDetectionArea/VisibleArea");
    }

    public override void _Process(double delta)
    {
        if (startedDrag)
        {
            float movedDistance = (GetGlobalMousePosition() - draggedInitialMousePos).Length();
            if (movedDistance > 15)
            {
                dragged = true;
                startedDrag = false;
                draggedInitialPos = Position;
                if (Selected)
                {
                    OnClicked?.Invoke(this);
                }
            }
        }
        if (dragged)
        {
            Position = GetGlobalMousePosition();
            return;
        }

        if (_reloadTimer <= 0.0f && _nearbyEnemies.Count > 0)
        {
            _seekEnemy();
            _shootBullets();
        }
        else
        {
            _reloadTimer -= (float)delta * GameState.WaveTimeMultiplier;
        }
    }

    private void _seekEnemy()
    {
        if (towerSeekFunction != Data.TowerSeekFnc.NONE)
        {
            Enemy? selectedEnemy = Data.SelectEnemy(_nearbyEnemies, towerSeekFunction);
            if (selectedEnemy == null) return;
            turretNode?.LookAt(selectedEnemy.GlobalPosition);
        }
    }

    private void _shootBullets()
    {
        for (int i = 0; i < bulletsCount; ++i)
        {
            float angleOffset = ((1 - bulletsCount) / 2.0f + i) * bulletsAngle + bulletsAngleOffset;
            angleOffset = angleOffset * Mathf.Pi / 180.0f;
            EmitSignal("Shoot", GlobalPosition, (turretNode?.Rotation?? 0.0f) + angleOffset, bulletSpeed, bulletDamage, bulletMaxPenetration, (int)bulletType);
        }
        _reloadTimer = shootSpeed;
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
                //startedDrag = true;
                draggedInitialMousePos = GetGlobalMousePosition();
            }
            if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.IsReleased())
            {
                startedDrag = false;
                if (dragged)
                {
                    if (GetOverlappingAreas().Count > 0)
                    {
                        Position = draggedInitialPos;
                    }
                    dragged = false;
                }
                else
                {
                    OnClicked?.Invoke(this);
                }
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