using Godot;
using System;

#nullable enable
public partial class PlayerState : Node
{
    public int Money { get; private set; } = 100;
    public int Lives { get; private set; } = 20;

    public event Action<int>? MoneyChanged;
    public event Action<int>? LivesChanged;

    public bool SpendMoney(int amount)
    {
        if (Money < amount)
            return false;

        Money -= amount;
        MoneyChanged?.Invoke(Money);
        return true;
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        MoneyChanged?.Invoke(Money);
    }

    public void SetMoney(int amount)
    {
        Money = amount;
        MoneyChanged?.Invoke(Money);
    }

    public void LoseLife(int amount = 1)
    {
        Lives -= amount;
        LivesChanged?.Invoke(Lives);
    }

    public void Reset()
    {
        Money = 100;
        Lives = 20;

        MoneyChanged?.Invoke(Money);
        LivesChanged?.Invoke(Lives);
    }
}
