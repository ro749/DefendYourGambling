using Godot;
using System;
using System.Linq;
public partial class SpinToWin : Button
{
    public enum TowerState
    {
        DESTROY_ENEMIES = 27,
        WIN_MONEY = 125,
        MORE_ENEMIES = 343
    }
    [Export] public BoxContainer slotMachine;

    private bool IsFirstRoll = true;
    private PlayerState playerState;
    public override void _Ready()
    {
        playerState = GetTree().Root.GetNode<PlayerState>("Map/PlayerState");
    }
    public void _on_pressed()
    {
        if (playerState.SpendMoney(167) == false)
            return;
        var v = 1;
        foreach (Reel child in slotMachine.GetChildren().OfType<Reel>()) {
            v *= child.roll();
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
        switch (v)
        {
            case (int)TowerState.DESTROY_ENEMIES:
                foreach (Node2D child in GetTree().Root.GetNode<Node2D>("Map/Path2D").GetChildren())
                {
                    child.QueueFree();
                }
                break;
            case (int)TowerState.WIN_MONEY:
                playerState.AddMoney(1000);
                break;
            case (int)TowerState.MORE_ENEMIES:
                GD.Print("More Enemies");
                break;
        }
    }
}
