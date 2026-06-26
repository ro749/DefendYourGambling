using Godot;
using System;
using System.Collections.Generic;

#nullable enable
public partial class Enemy : Area2D
{
    [Signal]
    public delegate void DiedEventHandler(Enemy enemy);

    public Action<Enemy, int>? DoDamageToPlayer;

    private RandomNumberGenerator _rng = new();


    public PathFollow2D? PathFollow{ get; private set; }
    private AudioStreamPlayer2D? _audioPlayer;
    [Export]
    private AudioStream[] _damageSounds = [];
    [Export]
    private AudioStream? _deadSound;

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
        _rng.Randomize();
        Health = _maxHealth;
        _audioPlayer = GetNode<AudioStreamPlayer2D>("AudioPlayer");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
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
    
    public void Setup(PathFollow2D newPathFollow)
    {
        PathFollow = newPathFollow;
        Health = _maxHealth;
    }

    public void _OnAreaEntered(Area2D area)
    {
        if (area is Bullet bullet)
        {
            if (Health <= 0 || bullet.EnemiesHit.Contains(this) || bullet.EnemiesHit.Count >= Data.bulletStats[bullet.bulletEnum].maxPenetration)
            {
                return; // This bullet has already hit max enemies, ignore it.
            }
            Health -= Data.bulletStats[bullet.bulletEnum].damage;
            bullet.HitEnemy(this);
            if (Health <= 0)
            {
                Die();
                if (_audioPlayer != null && _deadSound != null)
                {
                    _audioPlayer.Stream = _deadSound;
                    _audioPlayer.Play();
                }
            }
            else if (_audioPlayer != null && _damageSounds != null && _damageSounds.Length > 0)
            {
                _audioPlayer.Stream = _damageSounds[_rng.RandiRange(0, _damageSounds.Length - 1)];
                _audioPlayer.Play();
            }
        }
    }

    public void Die()
    {
        EmitSignal("Died", this);
        QueueFree();
    }
}
