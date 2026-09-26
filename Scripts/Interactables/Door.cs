using Godot;
using System;
using MeatPartyV0.Empty.Player;

public partial class Door : Interactable
{
    [Export] public Marker3D TeleportMarker { get; set; }

    public override void _Ready()
    {
        base._Ready();
    }

    public void _on_interacted(Node body)
    {
        if (TeleportMarker != null && body is Player player)
        {
            player.GlobalPosition = TeleportMarker.GlobalPosition;
            player.GlobalRotation = TeleportMarker.GlobalRotation;
        }
            
    }
}
