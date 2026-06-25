using Godot;
using System;

#nullable enable
public partial class TowerSlotMachine : TextureRect
{
    public Action? OnLeverPressed;

    private AnimationPlayer? leverPullAnimation;

    public override void _Ready()
    {
        leverPullAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    private void _onTextureButtonPressed()
    {
        if (leverPullAnimation?.IsPlaying()?? false) return;

        OnLeverPressed?.Invoke();
        leverPullAnimation?.Play("pull_lever");
    }
}
