using Godot;
using System;
using System.Linq;
public partial class SpinToWin : Button
{
    [Export] public BoxContainer slotMachine;
    public void _on_pressed()
    {
        foreach (Reel child in slotMachine.GetChildren().OfType<Reel>()) {
            child.roll();
        }
        
    }
}
