using Godot;
using System;

public partial class MainMenu : Control
{
    ColorRect fadeTransition;
    AnimationPlayer fadeAnimation;

    string btnPressed;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        fadeTransition = GetNode<ColorRect>("FadeTransition");
        fadeAnimation = GetNode<AnimationPlayer>("FadeTransition/AnimationPlayer");
        fadeAnimation.AnimationFinished += _onFadeAnimationFinished;
        fadeTransition.Show();
        fadeAnimation.Play("fade_out");
    }

    private void _onStartGamePressed()
    {
        btnPressed = "StartGame";
        fadeTransition.Show();
        fadeAnimation.Play("fade_in");
    }

    private void _onOptionsPressed()
    {
        btnPressed = "Options";
        fadeTransition.Show();
        fadeAnimation.Play("fade_in");
    }

    private void _onCreditsPressed()
    {
        btnPressed = "Credits";
        fadeTransition.Show();
        fadeAnimation.Play("fade_in");
    }

    private void _onCloseGamePressed()
    {
        btnPressed = "CloseGame";
        fadeTransition.Show();
        fadeAnimation.Play("fade_in");
    }

    private void _onFadeAnimationFinished(StringName animName)
    {
        fadeTransition.Hide();
        
        if (animName != "fade_in") return;

        switch (btnPressed)
        {
            case "StartGame":
                GetTree().ChangeSceneToFile("res://Scenes/Maps/TestMap.tscn");
                break;
            case "Credits":
                GetTree().ChangeSceneToFile("res://Scenes/Maps/credits.tscn");
                break;
            case "Options":
                // Handle options menu logic here
                break;
            case "CloseGame":
                GetTree().Quit();
                break;
        }
    }
}
