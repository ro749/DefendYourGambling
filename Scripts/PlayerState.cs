using Godot;
using System;

#nullable enable
public partial class PlayerState : Node
{
    public int Gold { get; private set; } = 100;
    public int Lives { get; private set; } = 20;
    public int CurrentWave { get; private set; } = 0;

    public event Action<int>? GoldChanged;
    public event Action<int>? LivesChanged;
    public event Action<int>? WaveChanged;

    public bool SpendGold(int amount)
    {
        if (Gold < amount)
            return false;

        Gold -= amount;
        GoldChanged?.Invoke(Gold);
        return true;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        GoldChanged?.Invoke(Gold);
    }

    public void LoseLife(int amount = 1)
    {
        Lives -= amount;
        LivesChanged?.Invoke(Lives);

        GD.Print($"Player lost a life. Remaining lives: {Lives}"); // Debug print

        if (Lives <= 0)
        {
            GD.Print("Game Over");
        }
    }

    public void StartNextWave()
    {
        CurrentWave++;
        WaveChanged?.Invoke(CurrentWave);
    }

    public void Reset()
    {
        Gold = 100;
        Lives = 20;
        CurrentWave = 0;

        GoldChanged?.Invoke(Gold);
        LivesChanged?.Invoke(Lives);
        WaveChanged?.Invoke(CurrentWave);
    }
}
