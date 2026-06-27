using Godot;
using System;

public partial class Credits : Control
{
	ColorRect fadeTransition;
	AnimationPlayer fadeAnimation;

	string btnPressed;

    private Label _programmersTitle;
    private Label _artistsTitle;

    [Export]
    private double _minTimeOn = 0.3;
    [Export]
    private double _maxTimeOn = 2.0;
    [Export]
    private double _minTimeOff = 0.05;
    [Export]
    private double _maxTimeOff = 0.3;

    private bool _programmersLight = true;
    private double _programmersCurrentFlickerTimeout = 0.0f;
    private bool _artistsLight = true;
    private double _artistsCurrentFlickerTimeout = 0.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
		fadeTransition = GetNode<ColorRect>("FadeTransition");
		fadeAnimation = GetNode<AnimationPlayer>("FadeTransition/AnimationPlayer");
		fadeAnimation.AnimationFinished += _onFadeAnimationFinished;
		fadeTransition.Show();
		fadeAnimation.Play("fade_out");

        _programmersTitle = GetNode<Label>("ColorRect/VBoxContainer/Programmers");
        _artistsTitle = GetNode<Label>("ColorRect/VBoxContainer/Artists");

        _programmersCurrentFlickerTimeout = GD.Randf() * (_maxTimeOn - _minTimeOn) + _minTimeOn;
        _artistsCurrentFlickerTimeout = GD.Randf() * (_maxTimeOn - _minTimeOn) + _minTimeOn;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("pause") && GetTree().Paused)
        {
            GetTree().Paused = false;
            Hide();
        }

        _programmersCurrentFlickerTimeout -= delta;
        _artistsCurrentFlickerTimeout -= delta;

        if (_programmersCurrentFlickerTimeout <= 0.0)
        {
            _programmersLight = !_programmersLight;
            _programmersCurrentFlickerTimeout = GD.Randf() * (_programmersLight ? (_maxTimeOn - _minTimeOn) : (_maxTimeOff - _minTimeOff)) + (_programmersLight ? _minTimeOn : _minTimeOff);

            _programmersTitle.AddThemeColorOverride("font_color", _programmersLight ? new Color("#ffc200") : new Color("#785900"));
        }
        if (_artistsCurrentFlickerTimeout <= 0.0)
        {
            _artistsLight = !_artistsLight;
            _artistsCurrentFlickerTimeout = GD.Randf() * (_artistsLight ? (_maxTimeOn - _minTimeOn) :(_maxTimeOff - _minTimeOff)) + (_artistsLight ? _minTimeOn : _minTimeOff);

            _artistsTitle.AddThemeColorOverride("font_color", _artistsLight ? new Color("#ffc200") : new Color("#785900"));
        }
    }

    private void _onBackButtonPressed()
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
