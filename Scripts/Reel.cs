using Godot;
using System;
using Godot.Collections;
using System.Linq;

public partial class Reel : Node
{
    public ReelResult reelResult;

    public Array<PackedScene> Results;
    public Array<int> Probabilities;

    [Export]
    public Array<Texture2D> Textures = [];
    [Export]
    public Array<ReelResultType> ResultTypes = [];
    [Export]
    public PackedScene Slot;
    [Export]
    public float Speed = 1f;

    [Export]
    public Control rulett;

    public int index = 0;
    public int result = 0;
    public int result_in_num = 0;
    public bool active = false;
    private float activeTime = 0;

    public override void _Ready()
    {
        Results = GetParent<SlotMachine>().Results;
        Probabilities = GetParent<SlotMachine>().Probabilities;
        for(var i = 0; i < 4; i++)
        {
            var newSlot = AddSlot();
            newSlot.Position = new Vector2(0, i * 123);
        }
    }

    public Slot AddSlot()
    {
        var newSlot = Slot.Instantiate<Slot>();
        rulett.AddChild(newSlot);
        newSlot.GetChild<TextureRect>(0).Texture = Textures[index];
        newSlot.resultType = ResultTypes[index];
        newSlot.Position = new Vector2(0, 369);
        index++;
        index %= Textures.Count;
        return newSlot;
    }


    public override void _Process(double delta)
    {
        if (!active) return;
        activeTime += (float)delta;

        if(
            rulett.GetChild<Slot>(2).Position.Y < 129 && 
            rulett.GetChild<Slot>(2).Position.Y > 117 &&
            rulett.GetChild<Slot>(2).resultType == reelResult.type
        ){
            active = false;
            reelResult.Modulate = new Color(1, 1, 1, 1f);
            return;
        }
        foreach (Slot child in rulett.GetChildren().OfType<Slot>())
        {
            child.Position += Vector2.Down * (float)delta * Speed * 216.0f;
            if(child.Position.Y < -123)
            {
                child.QueueFree();
                AddSlot();
            }
        }
    }

    public int roll()
    {
        active = true;
        activeTime = 0;
        if(IsInstanceValid(reelResult))
        {
            reelResult.QueueFree();
        }
        var total = 0;
        foreach (var n in Probabilities)
        {
            total += n;
        }
        var v = (int)(GD.Randi() % total);
        var i = 0;
        foreach (var n in Probabilities)
        {
            v -= n;
            if (v < 0)
            {
                break;
            }
            i++;
        }
        result_in_num = i;
        reelResult = (ReelResult)Results[result_in_num].Instantiate();
        AddChild(reelResult);
        reelResult.AnchorBottom = .5f;
        reelResult.AnchorTop = .5f;
        reelResult.AnchorLeft = .5f;
        reelResult.AnchorRight = .5f;
        reelResult.Modulate = new Color(1, 1, 1, 0f);
        return reelResult.getResult();
    }
}
