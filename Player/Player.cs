using System;
using Godot;

namespace MeatPartyV0.Empty.Player;

[GlobalClass]
public partial class Player : CharacterBody3D
{
    private Camera3D _camera;
    private Node3D _head;
    private RayCast3D _raycast;
    private CollisionShape3D _standingCol;
    private CollisionShape3D _crouchingCol;
    private RayCast3D _crouchDetect;

    private Vector3 _standingHeadPos;
    private Vector3 _crouchingHeadPos;

    private bool _hasControl = true;
    private bool _isCrouching;
    
    

    private float _currentSpeed;
    [Export] public float DefaultSpeed { get; set; } = 4f;
    [Export] public float SprintMultiplier { get; set; } = 2.3f;
    [Export] public float CrouchSpeed { get; set; } = 1.5f;
    [Export] public float MouseSensitivity { get; set; } = 0.2f;
    [Export] public float LerpSpeed { get; set; } = 10f;
    
    //Headbob
    [Export] public float HeadbobFreq = 0.5f;
    [Export] public float HeadbobAmp = 0.5f;
    [Export] public float HeadbobTime = 0.5f;
    
    [Export] public float HeadRotateAmount { get; set; } = 0.05f;

    private float _currentJumpVelocity = 4.0f;
    
    [Export(PropertyHint.Range, "0,5")] public float AirLerpSpeed { get; set; } = 3f;
    
    
    private Vector3 _targetVelocity = Vector3.Zero;
    private Vector3 _direction = Vector3.Zero;
    private bool _isPaused;
    private Vector2 _mouseInput;
    
    private float _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
    

    public override void _Ready()
    {
        var global = GetNode<Global>("/root/Global");
        
        Input.MouseMode = Input.MouseModeEnum.Captured;
        _head = GetNode<Node3D>("head");
        _camera = GetNode<Camera3D>("head/Camera3D");
        _raycast = GetNode<RayCast3D>("head/RayCast3D");
        _standingCol = GetNode<CollisionShape3D>("StandingCol");
        _crouchingCol = GetNode<CollisionShape3D>("CrouchingCol");
        _crouchDetect = GetNode<RayCast3D>("CrouchDetect");

        _standingHeadPos = new Vector3(0, 0.5f, 0);
        _crouchingHeadPos = _standingHeadPos + new Vector3(0, -0.8f, 0);
        _head.Position = _standingHeadPos;
        
        _currentSpeed = DefaultSpeed;

        _crouchingCol.Disabled = true;

        
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (Input.IsActionJustPressed("hard quit"))
        {
            GetTree().Quit();
        }

        if (Input.IsActionJustReleased("pause"))
        {
            if (!_isPaused)
            {
                _isPaused = true;
                _hasControl = false;
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            else
            {
                _isPaused = false;
                _hasControl = true;
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
            
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventMouseMotion mouseMotion ) return;
        if (!_hasControl) return;
        RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * MouseSensitivity));
        _head.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * MouseSensitivity));
        _head.Rotation = new Vector3(
            Mathf.Clamp(_head.Rotation.X, -1.25f, 1.5f),
            _camera.Rotation.Y,
            _camera.Rotation.Z);
        
        _mouseInput = mouseMotion.Relative;
    }

    
    public override void _PhysicsProcess(double delta)
    {
        GameProcessCheck();
        if (!_hasControl) return;

        var fDelta = (float)delta; 
        Vector3 velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity.Y -= _gravity * fDelta;
        }
        
        var inputDir = Input.GetVector("left", "right", "up", "down");
        //var controllerInputDir = Input.GetVector("right", "left", "up", "down");
        
        
        if (IsOnFloor())
        {
            var wishDir = Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);
            _direction = _direction.Lerp(wishDir.Length() > 0.01f ? wishDir : Vector3.Zero, LerpSpeed * fDelta);
        }
        
        else
        {
            if (inputDir != Vector2.Zero)
            {
                _direction = _direction.Lerp(
                    (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)),
                    fDelta * AirLerpSpeed);
            }
        }

        
        

        velocity.X = _direction.X * _currentSpeed;
        velocity.Z = _direction.Z * _currentSpeed;

        Jump(ref velocity);
        Velocity = velocity;
        MoveAndSlide();
        Sprint(delta);
        Interact();
        Crouch(fDelta);
        CrouchCheck();
        
        CamTilt(inputDir.X, delta);

        HeadbobTime += fDelta * Velocity.Length() * (IsOnFloor() ? 1.0f : 0.0f);
        _camera.Position = HeadBob(HeadbobTime);
        
    }

    private void Sprint(double delta)
    {
        if (_direction.Length() < 0.1f) return;
        if (Input.IsActionPressed("sprint"))
        {
            _currentSpeed = DefaultSpeed * SprintMultiplier;
            _camera.Fov = float.Lerp(_camera.Fov, 80f, Mathf.Clamp(LerpSpeed * (float)delta, 0, 1f));
        }
        else
        {
            _currentSpeed = DefaultSpeed;
            _camera.Fov = float.Lerp(_camera.Fov, 75f, Mathf.Clamp(LerpSpeed * (float)delta, 0, 1f));
        }
        
    }

    private void Crouch(float delta)
    {
        if (Input.IsActionPressed("crouch"))
        {
            if (!IsOnFloor()) return;
            _isCrouching = true;
            _standingCol.Disabled = true;
            _crouchingCol.Disabled = false;
            _head.Position = _head.Position.Lerp(_crouchingHeadPos, Mathf.Clamp(LerpSpeed * delta, 0, 1f));

        }
        else if (!_crouchDetect.IsColliding())
        {
            _isCrouching = false;
            _standingCol.Disabled = false;
            _crouchingCol.Disabled = true;
            _head.Position = _head.Position.Lerp(_standingHeadPos, Mathf.Clamp(LerpSpeed * delta, 0, 1f));
        }
        
        
    }

    private void CrouchCheck()
    {
        if (_isCrouching)
        {
            _currentSpeed = CrouchSpeed;
        }
        else return;
    }

    private Vector3 HeadBob(float headbobTime)
    {
        var headbobPos = Vector3.Zero;
        headbobPos.Y = MathF.Sin(headbobTime * HeadbobFreq) * HeadbobAmp;
        return headbobPos;
    }

    private void Interact()
    {
        if (_raycast.IsColliding())
        {
            var collider =  _raycast.GetCollider();

            if (collider is Interactable interactable)
            {
                if (Input.IsActionJustPressed("interact"))
                {
                    GD.Print($"{collider}");
                    interactable.Interact(this);
                }
                
            }
            
        }
    }

    private void Jump(ref Vector3 velocity)
    {
        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            velocity.Y = _currentJumpVelocity;
        }
    }

    private void CamTilt(float inputX, double delta)
    {
        if(_camera == null) return;
        var rotation = _camera.Rotation;
        var targetTilt = _hasControl ? -inputX * HeadRotateAmount : 0f;

        rotation.Z = Mathf.Lerp(rotation.Z, targetTilt, 9f * (float)delta);
        _camera.Rotation = rotation;
    }

    private void GameProcessCheck()
    {
        var global = GetNode<Global>("/root/Global");
        if (global._inMenu)
        {
            _hasControl = false;
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else if (!_isPaused)
        {
            _hasControl = true;
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }
}
