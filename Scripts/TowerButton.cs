using Godot;
using System;

public partial class TowerButton : TextureButton
{
    [Export] public PackedScene towerPreview;
    private Node2D _towerPreviewInstance;
    public void _on_button_down()
    {
        _towerPreviewInstance = towerPreview.Instantiate<Node2D>();
        GetTree().Root.AddChild(_towerPreviewInstance);
        QueueFree();
    }
}
