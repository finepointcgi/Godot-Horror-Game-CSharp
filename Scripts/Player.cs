using Godot;
using GodotHorrorGameCSharp.Scripts;
using System;

public partial class Player : CharacterBody3D
{
	enum States
	{
		walking,
		sneaking,
		running,
		falling
	}
	public const float Speed = 4.0f;
	public const float CrouchedSpeed = 2.0f;
    public const float JumpVelocity = 4.5f;
	public const float Sensitivity = 3.0f;

	public bool IsCrouched;
	public bool FlashlightOut;
	public bool Moving;
	public int NoiseLevel;
	public double LightLevel;
	public float acceleration = .1f;
    public LightDetect lightDetect;
	private Vector3 lastVelocity;

	private bool WasInAirLastFrame;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		base._Ready();
		Input.MouseMode = Input.MouseModeEnum.Captured;
		AudioManager.playerAudioStream = GetNode<AudioStreamPlayer>("Footsteps");
		lightDetect = GetNode<LightDetect>("LightDetect");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
        PhysicsDirectSpaceState3D spaceState = GetWorld3d().DirectSpaceState;
        var result = spaceState.IntersectRay(new PhysicsRayQueryParameters3D() { 
			From = new Vector3(Position.x, Position.y + 2, Position.z), 
			To = Position + new Vector3(0, -1, 0), 
			Exclude = new Godot.Collections.Array { this } 
		});

		Surface surface = null;
		if (result.Count >= 0)
		{

			if (result.ContainsKey("collider"))
			{

				Node3D s = ((Node3D)result["collider"]) as Node3D;
				if (s is Surface)
				{
					surface = s as Surface;

				}
			}
		}
        

		// Add the gravity.
        if (!IsOnFloor())
			velocity.y -= gravity * (float)delta;

		// Handle Jump.
		if (IsOnFloor())
		{
			
			if (WasInAirLastFrame)
			{
				
                AudioManager.playerAudioStream.Stream = surface.JumpLandSteamWAV[0];
                AudioManager.playerAudioStream.Play();
				GD.Print("playing audio");
                NoiseLevel = surface.SoundValue;
            }
              
			velocity.y = 0;
			if (Input.IsActionJustPressed("ui_accept"))
				velocity.y = JumpVelocity;
		}

		LightLevel = lightDetect.LightLevel;

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
            NoiseLevel = 3;
            if (direction != Vector3.Zero)
				NoiseLevel = 1;
        }else
		{
			if (IsCrouched)
			{
				PhysicsDirectSpaceState3D spaceState2 = GetWorld3d().DirectSpaceState;
				var result2 = spaceState2.IntersectRay(new PhysicsRayQueryParameters3D() { From = Position, To = new Vector3(Position.x, Position.y + 2, Position.z), Exclude = new Godot.Collections.Array { this } });
				if (result2.Count <= 0)
				{
					GetNode<AnimationPlayer>("AnimationPlayer").Play("UnCrouch");
					IsCrouched = false;
				}
            }
		}

		if (Input.IsActionJustPressed("Flashlight"))
		{
			if (FlashlightOut)
			{
				GetNode<AnimationPlayer>("AnimationPlayer").Play("FlashlightHide");
                LightLevel -= 1;
            }
			else
			{
				LightLevel += 1;
				GetNode<AnimationPlayer>("AnimationPlayer").Play("Flashlight");
			}

			FlashlightOut = !FlashlightOut;
		}

		if (direction != Vector3.Zero)
		{
           

            velocity.x = Mathf.Lerp(lastVelocity.x, direction.x * speed, acceleration);
			velocity.z = Mathf.Lerp(lastVelocity.z, direction.z * speed, acceleration);
			lastVelocity = velocity;
            if (!AudioManager.playerAudioStream.Playing && checkVelocityAboveValue(new Vector2(velocity.x, velocity.z), 1))
			{
                Random r = new Random();
				GD.Print("playing Audio");
				if (surface != null)
				{
					AudioManager.playerAudioStream.Stream = AudioManager.getNonLastAudioStream(surface.WalkStreamWAV);
                    AudioManager.playerAudioStream.Play();
					NoiseLevel = surface.SoundValue;
				}
			}
        }
		else
		{

			if (AudioManager.playerAudioStream.IsPlaying())
			{
				AudioManager.playerAudioStream.Stop();

            }
            velocity.x = Mathf.MoveToward(Velocity.x, 0, speed);
			velocity.z = Mathf.MoveToward(Velocity.z, 0, speed);
            lastVelocity = velocity;
			NoiseLevel = 0;			
        }

		Velocity = velocity;
        if (IsOnFloor())
        {
            WasInAirLastFrame = false;

        }
        else
        {
            WasInAirLastFrame = true;
        }
        MoveAndSlide();
		
	}

	private bool checkVelocityAboveValue(Vector3 velocity, float valueToCheck) => Mathf.Abs(velocity.x) > valueToCheck && Mathf.Abs(velocity.y) > valueToCheck || Mathf.Abs(velocity.z) > valueToCheck;
	private bool checkVelocityAboveValue(Vector2 velocity, float valueToCheck) => Mathf.Abs(velocity.x) > valueToCheck || Mathf.Abs(velocity.y) > valueToCheck;


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
