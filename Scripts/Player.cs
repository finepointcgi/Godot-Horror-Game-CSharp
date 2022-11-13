using Godot;

/// <summary>
/// The main player class
/// </summary>
public partial class Player : CharacterBody3D
{
	/// <summary>
	/// A Global Refrence To The Player
	/// </summary>
	public static Player player;
	/// <summary>
	/// How fast the player moves
	/// </summary>
	public const float Speed = 5.0f;
	/// <summary>
	/// How fast the player moves when crouched
	/// </summary>
	public const float CrouchedSpeed = 2.0f;
	/// <summary>
	/// How high the player jumps
	/// </summary>
	public const float JumpVelocity = 4.5f;
	/// <summary>
	/// How much the mouse moves the camera
	/// </summary>
	public const float Sensitivity = 3.0f;
	/// <summary>
	/// If the player is crouched
	/// </summary>
	public bool IsCrouched;
	/// <summary>
	/// If the flashlight is out
	/// </summary>
	public bool FlashlightOut;
	/// <summary>
	/// The light detection object that is used for sneaking
	/// </summary>
	private LightDetect LightDetectObject;
	/// <summary>
	/// The current light value of the player
	/// </summary>
	public double LightValue;
	/// <summary>
	/// The noise value the user has when they are moving around
	/// </summary>
	public double NoiseValue;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	/// <summary>
	/// The gravity of the player
	/// </summary>
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	/// <summary>
	/// The audio player used for the players footsteps
	/// </summary>
	private AudioStreamPlayer footAudioPlayer;
	/// <summary>
	/// The audio player used for the players jump sounds
	/// </summary>
	private AudioStreamPlayer jumpAudioPlayer;
	/// <summary>
	/// If the player was in air last frame
	/// </summary>
	private bool wasInAirLastFrame;
	/// <summary>
	/// The initial surface object used when surface is null
	/// </summary>
	private Surface surfaceInit;
    public override void _Ready()
	{
		player = this;
		base._Ready();
		Input.MouseMode = Input.MouseModeEnum.Captured;
		LightDetectObject = GetNode<LightDetect>("LightDetect");
		surfaceInit = new Surface();
		surfaceInit.SurfaceResource = ResourceLoader.Load<SurfaceResource>("res://Sounds/Wood.tres");

		footAudioPlayer = GetNode<AudioStreamPlayer>("Footsteps");
		jumpAudioPlayer = GetNode<AudioStreamPlayer>("Jump");
    }

	public override void _PhysicsProcess(double delta)
	{
		NoiseValue = 0;
		Vector3 velocity = Velocity;
        PhysicsDirectSpaceState3D spaceState = GetWorld3d().DirectSpaceState;
        var surfaceResult = spaceState.IntersectRay(new PhysicsRayQueryParameters3D() { 
			From = new Vector3(Position.x, Position.y + .5f, Position.z), 
			To = new Vector3(Position.x, Position.y - 1, Position.z), 
			Exclude = new Godot.Collections.Array { this } });
		Surface surface = surfaceInit;
		if (surfaceResult.Count > 0)
		{
			if (surfaceResult.ContainsKey("collider"))
			{
				
                if (surfaceResult["collider"].AsGodotObject() is Surface)
				{
					surface = (Surface)surfaceResult["collider"];

                }
			}
		}
		// Add the gravity.
            if (!IsOnFloor())
			velocity.y -= gravity * (float)delta;

		// Handle Jump.
		if (IsOnFloor())
		{
			if (wasInAirLastFrame)
			{
				jumpAudioPlayer.Stream = surface.SurfaceResource.JumpLandStreamWAV;
				jumpAudioPlayer.Play();
				NoiseValue = surface.SurfaceResource.JumpLandNoiseLevel;
			}
			if (Input.IsActionJustPressed("ui_accept"))
				velocity.y = JumpVelocity;
		}

		LightValue = LightDetectObject.LightLevel;
		//GD.Print(LightValue);
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBackward");
		Vector3 direction = (Transform.basis * new Vector3(inputDir.x, 0, inputDir.y)).Normalized();
		float speed = Speed;
		if (Input.IsActionPressed("Crouch"))
		{
			if (!IsCrouched)
			{
				GetNode<AnimationPlayer>("AnimationPlayer").Play("Crouch");
				IsCrouched = true;
			}

		}
		else
		{
			if (IsCrouched)
			{
				var result = spaceState.IntersectRay(new PhysicsRayQueryParameters3D() { From = Position, To = new Vector3(Position.x, Position.y + 2, Position.z), Exclude = new Godot.Collections.Array { this } });
				if (result.Count <= 0)
				{
					GetNode<AnimationPlayer>("AnimationPlayer").Play("UnCrouch");
					IsCrouched = false;
				}
			}
		}
		if (IsCrouched)
			speed = CrouchedSpeed;

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

			if (IsOnFloor()) {
				if (IsCrouched)
				{
					NoiseValue = surface.SurfaceResource.NoiseLevel / 3;
				}
				else
				{
                    NoiseValue = surface.SurfaceResource.NoiseLevel;
                }

				if (!footAudioPlayer.Playing)
				{
					if (IsCrouched)
					{
						footAudioPlayer.Stream = surface.SurfaceResource.SneakStreamWAV;
					}
					else
					{
						footAudioPlayer.Stream = surface.SurfaceResource.WalkStreamWAV;
					}
					footAudioPlayer.Play();

				} else if (!IsOnFloor())
				{
					footAudioPlayer.Stop();
				}
			}
		}
		else
		{
			velocity.x = Mathf.MoveToward(Velocity.x, 0, speed);
			velocity.z = Mathf.MoveToward(Velocity.z, 0, speed);
			if (footAudioPlayer.Playing)
			{
				footAudioPlayer.Stop();
			}
		}

		Velocity = velocity;
		wasInAirLastFrame = !IsOnFloor();
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventMouseMotion)
		{
			InputEventMouseMotion motion = @event as InputEventMouseMotion;
			Rotation = new Vector3(Rotation.x, Rotation.y - motion.Relative.x / 1000 * Sensitivity, Rotation.z);
			Camera3D camera = GetNode<Camera3D>("Camera3d");

			camera.Rotation = new Vector3(Mathf.Clamp(camera.Rotation.x - motion.Relative.y / 1000 * Sensitivity, -2, 2), camera.Rotation.y, camera.Rotation.z);
		}
	}
}
