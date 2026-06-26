using Godot;

#nullable enable
public partial class VictoryScreen : CenterContainer
{
    ColorRect? fadeTransition;
    AnimationPlayer? fadeAnimation;
    string btnPressed = "";


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        fadeTransition = GetNode<ColorRect>("FadeTransition");
        fadeAnimation = GetNode<AnimationPlayer>("FadeTransition/AnimationPlayer");
        if (fadeAnimation != null)
        {
            fadeAnimation.AnimationFinished += _onFadeAnimationFinished;
        }
        fadeTransition?.Hide();
    }
    
    private void _onInfiniteModePressed()
    {
        // Handle infinite mode logic here
    }

    private void _onMainMenuPressed()
    {
        btnPressed = "MainMenu";
        fadeTransition?.Show();
        fadeAnimation?.Play("fade_in");
    }

    private void _onFadeAnimationFinished(StringName animName)
    {
        fadeTransition?.Hide();
        
        if (animName != "fade_in") return;

        switch (btnPressed)
        {
            case "MainMenu":
                GetTree().Paused = false;
                GetTree().ChangeSceneToFile("res://Scenes/Maps/main_menu.tscn");
                break;
        }
    }
}
