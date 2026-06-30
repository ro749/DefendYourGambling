using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
//fixed
public partial class SpinToWin : TextureButton
{
    public enum TowerState
    {
        DESTROY_ENEMIES_1 = 125, // 5^3
        DESTROY_ENEMIES_2 = 625, // 5^4
        DESTROY_ENEMIES_3 = 3125, // 5^5

        WIN_MONEY_1 = 343, // 7^3
        WIN_MONEY_2 = 2401, // 7^4
        WIN_MONEY_3 = 16807, // 7^5

        MORE_ENEMIES_1 = 1331, // 11^3
        MORE_ENEMIES_2 = 14641, // 11^4
        MORE_ENEMIES_3 = 161051 // 11^5
    }

    AudioManager audioManager;

    [Export] public BoxContainer slotMachine;
    [Export] public float SecondsBetweenRolls = 1.0f;
    [Export] public int moneyCost = 50;

    public Action<int> KillEnemies;
    public Action<int> MoneyReward;
    public Action<int> EnemyPenalty;

    [Export] private AudioSetting _startSound;
    [Export] private AudioSetting[] _moneyRewardSounds = [];
    [Export] private AudioSetting[] _moreEnemySounds = [];
    [Export] private AudioSetting[] _killEnemySounds = [];


    private Func<int, bool> _tryBuy;

    [Export]
    private int RerollTimes = 0;

    public override void _Ready()
    {
        audioManager = GetNode<AudioManager>("/root/AudioManager");
    }

    public void Setup(Func<int, bool> canBuy)
    {
        _tryBuy = canBuy;
    }

    private async Task<int> Roll(bool seeRoll)
    {
        var v = 1;
        foreach (Reel child in slotMachine.GetChildren().OfType<Reel>()) {
            child.HideBackgroundColor();
            v *= child.Roll(seeRoll);
            if (!seeRoll) continue;
            await ToSignal(GetTree().CreateTimer(SecondsBetweenRolls), "timeout");
        }
        return v;
    }

    public async void _on_pressed()
    {
        if (_tryBuy == null || !_tryBuy(moneyCost))
            return;
            
        audioManager.PlaySound(_startSound);

        var v = await Roll(true);

        if (RerollTimes > 0)
        {
            while(v % 2 != 0)
            {
                v = await Roll(false);
            }
            --RerollTimes;
        }
        foreach (Reel child in slotMachine.GetChildren().OfType<Reel>()) {
            child.canStop = true;
            await ToSignal(child, "Stoped");
        }

        if (v % (int)TowerState.DESTROY_ENEMIES_1 == 0)
        {
            int rewardIndex = v % (int)TowerState.DESTROY_ENEMIES_3 == 0 ? 2 : (v % (int)TowerState.DESTROY_ENEMIES_2 == 0 ? 1 : 0);
            if (rewardIndex < _killEnemySounds.Length)
            {
                int soundIndex = Mathf.Min(rewardIndex, _killEnemySounds.Length - 1);
                audioManager.PlaySound(_killEnemySounds[soundIndex]);
            }
            KillEnemies?.Invoke(rewardIndex + 1);
        }
                
        if (v % (int)TowerState.WIN_MONEY_1 == 0)
        {
            int rewardIndex = v % (int)TowerState.WIN_MONEY_3 == 0 ? 2 : (v % (int)TowerState.WIN_MONEY_2 == 0 ? 1 : 0);
            audioManager.PlaySound(_moneyRewardSounds[rewardIndex]);
            if (rewardIndex < _moneyRewardSounds.Length)
            {
                int soundIndex = Mathf.Min(rewardIndex, _moneyRewardSounds.Length - 1);
                audioManager.PlaySound(_moneyRewardSounds[soundIndex]);
            }
            MoneyReward?.Invoke(rewardIndex == 2 ? 5 * moneyCost : (rewardIndex == 1 ? 3 * moneyCost / 2 : moneyCost / 2));
        }

        if (v % (int)TowerState.MORE_ENEMIES_1 == 0)
        {
            int rewardIndex = v % (int)TowerState.MORE_ENEMIES_3 == 0 ? 2 : (v % (int)TowerState.MORE_ENEMIES_2 == 0 ? 1 : 0);
            if (_moreEnemySounds.Length > 0)
            {
                int soundIndex = Mathf.Min(rewardIndex, _moreEnemySounds.Length - 1);
                audioManager.PlaySound(_moreEnemySounds[soundIndex]);
            }
            EnemyPenalty?.Invoke(rewardIndex);
        }
    }
}
