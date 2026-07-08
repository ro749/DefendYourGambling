using Godot;
using System;

#nullable enable
public partial class TowerSlotMachine : TextureRect
{
    public Action? OnLeverPressed;

    private AnimationPlayer? leverPullAnimation;

    [Export]
    private bool _startCallAnimationOnReady = false;

    public override void _Ready()
    {
        leverPullAnimation = GetNode<AnimationPlayer>("AnimationPlayer");

        if (_startCallAnimationOnReady)
        {
            leverPullAnimation?.Play("pull_call");
        }
    }

    private void _onTextureButtonPressed()
    {
        if (leverPullAnimation?.CurrentAnimation == "pull_call" && (leverPullAnimation?.IsPlaying()?? false))
        {
            leverPullAnimation?.Stop();
        }

        if (leverPullAnimation?.IsPlaying()?? false) return;

        OnLeverPressed?.Invoke();
        leverPullAnimation?.Play("pull_lever");
    }
}
