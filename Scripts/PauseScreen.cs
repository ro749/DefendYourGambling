using Godot;
using System;

#nullable enable
public partial class PauseScreen : CenterContainer
{
    public event Action? OnResume;

    private void _onResumePressed()
    {
        OnResume?.Invoke();
    }

    private void _onMainMenuPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://Scenes/Maps/main_menu.tscn");
    }
}
