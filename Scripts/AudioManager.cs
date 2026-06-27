using Godot;
using System;
using System.Collections.Generic;

public partial class AudioManager : Node2D
{
    [Export]
    private int _maxStreamPlayers = 100;

    private List<AudioStreamPlayer2D> _streamPlayers = [];
    private List<bool> _auidoPlayerStatus = [];
    
    [Export] private AudioSetting _backgroundMusic;

    [Export]
    private AudioSetting[] _normalEnemyDamageSounds = [];
    [Export]
    private AudioSetting[] _tankEnemyDamageSounds = [];
    [Export]
    private AudioSetting[] _normalEnemyDeadSounds = [];
    [Export]
    private AudioSetting[] _tankEnemyDeadSounds = [];

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        for (int i = 0; i < _maxStreamPlayers; ++i)
        {
            AudioStreamPlayer2D newPlayer = new AudioStreamPlayer2D();
            this.AddChild(newPlayer);

            int index = i;
            newPlayer.Finished += () => { _AudioStopedPlaying(index);};

            _streamPlayers.Add(newPlayer);
            _auidoPlayerStatus.Add(false);
        }

        PlaySound(_backgroundMusic);
    }

    private void _AudioStopedPlaying(int index)
    {
        _auidoPlayerStatus[index] = false;
    }

    public void PlaySound(AudioSetting sound)
    {
        for (int i = 0; i < _maxStreamPlayers; ++i)
        {
            if (_auidoPlayerStatus[i]) continue;

            AudioStreamPlayer2D player = _streamPlayers[i];
            player.Stream = sound.source;
            player.VolumeDb = sound.volumeDB;
            player.Play();

            _auidoPlayerStatus[i] = true;

            break;
        }
    }


    public void PlayNormalEnemyDamageSound()
    {
        PlaySound(_normalEnemyDamageSounds[GD.Randi() % _normalEnemyDamageSounds.Length]);
    }

    public void PlayTankEnemyDamageSound()
    {
        PlaySound(_tankEnemyDamageSounds[GD.Randi() % _tankEnemyDamageSounds.Length]);
    }

    public void PlayNormalEnemyDeadSound()
    {
        PlaySound(_normalEnemyDeadSounds[GD.Randi() % _normalEnemyDeadSounds.Length]);
    }

    public void PlayTankEnemyDeadSound()
    {
        PlaySound(_tankEnemyDeadSounds[GD.Randi() % _tankEnemyDeadSounds.Length]);
    }
}
