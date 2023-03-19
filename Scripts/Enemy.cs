using Godot;
using GodotHorrorGameCSharp.Scripts;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// The Main enemy class
/// </summary>
public partial class Enemy : CharacterBody3D
{
	/// <summary>
	/// The States alv to the Enemy
	/// </summary>
	public enum States
	{
		Patrol,
		Chasing,
		Hunting,
		Waiting
	}
	/// <summary>
	/// The Enemies Current State
	/// </summary>
	public States CurrentState;
	/// <summary>
	/// When patrolling how fast the enemy should move to each point
	/// </summary>
	private float patrolSpeed = 2;
	/// <summary>
	/// When chasing the player how fast to move 
	/// </summary>
	private float chaseSpeed = 3;
	/// <summary>
	/// The patrol timer used to determine wait time at each patrol point
	/// </summary>
	private Timer patrolTimer;
	/// <summary>
	/// The navigation agent used for patroling and chasing
	/// </summary>
	public NavigationAgent3D NavigationAgent;
	/// <summary>
	/// A list of waypoints used for patroling
	/// </summary>
	private List<Marker3D> waypoints = new List<Marker3D>();
	/// <summary>
	/// The current waypoint index offset
	/// </summary>
	private int waypointIndex;
	/// <summary>
	/// Where the last looking direction of the enemy was
	/// </summary>
	private Vector3 lastLookingDirection;
	/// <summary>
	/// If player is in earshot far
	/// </summary>
	private bool playerInEarshotFar;
	/// <summary>
	/// If player is in earshot close
	/// </summary>
	private bool playerInEarshotClose;
	/// <summary>
	/// If player is in sight far
	/// </summary>
	private bool playerInSightFar;
	/// <summary>
	/// If the player is in close sight
	/// </summary>
	private bool playerInSightClose;
	/// <summary>
	/// The player
	/// </summary>
	private Player player;
	/// <summary>
	/// The enemys head object
	/// </summary>
	private Node3D head;
	/// <summary>
	/// The players camera
	/// </summary>
	private Node3D playerCamera;
	/// <summary>
	/// Detects player close sound value
	/// </summary>
	[Export]
	private float CloseSoundDetect = .4f;
    /// <summary>
    /// Detects player far sound value
    /// </summary>
    [Export]
	private float FarSoundDetect = .6f;
    /// <summary>
    /// Detects player close light value
    /// </summary>
    [Export]
	private float CloseLightDetect = .3f;
    /// <summary>
    /// Detects player far light value
    /// </summary>
    [Export]
    private float FarLightDetect = .5f;
    /// <summary>
    /// Detects player close crouched light value
    /// </summary>
    [Export]
    private float CloseCrouchedLightDetect = .3f;
    /// <summary>
    /// Detects player far crouched light value
    /// </summary>
    [Export]
    private float FarCrouchedLightDetect = .6f;

	private List<Node3D> soundObjects = new List<Node3D>();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{

		head = GetNode<Node3D>("Head");
		NavigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		CurrentState = States.Patrol;
		patrolTimer = GetNode<Timer>("waitTimer");
		player = GetTree().GetNodesInGroup("Player")[0] as Player;
		playerCamera = player.GetNode<Camera3D>("Camera3d");

		waypoints = GetTree().GetNodesInGroup("EnemyWaypoint").Select(saar => saar as Marker3D).ToList();
		NavigationAgent.TargetPosition = waypoints[0].GlobalPosition;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		checkItemsForSound();

        switch (CurrentState)
		{
			case States.Patrol:
				if (NavigationAgent.IsNavigationFinished())
				{
					patrolTimer.Start();
					CurrentState = States.Waiting;
					return;
				}

				moveTowardsPoint(delta, patrolSpeed);

				break;
			case States.Chasing:
				if (NavigationAgent.IsNavigationFinished())
				{
					GD.Print("Attacking!");
					CurrentState = States.Waiting;
					patrolTimer.Start();
					return;
				}
				NavigationAgent.TargetPosition = player.GlobalPosition;
				moveTowardsPoint(delta, chaseSpeed);
				break;
			case States.Hunting:
				if (NavigationAgent.IsNavigationFinished())
				{
					CurrentState = States.Waiting;
					patrolTimer.Start();
					return;
				}
				moveTowardsPoint(delta, patrolSpeed);
				break;
			case States.Waiting:
                checkForPlayer();
                break;
			default:
				break;
		}
	}
	/// <summary>
	/// Moves the enemy towards the next nav point
	/// </summary>
	/// <param name="delta">the games delta speed</param>
	/// <param name="speed">the speed to move the enemy</param>
	private void moveTowardsPoint(double delta, float speed)
	{
		var targetPos = NavigationAgent.GetNextPathPosition();
		var direction = GlobalPosition.DirectionTo(targetPos);
		Vector3 lookingDirection = lastLookingDirection.Lerp(targetPos, .2f);
		LookAt(new Vector3(lookingDirection.X, GlobalPosition.Y, lookingDirection.Z), Vector3.Up);
		lastLookingDirection = lookingDirection;
		Velocity = direction * speed;
		MoveAndSlide();
		if (playerInEarshotFar)
			checkForPlayer();
	}

	private void checkItemsForSound()
	{
		foreach (var item in soundObjects)
		{
			if(item is Player)
			{
				Player player = (Player)item;
				if(isSoundLoudEnough(player.NoiseValue, checkIfObjectIsBehindWall(player), player.GlobalPosition))
				{
					CurrentState = States.Hunting;
					NavigationAgent.TargetPosition = player.GlobalPosition;

				}
			}
			if (item is Interactable)
			{
				Interactable interactable = (Interactable)item;
                if (isSoundLoudEnough(interactable.GetSound(), checkIfObjectIsBehindWall(item), item.GlobalPosition))
                {
                    CurrentState = States.Hunting;
                    NavigationAgent.TargetPosition = item.GlobalPosition;

                }
            }
		}
	}

