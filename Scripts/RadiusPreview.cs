using Godot;
using System;

public partial class RadiusPreview : Node2D
{
    [Export] public float Radius { get; set; } = 100f;

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, Radius, new Color(1, 1, 1, 0.2f));
    }
}
