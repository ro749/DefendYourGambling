using Godot;
using System;
using System.Collections.Generic;

#nullable enable
[GlobalClass]
public partial class Tower : Area2D
{
    [Signal]
    public delegate void ShootEventHandler(Vector2 pos, float direction, int bulletEnum);

    public Action<Tower>? Clicked;
    public bool Selected;

    private Node2D? DetectionVisibleArea;

    private Node2D? turretNode;
    
    protected List<Enemy> _nearbyEnemies = new List<Enemy>();

    [Export]
    public Data.BulletType bulletType = Data.BulletType.SINGLE;
    
    [Export]
    public Data.TowerSeekFnc towerSeekFunction = Data.TowerSeekFnc.FURTHEST;
    
    [Export]
    public Data.TowerType towerType = Data.TowerType.NONE;

    [Export] public bool debug = false;

    public override void _Ready()
    {
        turretNode = GetNode<Node2D>("Turret");
    }
    
    public override void _Process(double delta)
    {
        if (_nearbyEnemies.Count == 0) return;
        Enemy? selectedEnemy = Data.SelectEnemy(_nearbyEnemies, towerSeekFunction);
        if (selectedEnemy == null) return;
        turretNode?.LookAt(selectedEnemy.GlobalPosition);

        DetectionVisibleArea = GetNode<Node2D>("EnemyDetectionArea/VisibleArea");
    }
    
    private void _OnReloadTimerTimeout()
    {
        if (_nearbyEnemies.Count == 0) return;
        EmitSignal("Shoot", GlobalPosition, turretNode?.Rotation?? 0.0f, (int)bulletType);
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
                Clicked?.Invoke(this);
            }
        }
    }

    public void SetSelected(bool selected)
    {
        Selected = selected;

        if (selected) DetectionVisibleArea?.Show();
        else DetectionVisibleArea?.Hide();
    }
}
