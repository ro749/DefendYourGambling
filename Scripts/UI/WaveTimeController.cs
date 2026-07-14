using Godot;
using System;
using System.Collections.Generic;

public partial class WaveTimeController : Button
{
    public Action<float> onMultiplierChanged;
    public Action onWaveStarted;

    public bool isWaveActive = false;

    [Export] private float[] multipliers = new float[] { 1f, 2f, 3f };
    [Export] private int multiplierIndex = 0;
    public float multiplier => multipliers[multiplierIndex];

    private void _onPressed()
    {
        if (isWaveActive)
        {
            onWaveStarted?.Invoke();
        }
        else
        {
            ChangeMultiplier();
        }
    }
    
    private void ChangeMultiplier()
    {
        multiplierIndex = (multiplierIndex + 1) % multipliers.Length;
        Text = $"x{multiplier}";
        onMultiplierChanged?.Invoke(multiplier);
    }
}
