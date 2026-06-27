using Godot;
using System;
using System.Linq;
//fixed
public partial class SpinToWin : TextureButton
{
    public enum TowerState
    {
        DESTROY_ENEMIES = 27,
        WIN_MONEY = 125,
        MORE_ENEMIES = 343
    }
    [Export] public BoxContainer slotMachine;
    [Export] public float SecondsBetweenRolls = 1.0f;
    private bool IsFirstRoll = true;
    private PlayerState playerState;
    public override void _Ready()
    {
        playerState = GetTree().Root.GetNode<PlayerState>("Map/PlayerState");
    }
    public async void _on_pressed()
    {
        if (playerState.SpendMoney(167) == false)
            return;
        var v = 1;
        foreach (Reel child in slotMachine.GetChildren().OfType<Reel>()) {
            v *= child.roll();
            await ToSignal(GetTree().CreateTimer(SecondsBetweenRolls), "timeout");
        }
        if (IsFirstRoll)
        {
            while(v % 2 != 0)
            {
                v = 1;
                foreach (Reel child in slotMachine.GetChildren().OfType<Reel>())
                {
                    v *= child.roll();
                }
            }
            IsFirstRoll = false;
        }
        if (v % (int)TowerState.DESTROY_ENEMIES == 0)
        {
            foreach (Node2D child in GetTree().Root.GetNode<Node2D>("Map/Path2D").GetChildren())
            {
                child.QueueFree();
            }
        }
                
        if (v % (int)TowerState.WIN_MONEY == 0)
        {
            playerState.AddMoney(1000);
        }
        if (v % (int)TowerState.MORE_ENEMIES == 0)
        {
            GD.Print("More Enemies");
        }
    }
}
