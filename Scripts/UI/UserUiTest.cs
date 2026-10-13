using Godot;
using System;
using MeatPartyV0.Empty.Scripts;

public partial class UserUiTest : CanvasLayer
{
    public Label keyCount;

    public override void _Ready()
    {
        keyCount = GetNode<Label>("Control/KeyCount");
    }

    public override void _Process(double delta)
    {
        keyCount.Text = "Key Count: " + Inventory.SCORE;
    }
}
