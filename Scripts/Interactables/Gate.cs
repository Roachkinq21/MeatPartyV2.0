using Godot;
using System;
using MeatPartyV0.Empty.Scripts;

public partial class Gate : Interactable
{
    public override void _Ready()
    {
        base._Ready();
    }

    private void _on_interacted(Node body)
    {
        if (Inventory.SCORE < 1) return;
        Inventory.SCORE--;
        this.QueueFree();
    }
}
