using Godot;
using System;

public partial class TowerButton : TextureButton
{
    [Export] public PackedScene towerPreview;
    private PreviewTower _towerPreviewInstance;
    public void _on_button_down()
    {
        _towerPreviewInstance = towerPreview.Instantiate<PreviewTower>();
        GetTree().Root.AddChild(_towerPreviewInstance);
        Modulate = new Color(1, 1, 1, 0f);
        _towerPreviewInstance.towerButton = this;
        //QueueFree();
    }
}
