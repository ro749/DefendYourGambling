using Godot;
using System.Collections.Generic;

public partial class Bullet : Area2D
{
    private Vector2 _direction;
    [Export]
    private float _speed = 1000;
    [Export]
    public float Damage { get; private set; } = 20;
    [Export]
    public float MaxPenetration { get; private set; } = 3;
    public Data.BulletType bulletEnum { get; private set; }

    private float _lifetime = 2.0f;
    private float _currentLifetime = 0.0f;

    public List<Enemy> EnemiesHit { get; private set; } = new List<Enemy>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // TODO: USE A ShapeCast2D for better performance and accuracy
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += _direction * _speed * (float)delta;
        _currentLifetime += (float)delta;
        if (_currentLifetime >= _lifetime)
        {
            QueueFree();
        }
    }

    public void Setup(Vector2 pos, float direction, float newSpeed, float newDamage, int newMaxPenetration, Data.BulletType bulletEnum)
    {
        GlobalPosition = pos;
        _direction = Vector2.FromAngle(direction);
        _speed = newSpeed;
        Damage = newDamage;
        MaxPenetration = newMaxPenetration;
        this.bulletEnum = bulletEnum;
    }

    public void HitEnemy(Enemy enemy)
    {
        EnemiesHit.Add(enemy);
        if (EnemiesHit.Count >= MaxPenetration)
        {
            QueueFree();
        }
    }
}
