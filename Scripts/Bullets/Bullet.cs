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
    public int MaxPenetration { get; private set; } = 1;
    public Data.BulletType bulletEnum { get; private set; }

    [Export] private ShapeCast2D _shapeCast = null!;

    private readonly HashSet<ulong> _damagedEnemyIds = new();

    private bool _isFinishing;

    private float _lifetime = 2.0f;
    private float _currentLifetime = 0.0f;

    public List<Enemy> EnemiesHit { get; private set; } = new List<Enemy>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // We update it manually before moving.
        _shapeCast.Enabled = false;
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

    // TODO: USE A ShapeCast2D for better performance and accuracy
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        if (_isFinishing)
            return;

        Vector2 motion = _direction * _speed * (float)delta;

        if (motion.IsZeroApprox())
            return;

        ProcessMovement(motion);
    }
    private void ProcessMovement(Vector2 motion)
    {
        _shapeCast.ClearExceptions();

        /*
         * TargetPosition is local to ShapeCast2D.
         *
         * Transform the bullet's global motion into the ShapeCast2D's
         * local coordinate system. This remains correct if the bullet
         * or ShapeCast2D is rotated.
         */
        Vector2 globalDestination = GlobalPosition + motion;
        _shapeCast.TargetPosition = _shapeCast.ToLocal(globalDestination);

        int remainingHits = MaxPenetration;

        while (remainingHits > 0)
        {
            _shapeCast.ForceShapecastUpdate();

            if (!_shapeCast.IsColliding())
                break;

            ShapeHit? nearestHit = FindNearestValidHit();

            if (nearestHit is null)
                break;

            ShapeHit hit = nearestHit.Value;

            hit.Enemy.TakeDamage(Damage);

            _damagedEnemyIds.Add(hit.Enemy.GetInstanceId());
            remainingHits--;

            /*
             * Exclude it before repeating the cast. The next update will
             * therefore report the next enemy along the same movement.
             */
            _shapeCast.AddException(hit.Collider);

            if (remainingHits == 0)
            {
                FinishAtImpact(hit.Point);
                return;
            }
        }

        GlobalPosition += motion;
    }

    private ShapeHit? FindNearestValidHit()
    {
        ShapeHit? nearest = null;
        float nearestDistanceSquared = float.PositiveInfinity;

        Vector2 castStart = _shapeCast.GlobalPosition;

        for (int i = 0; i < _shapeCast.GetCollisionCount(); i++)
        {
            GodotObject colliderObject = _shapeCast.GetCollider(i);

            if (colliderObject is not Area2D collider)
                continue;

            if (collider is not Enemy enemy)
                continue;

            if (enemy is null || !GodotObject.IsInstanceValid(enemy)) // No enemy or enemy is already dead
                continue;

            if (_damagedEnemyIds.Contains(enemy.GetInstanceId())) // Already damaged this enemy
                continue;

            Vector2 collisionPoint = _shapeCast.GetCollisionPoint(i);
            float distanceSquared =
                castStart.DistanceSquaredTo(collisionPoint);

            if (distanceSquared >= nearestDistanceSquared)
                continue;

            nearestDistanceSquared = distanceSquared;

            nearest = new ShapeHit(
                enemy,
                collider,
                collisionPoint
            );
        }

        return nearest;
    }

    private void FinishAtImpact(Vector2 collisionPoint)
    {
        _isFinishing = true;

        /*
         * GetCollisionPoint() is in global coordinates, so this assignment
         * is appropriate when the bullet's origin represents its front.
         */
        GlobalPosition = collisionPoint;

        /*
         * QueueFree() normally leaves the bullet visible for the rest of
         * this rendered frame. For a deliberate final-impact frame, wait
         * for one process frame before deleting it.
         */
        RemoveAfterVisualFrame();
    }

    private async void RemoveAfterVisualFrame()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        if (GodotObject.IsInstanceValid(this))
            QueueFree();
    }

    private readonly record struct ShapeHit(
        Enemy Enemy,
        Area2D Collider,
        Vector2 Point
    );
}
