using Godot;
using System;

public partial class GameInterface : Control
{
    private Label _lifesLable;
    private Label _moneyLable;
    private Label _waveLable;

    public override void _Ready()
    {
        _lifesLable = GetNode<Label>("LifesLabel");
        _moneyLable = GetNode<Label>("MoneyLabel");
        _waveLable = GetNode<Label>("WaveLabel");
    }

    public void ChangeMoneyText(int money)
    {
        _moneyLable.Text = Mathf.Max(money, 0).ToString();
    }

    public void ChangeLifesText(int lifes)
    {
        _lifesLable.Text = Mathf.Max(lifes, 0).ToString();
    }

    public void ChangeWaveText(int currentWave, int? maxWave = -1)
    {
        if (maxWave != null)
        {
            _waveLable.Text = $"{Mathf.Max(currentWave, 0)}/{maxWave}";
        }
        else
        {
            _waveLable.Text = Mathf.Max(currentWave, 0).ToString();
        }
    }
}
