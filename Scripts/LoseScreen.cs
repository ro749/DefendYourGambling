using Godot;
using System;

public partial class LoseScreen : CenterContainer
{
    private void _onMainMenuPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://Scenes/Maps/main_menu.tscn");
    }
}
