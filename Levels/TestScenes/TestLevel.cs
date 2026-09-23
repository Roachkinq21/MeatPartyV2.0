using Godot;
using System;

public partial class TestLevel : Node3D
{
    
    private Interactable _interactable;
    private CharacterBody3D _player;
    private ColorRect _controlNode;
    
    private Control _letterbox;

    
    
    public override void _Ready()
    {
        var letterboxScene = GD.Load<PackedScene>("res://Prefab/LetterBox.tscn");
        _letterbox = letterboxScene.Instantiate<Control>();
        _letterbox.Hide();
        
        _player = GetNode<CharacterBody3D>("Player");
        _interactable = GetNode<Interactable>("Interactable");
        _interactable.Interacted += OnInteractableInteracted;

        var hudLayer = new CanvasLayer();
        AddChild(hudLayer);
        
        hudLayer.AddChild(_letterbox);

        _controlNode = new ColorRect
        {
            Color = new Color(0f, 0f, 0f, 0.75f),
            Position = new Vector2(24f, 24f),
            Size = new Vector2(240f, 80f),
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Visible = false
        };
        hudLayer.AddChild(_controlNode);
    }

    private void OnInteractableInteracted(Node body)
    {
        if (body != _player || !body.IsInGroup("Player"))
        {
            return;
        }

        _letterbox.Show();
    }
}