	private bool isSoundLoudEnough(double noise, bool behindWall, Vector3 position)
	{
        if (((behindWall ?
            noise / 2 :
            noise) / GlobalPosition.DistanceTo(position) > CloseSoundDetect) || 
			((behindWall ? 
			noise / 2 :
            noise) / GlobalPosition.DistanceTo(position) > FarSoundDetect))
        {
			return true;
        }
		return false;
    }

	private bool checkIfObjectIsBehindWall(Node3D obj)
	{
        PhysicsDirectSpaceState3D spaceState3D = GetWorld3D().DirectSpaceState;
        var result = spaceState3D.IntersectRay(new PhysicsRayQueryParameters3D()
        {
            From = head.GlobalPosition,
            To = obj.GlobalPosition,
            Exclude = { this.GetRid() }
        });

        bool objBehindWall = true;

		if (result.Keys.Count > 0)
		{
			Node3D node = (Node3D)result["collider"];

			if (node is Player || node is Interactable)
			{
				objBehindWall = false;
			}
		}
		return objBehindWall;
	}

	/// <summary>
	/// Raycasts for player and checks if player is in rage and seeable
	/// sets state to chase or hunt.
	/// </summary>
	private void checkForPlayer()
	{
		PhysicsDirectSpaceState3D spaceState3D = GetWorld3D().DirectSpaceState;
		var result = spaceState3D.IntersectRay(new PhysicsRayQueryParameters3D()
		{
			From = head.GlobalPosition,
			To = playerCamera.GlobalPosition,
			Exclude =  { this.GetRid() }
		});

		bool playerBehindWall = true;

		if (result.Keys.Count > 0)
		{
			Node3D node = (Node3D)result["collider"];

			if (node is Player)
			{
				Player p = node as Player;

				playerBehindWall = false;
				if (playerInSightClose && (p.LightValue > CloseLightDetect || (p.IsCrouched && p.LightValue > CloseCrouchedLightDetect)))
				{

					CurrentState = States.Chasing;
					GD.Print("Player In Sight Close");
				}
				GD.Print(p.LightValue);
				if (playerInSightFar && (p.LightValue > FarLightDetect || (p.IsCrouched && p.LightValue > FarCrouchedLightDetect)))
				{
					CurrentState = States.Hunting;
					NavigationAgent.TargetPosition = player.GlobalPosition;
					GD.Print("Player is hunted");
				}
			}
		}

        
    }

	/// <summary>
	/// Signal for wait timer for patroling.
	/// </summary>
	private void _on_wait_timer_timeout()
	{
		waypointIndex += 1;
		if (waypointIndex > waypoints.Count - 1)
		{
			waypointIndex = 0;
		}
		NavigationAgent.TargetPosition = waypoints[waypointIndex].GlobalPosition;
		CurrentState = States.Patrol;
	}

	/// <summary>
	/// Signal for when player is in hearing range close to enemey
	/// </summary>
	/// <param name="obj">the object that is in range</param>
	private void _on_close_hearing_body_entered(Node3D obj)
	{
		if (obj is Player || obj is Interactable)
		{
			if (!soundObjects.Contains(obj))
			{
				soundObjects.Add(obj);
			}
		}
	}
	/// <summary>
	/// signal for when the player has left hearing range close to enemy
	/// </summary>
	/// <param name="obj">the object that has left the range</param>
	private void _on_close_hearing_body_exited(Node3D obj)
	{
		if (obj is Player || obj is Interactable)
		{
			if (soundObjects.Contains(obj))
			{
				soundObjects.Remove(obj);
			}
		}

	}
	/// <summary>
	/// signal that fires when the player has entered the far hearing range.
	/// </summary>
	/// <param name="obj">the object that has entered the far hearing range.</param>
	private void _on_far_hearing_body_entered(Node3D obj)
	{
        if (obj is Player || obj is Interactable)
        {
            if (!soundObjects.Contains(obj))
            {
                soundObjects.Add(obj);
            }
        }
    }
	/// <summary>
	/// Signal for when the player has left the far hearing range.
	/// </summary>
	/// <param name="obj">the obj that has left far hearing range.</param>
	private void _on_far_hearing_body_exited(Node3D obj)
	{
        if (obj is Player || obj is Interactable)
        {
            if (soundObjects.Contains(obj))
            {
                soundObjects.Remove(obj);
            }
        }

    }
	/// <summary>
	/// The signal for when the player has entered close sight range.
	/// </summary>
	/// <param name="obj">The obj that has entered close sight range.</param>
	private void _on_close_sight_body_entered(Node3D obj)
	{
		if (obj is Player)
			playerInSightClose = true;
	}
	/// <summary>
	/// The signal for when the player has left close sight range.
	/// </summary>
	/// <param name="obj">the obj that has left close sight range</param>
	private void _on_close_sight_body_exited(Node3D obj)
	{
		if (obj is Player)
			playerInSightClose = false;

	}
	/// <summary>
	/// The signal for when the player has entered far sight range
	/// </summary>
	/// <param name="obj">the obj that has entered far sight range</param>
	private void _on_far_sight_body_entered(Node3D obj)
	{
		if (obj is Player)
			playerInSightFar = true;
	}
	/// <summary>
	/// The signal for when the player has exited far sight range.
	/// </summary>
	/// <param name="obj">the obj that has exited far sight range</param>
	private void _on_far_sight_body_exited(Node3D obj)
	{
		if (obj is Player)
			playerInSightFar = false;
	}

}
