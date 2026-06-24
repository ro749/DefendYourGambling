using Godot;
using System;

public partial class VictoryScreen : CenterContainer
{
    private void _onInfiniteModePressed()
    {
        // Handle infinite mode logic here
    }

    private void _onMainMenuPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://Scenes/Maps/main_menu.tscn");
    }
}
