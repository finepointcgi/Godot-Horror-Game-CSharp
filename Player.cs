using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float CrouchedSpeed = 2.0f;
    public const float JumpVelocity = 4.5f;
	public const float Sensitivity = 3.0f;

	public bool IsCrouched;
	public bool FlashlightOut;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		base._Ready();
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBackward");
		Vector3 direction = (Transform.basis * new Vector3(inputDir.x, 0, inputDir.y)).Normalized();
		float speed = Speed;
		if (Input.IsActionPressed("Crouch")){
			if (!IsCrouched)
			{
                GetNode<AnimationPlayer>("AnimationPlayer").Play("Crouch");
				IsCrouched = true;
            }
			speed = CrouchedSpeed;
        }else
		{
			if (IsCrouched)
			{
				PhysicsDirectSpaceState3D spaceState = GetWorld3d().DirectSpaceState;
				var result = spaceState.IntersectRay(new PhysicsRayQueryParameters3D() { From = Position, To = new Vector3(Position.x, Position.y + 2, Position.z), Exclude = new Godot.Collections.Array { this } });
				if (result.Count <= 0)
				{
					GetNode<AnimationPlayer>("AnimationPlayer").Play("UnCrouch");
					IsCrouched = false;
				}
            }
		}

		if (Input.IsActionJustPressed("Flashlight"))
		{
			if (FlashlightOut)
                GetNode<AnimationPlayer>("AnimationPlayer").Play("FlashlightHide");
			else
                GetNode<AnimationPlayer>("AnimationPlayer").Play("Flashlight");

			FlashlightOut = !FlashlightOut;
		}

		if (direction != Vector3.Zero)
		{
			velocity.x = direction.x * speed;
			velocity.z = direction.z * speed;
		}
		else
		{
			velocity.x = Mathf.MoveToward(Velocity.x, 0, speed);
			velocity.z = Mathf.MoveToward(Velocity.z, 0, speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if(@event is InputEventMouseMotion)
		{
			InputEventMouseMotion motion = @event as InputEventMouseMotion;
			Rotation = new Vector3(Rotation.x, Rotation.y - motion.Relative.x / 1000 * Sensitivity, Rotation.z);
			Camera3D camera = GetNode<Camera3D>("Camera3d");

            camera.Rotation = new Vector3(Mathf.Clamp(camera.Rotation.x - motion.Relative.y / 1000 * Sensitivity, -2, 2), camera.Rotation.y, camera.Rotation.z);
		}
	}
}
