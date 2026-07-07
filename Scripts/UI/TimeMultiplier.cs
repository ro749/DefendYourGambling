using Godot;
using System;
using System.Collections.Generic;

public partial class TimeMultiplier : Button
{
    public Action<float> onMultiplierChanged;

    [Export] private float[] multipliers = new float[] { 1f, 2f, 3f };
    [Export] private int multiplierIndex = 0;
    public float multiplier => multipliers[multiplierIndex];

    private void _onPressed()
    {
        multiplierIndex = (multiplierIndex + 1) % multipliers.Length;
        Text = $"x{multiplier}";
        onMultiplierChanged?.Invoke(multiplier);
    }
}
