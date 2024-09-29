extends CharacterBody3D

@export var move_speed: float = 3.0
@export var mouse_sensitivity: float = 0.002
@export var look_limit: float = 90.0

@export var left_joystick: VirtualJoystick
@export var right_joystick: VirtualJoystick

var _camera: Camera3D
var _mouse_rotation: Vector2 = Vector2.ZERO  # Store yaw and pitch
var _is_menu_mode: bool = true # app starts in menu mode

func _ready() -> void:
	# Get reference to the Camera3D node (assumed to be a child of this node)
	_camera = $Camera3D
	Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
	left_joystick.visible = false
	right_joystick.visible = false
# Input.mouse_mode = Input.MOUSE_MODE_CAPTURED  # Capture the mouse for FPS-style controls

func on_menu_mode_changed(is_menu_mode: bool) -> void:
	print("Menu mode changed: ", is_menu_mode)
	# Change the mouse mode based on the menu mode
	# Input.mouse_mode = if is_menu_mode else Input.MOUSE_MODE_CAPTURED
	_is_menu_mode = is_menu_mode

	if !is_menu_mode:
		left_joystick.visible = true
		right_joystick.visible = true
	else:
		left_joystick.visible = false
		right_joystick.visible = false


func _process(delta: float) -> void:
	# Don't process input if in menu mode
	if _is_menu_mode:
		return

	# handle_mouse_look()  # Rotate the camera based on mouse movement
	handle_movement(delta)  # Move the player based on keyboard input

func handle_mouse_look() -> void:
	var mouse_movement: Vector2 = Input.get_last_mouse_velocity()  # Get mouse movement

	_mouse_rotation.x = clamp(_mouse_rotation.x - mouse_movement.y * mouse_sensitivity, -look_limit, look_limit)  # Pitch (Vertical look)
	_mouse_rotation.y -= mouse_movement.x * mouse_sensitivity  # Yaw (Horizontal look)

	# Apply the vertical (pitch) rotation to the camera and horizontal (yaw) to the player
	_camera.rotation_degrees = Vector3(_mouse_rotation.x, 0, 0)
	rotation_degrees = Vector3(0, _mouse_rotation.y, 0)  # Rotate the player body horizontally

func handle_movement(_delta: float) -> void:
	#var velocity: Vector3 = Vector3.ZERO

	#print("handle_movement")

	var is_idle:bool = true


	# Move based on input (WASD or arrow keys)
	if Input.is_action_pressed("move_forward"):
		velocity -= transform.basis.z  # Move forward in the direction the player is facing
		is_idle = false
	if Input.is_action_pressed("move_backward"):
		velocity += transform.basis.z  # Move backward
		is_idle = false
	if Input.is_action_pressed("move_left"):
		velocity -= transform.basis.x  # Move left
		is_idle = false
	if Input.is_action_pressed("move_right"):
		velocity += transform.basis.x  # Move right
		is_idle = false
	
	if is_idle:
		velocity = Vector3.ZERO  # Stop moving if no keys are pressed

	# Normalize and apply velocity
	velocity = velocity.normalized() * move_speed
	velocity = Vector3(velocity.x, velocity.y, velocity.z)  # Apply movement, preserving Y axis (gravity)

	move_and_slide()  # Moves the character while handling collisions