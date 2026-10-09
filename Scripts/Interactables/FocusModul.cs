using Godot;
using System;
using MeatPartyV0.Empty.Player;

public partial class FocusModul : Interactable
{

    
    private Camera3D _focusCam;
    private Camera3D _playerCam;

    private Transform3D _focusStartTransform;
    private float _offsetScale = 0.001f;
    private float _cameraSmoothSpeed = 8f;
    private float _minOff = -100;
    private float _maxOff = 100;
    private Vector2 _desiredOffset;
    
    public override void _Ready()
    {
        _focusCam = GetNode<Camera3D>("FocusCam");
        _focusStartTransform = _focusCam.Transform;
    }

    public override void _Process(double delta)
    {
        if (_playerCam == null || !_focusCam.Current)
            return;

        Vector2 mousePosition = GetViewport().GetMousePosition();
        Vector2 screenCenter = GetViewport().GetVisibleRect().Size * 0.5f;

        _desiredOffset = (mousePosition - screenCenter) * 0.5f;

        _desiredOffset.X = Mathf.Clamp(
            _desiredOffset.X, _minOff, _maxOff);

        _desiredOffset.Y = Mathf.Clamp(
            _desiredOffset.Y, _minOff / 2f, _maxOff / 2f);

        var cameraOffset = new Vector3(
            _desiredOffset.X,
            -_desiredOffset.Y,
            0f
        ) * _offsetScale;

        var targetPosition = _focusStartTransform.Origin + _focusStartTransform.Basis * cameraOffset;

        var weight = 1f - Mathf.Exp(-_cameraSmoothSpeed * (float)delta);

        _focusCam.Position = _focusCam.Position.Lerp(targetPosition, weight);
        
        
    }
    
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        base._UnhandledKeyInput(@event);
        if (!Input.IsActionPressed("tab") || _playerCam == null) return;
        
        _playerCam.MakeCurrent();
        _playerCam = null;
        
        var global = GetNode<Global>("/root/Global");
        global._inMenu = false;
    }
    
    public void _on_interacted(Node body)
    {
        if (body is Player player)
        {
            _playerCam = player.GetNode<Camera3D>("head/Camera3D");
            _focusCam.Transform = _focusStartTransform;
            _focusCam.MakeCurrent();
            
            var global = GetNode<Global>("/root/Global");
            global._inMenu = true;


        }
    }
}
