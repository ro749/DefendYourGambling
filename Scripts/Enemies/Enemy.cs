using Godot;
using System;
using System.Collections.Generic;

#nullable enable
public partial class Enemy : Area2D
{
    public Action<Enemy>? Died;
    public Action<Enemy>? Damaged;
    public Action<Enemy, int>? DoDamageToPlayer;

    [Export]
    public Data.EnemyType Type { get; private set; } = Data.EnemyType.BASIC;


    public PathFollow2D? PathFollow{ get; private set; }

    [Export]
    private double _maxHealth = 50.0;
    public double Health { get; private set; } = 50.0;
    
    [Export]
    private double _moveSpeed = 100.0;

    [Export]
    private int _damage = 1;

    [Export]
    public int Reward { get; private set; } = 10;
    
    private float _lastProgressRatio = 0.0f;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Health = _maxHealth;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        if (PathFollow == null) return;

        PathFollow.Progress += (float)(_moveSpeed * delta);
        if (PathFollow.ProgressRatio < _lastProgressRatio)  
        {
            DoDamageToPlayer?.Invoke(this, _damage);
            QueueFree();
        }
        _lastProgressRatio = PathFollow.ProgressRatio;
    }
    
    public void Setup(PathFollow2D newPathFollow, double maxHealthMultiply = 1.0)
    {
        PathFollow = newPathFollow;
        Health = _maxHealth * maxHealthMultiply;
    }

    public void TakeDamage(double damage)
    {
        Health -= damage;
        Damaged?.Invoke(this);
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (!GodotObject.IsInstanceValid(this)) return;

        Died?.Invoke(this);
        QueueFree();
    }
}
