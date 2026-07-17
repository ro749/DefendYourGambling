using Godot;
using System;
using System.Collections.Generic;

public partial class WaveTimeController : Button
{
    public Action<float> onMultiplierChanged;
    public Action onWaveStarted;

    private ProgressBar timerBar;

    public bool isWaveActive = false;

    [Export] private float[] multipliers = new float[] { 1f, 2f, 3f };
    [Export] private int multiplierIndex = 0;
    public float multiplier => multipliers[multiplierIndex];

    public override void _Ready()
    {
        timerBar = GetNode<ProgressBar>("TimerBar");
        Text = "Start";
    }

    public override void _Process(double delta)
    {
        if (!isWaveActive)
        {
            timerBar.Value = GameState.WaveTimer;
            timerBar.MaxValue = GameState.TimeBetweenWaves;
        }
        else
        {
            timerBar.Value = 0f;
            timerBar.MaxValue = 1f;
        }
    }

    private void _onPressed()
    {
        GD.Print("WaveTimeController: Button pressed. isWaveActive = " + isWaveActive);
        if (isWaveActive)
        {
            ChangeMultiplier();
        }
        else
        {
            onWaveStarted?.Invoke();
        }
    }

    public void SetWaveActive(bool active)
    {
        isWaveActive = active;
        if (isWaveActive)
        {
            timerBar.Hide();
            Text = $"x{multiplier}";
        }
        else
        {
            timerBar.Show();
            Text = "Start";
        }
    }
    
    private void ChangeMultiplier()
    {
        multiplierIndex = (multiplierIndex + 1) % multipliers.Length;
        Text = $"x{multiplier}";
        onMultiplierChanged?.Invoke(multiplier);
    }
}
