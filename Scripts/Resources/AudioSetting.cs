using Godot;
using System;

[GlobalClass]
public partial class AudioSetting : Resource
{
    [Export]
    public AudioStream source{ get; private set; }
    
    [Export(PropertyHint.Range, "-40,20,")]
    public float volumeDB{ get; private set; } = 0.0f;
}
