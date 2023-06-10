using Godot;
using GodotHorrorGameCSharp.Scripts;

/// <summary>
/// The main player class
/// </summary>
public partial class Player : CharacterBody3D
{
	/// <summary>
	/// The states that are open to the player to use.
	/// </summary>
	enum states
	{
		walking,
		sneaking,
		crouching,
		inAir,
		standing,
		jumping
	}
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
	public float gravity;
	/// <summary>
	/// The audio player used for the players footsteps
	/// </summary>
	private AudioStreamPlayer footAudioPlayer;
	/// <summary>
	/// The audio player used for the players jump sounds
	/// </summary>
	private AudioStreamPlayer jumpAudioPlayer;
	private AudioStreamPlayer eventPlayer;
	/// <summary>
	/// If the player was in air last frame
	/// </summary>
	private bool wasInAirLastFrame;
	/// <summary>
	/// The initial surface object used when surface is null
	/// </summary>
	private Surface surfaceInit;
	/// <summary>
	/// The current state the player is in
	/// </summary>
	private states currentState;
	/// <summary>
	/// The players animation player used to play animations
	/// </summary>
	private AnimationPlayer playerAnimationPlayer;
	private RayCast3D headRaycast;
	private RigidBody3D grabbedObject;
	private Generic6DofJoint3D grabbedJoint;
	private Camera3D camera;
	private Inventory inventory;
	public override void _Ready()
	{
		gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
        currentState = states.standing;

		player = this;
		base._Ready();
		Input.MouseMode = Input.MouseModeEnum.Captured;
		LightDetectObject = GetNode<LightDetect>("LightDetect");
		surfaceInit = new Surface();
		surfaceInit.SurfaceResource = ResourceLoader.Load<SurfaceResource>("res://Sounds/Wood.tres");

		footAudioPlayer = GetNode<AudioStreamPlayer>("Footsteps");
		jumpAudioPlayer = GetNode<AudioStreamPlayer>("Jump");
		eventPlayer = GetNode<AudioStreamPlayer>("EventSoundPlayer");
		playerAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		camera = GetNode<Camera3D>("Camera3d");
        headRaycast = camera.GetNode<RayCast3D>("RayCast3D");
		grabbedJoint = camera.GetNode<Generic6DofJoint3D>("Generic6DOFJoint3D");
		inventory = GetNode<Inventory>("Inventory");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!GameManager.Instance.Paused)
		{
			wasInAirLastFrame = !IsOnFloor();
			Vector3 velocity = getInput();
			LightValue = LightDetectObject.LightLevel;

			handleSound(getSurface());
			handleAnimation();
			handleMovement(velocity, delta);
		}
	}
	/// <summary>
	/// Handles all the animations the player uses. Based off of state it will run though its varous animations.
	/// </summary>
	private void handleAnimation()
	{
		if (currentState == states.sneaking || currentState == states.crouching)
		{
			if (!IsCrouched)
			{
				playerAnimationPlayer.Play("Crouch");
				IsCrouched = true;
			}

		}
		else
		{
			if (IsCrouched)
			{
				PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
				var result = spaceState.IntersectRay(
					new PhysicsRayQueryParameters3D()
					{
						From = Position,
						To = new Vector3(Position.X, Position.Y + 2, Position.Z),
						Exclude = { this.GetRid() }
					});

				if (result.Count <= 0)
				{
					playerAnimationPlayer.PlayBackwards("Crouch");
					IsCrouched = false;
				}
				else
				{
					currentState = states.sneaking;
				}
			}
		}
	}
	/// <summary>
	/// Handles all the sound the player generates from sound effects 
	/// from walking to the noise values it produces when the player is moving
	/// </summary>
	/// <param name="surface">The current surface the player is on</param>
	private void handleSound(Surface surface)
	{
		NoiseValue = 0;
		if (currentState == states.walking)
		{
			NoiseValue = surface.SurfaceResource.NoiseLevel;
			if (!footAudioPlayer.Playing)
			{
				footAudioPlayer.Stream = surface.SurfaceResource.WalkStreamWAV;
				footAudioPlayer.Play();
			}
		}

		if (currentState == states.sneaking)
		{
			NoiseValue = surface.SurfaceResource.NoiseLevel / 3;
			if (!footAudioPlayer.Playing)
			{
				footAudioPlayer.Stream = surface.SurfaceResource.SneakStreamWAV;
				footAudioPlayer.Play();
			}
		}

		if (currentState == states.inAir && wasInAirLastFrame)
		{
			NoiseValue = surface.SurfaceResource.JumpLandNoiseLevel;
			jumpAudioPlayer.Stream = surface.SurfaceResource.JumpLandStreamWAV;
			jumpAudioPlayer.Play();
		}

		if (currentState == states.standing || currentState == states.crouching)
		{
			if (footAudioPlayer.Playing)
				footAudioPlayer.Stop();
		}
	}
	/// <summary>
	/// Handles the movement of the player. It will allow the player to move and sneak.
	/// </summary>
	/// <param name="direction">Direction the player is moving</param>
	/// <param name="delta">The overall Godot Delta time</param>
	private void handleMovement(Vector3 direction, double delta)
	{
		Vector3 velocity = Velocity;
		float speed = Speed;
		if (currentState == states.sneaking)
			speed = CrouchedSpeed;

		if (currentState == states.jumping)
			velocity.Y = JumpVelocity;

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
		}
		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;
		Velocity = velocity;
		MoveAndSlide();
	}
	/// <summary>
	/// The main switcher between states. It will take in user inputs and define the state that the player is in.
	/// </summary>
	/// <returns>The direction the player is moving in if any</returns>
	private Vector3 getInput()
	{
		Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBackward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (Input.IsActionJustPressed("Flashlight"))
			handleFlashlight();

		if (direction != Vector3.Zero)
		{
			if (Input.IsActionPressed("Crouch"))
				currentState = states.sneaking;
			else
				currentState = states.walking;

		}
		else
		{
			if (Input.IsActionPressed("Crouch"))
				currentState = states.crouching;
			else
				currentState = states.standing;

		}

		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
		{
			currentState = states.jumping;
		}
		else if (!IsOnFloor())
		{
			currentState = states.inAir;
		}
		if (headRaycast != null)
		{
			if (headRaycast.IsColliding())
			{
				GodotObject obj = headRaycast.GetCollider();
				if (obj is Interactable)
				{
					Interactable interactable = obj as Interactable;
					if (Input.IsActionJustPressed("Interact"))
					{
						interactable.Interact();
					}
				}
			}
		}
		if(grabbedObject != null)
		{
			if (Input.IsActionJustPressed("Throw"))
			{
				RigidBody3D temp = grabbedObject;
				GrabObject(grabbedObject);
				temp.ApplyImpulse((camera.GlobalPosition - temp.GlobalPosition).Normalized() * -1 * 5);
			}
		}
		if (Input.IsActionJustPressed("Inventory"))
		{
			inventory.Visible = !inventory.Visible;
			if(inventory.Visible)
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
			else
			{
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
		}

		return direction;
	}
	/// <summary>
	/// Handles the flashlight logic. this will show the flashlight if hidden and hide if flashlight is active.
	/// </summary>
	private void handleFlashlight()
	{
		if (FlashlightOut)
			GetNode<AnimationPlayer>("AnimationPlayer").Play("FlashlightHide");
		else
			GetNode<AnimationPlayer>("AnimationPlayer").Play("Flashlight");

		FlashlightOut = !FlashlightOut;
	}
	/// <summary>
	/// Returns the surface object under the player
	/// </summary>
	/// <returns>Surface object that is currently under the player</returns>
	private Surface getSurface()
	{
		Surface surface = surfaceInit;
		PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
		var surfaceResult = spaceState.IntersectRay(new PhysicsRayQueryParameters3D()
		{
			From = new Vector3(Position.X, Position.Y + .5f, Position.Z),
			To = new Vector3(Position.X, Position.Y - 1, Position.Z),
			Exclude = { this.GetRid() }
		});
		surface = surfaceInit;
		if (surfaceResult.Count > 0)
		{
			if (surfaceResult.ContainsKey("collider"))
			{

				if (surfaceResult["collider"].AsGodotObject() is Surface)
				{
					surface = surfaceResult["collider"].Obj as Surface;
				}
			}
		}
		return surface;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (!GameManager.Instance.Paused)
		{
			if (@event is InputEventMouseMotion && !inventory.Visible)
			{
				InputEventMouseMotion motion = @event as InputEventMouseMotion;
				Rotation = new Vector3(Rotation.X, Rotation.Y - motion.Relative.X / 1000 * Sensitivity, Rotation.Z);
				Camera3D camera = GetNode<Camera3D>("Camera3d");

				camera.Rotation = new Vector3(Mathf.Clamp(camera.Rotation.X - motion.Relative.Y / 1000 * Sensitivity, -2, 2), camera.Rotation.Y, camera.Rotation.Z);
			}
		}
	}

	public void PlayNonPositionalSound(AudioStream stream, float volume)
	{
		eventPlayer.Stream = stream;
		eventPlayer.VolumeDb = volume;
		eventPlayer.Play();
	}
	
	public void GrabObject(RigidBody3D body)
	{
		if(grabbedObject == null)
		{
			grabbedObject = body;
			grabbedJoint.NodeB = body.GetPath();
		}
		else
		{
			grabbedObject = null;
			grabbedJoint.NodeB = grabbedJoint.GetNode<StaticBody3D>("ResetBody").GetPath();
		}
	}
}

