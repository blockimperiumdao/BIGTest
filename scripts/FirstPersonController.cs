using Godot;
using System;

public partial class FirstPersonController : CharacterBody3D
{
    [Export] public float MoveSpeed = 3.0f;
    [Export] public float MouseSensitivity = 0.002f;
    [Export] public float LookLimit = 90.0f;
    [Export] 

    private Camera3D _camera;
    private Vector2 _mouseRotation = Vector2.Zero; // Store yaw and pitch
    private bool _isMenuMode = true;

    public override void _Ready()
    {
        // Get reference to the Camera3D node (assumed to be a child of this node)
        _camera = GetNode<Camera3D>("Camera3D");
        Input.MouseMode = Input.MouseModeEnum.Visible;
        //Input.MouseMode = Input.MouseModeEnum.Captured; // Capture the mouse for FPS-style controls
    }

    public void OnMenuModeChanged(bool isMenuMode)
    {
        GD.Print("Menu mode changed: " + isMenuMode);
        // Change the mouse mode based on the menu mode
        //Input.MouseMode = isMenuMode ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
        _isMenuMode = isMenuMode;
    }

    public override void _Process(double delta)
    {
        // Don't process input if in menu mode
        if (_isMenuMode)
        {
            return;
        }

        //HandleMouseLook();  // Rotate the camera based on mouse movement
        HandleMovement((float)delta);  // Move the player based on keyboard input

    }

    private void HandleMouseLook()
    {
        Vector2 mouseMovement = Input.GetLastMouseVelocity(); // Get mouse movement

        _mouseRotation.X = Mathf.Clamp(_mouseRotation.X - mouseMovement.Y * MouseSensitivity, -LookLimit, LookLimit); // Pitch (Vertical look)
        _mouseRotation.Y -= mouseMovement.X * MouseSensitivity; // Yaw (Horizontal look)

        // Apply the vertical (pitch) rotation to the camera and horizontal (yaw) to the player
        _camera.RotationDegrees = new Vector3(_mouseRotation.X, 0, 0);
        RotationDegrees = new Vector3(0, _mouseRotation.Y, 0); // Rotate the player body horizontally
    }

    private void HandleMovement(float delta)
    {
        Vector3 velocity = Vector3.Zero;

        // Move based on input (WASD or arrow keys)
        if (Input.IsActionPressed("move_forward"))
        {
            velocity -= Transform.Basis.Z; // Move forward in the direction the player is facing
        }
        if (Input.IsActionPressed("move_backward"))
        {
            velocity += Transform.Basis.Z; // Move backward
        }
        if (Input.IsActionPressed("move_left"))
        {
            velocity -= Transform.Basis.X; // Move left
        }
        if (Input.IsActionPressed("move_right"))
        {
            velocity += Transform.Basis.X; // Move right
        }

        // Normalize and apply velocity
        velocity = velocity.Normalized() * MoveSpeed;
        Velocity = new Vector3(velocity.X, Velocity.Y, velocity.Z); // Apply movement, preserving Y axis (gravity)

        MoveAndSlide(); // Moves the character while handling collisions
    }
}
