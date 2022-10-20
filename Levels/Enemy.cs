using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Enemy : CharacterBody3D
{
    public enum States
	{
		patrol,
		chasing,
		hunting,
		waiting

	}
    public States currentState;
    public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	NavigationAgent3D NavigationAgent3D;
	private List<Marker3D> waypoints = new();
	private int waypointIndex = 0;
	private Timer patrolTimer;
	private Vector3 lastLookingDirection;

	private bool playerInEarshotFar;
	private bool playerInEarshotClose;
	private bool playerInSightFar;
	private bool playerInSightClose;

	private Player player;
    public override void _Ready()
	{
		base._Ready();
		NavigationAgent3D = GetNode<NavigationAgent3D>("NavigationAgent3d");
		NavigationAgent3D.Connect("velocity_computed", new Callable(this, "onVelocityComputed"));
		
		currentState = States.patrol;
		NavigationAgent3D.SetTargetLocation(GetNode<Marker3D>("../EnemyPos").Position);
		
		waypoints = GetTree().GetNodesInGroup("EnemyPatrolPos").Select(saar => saar as Marker3D).ToList();

		player = GetTree().GetNodesInGroup("Player")[0] as Player;
		patrolTimer = GetNode<Timer>("PatrolTimer");
	}
	public override void _PhysicsProcess(double delta)
	{
		

		switch (currentState)
		{
			case States.patrol:
                if (NavigationAgent3D.IsNavigationFinished())
                {
					patrolTimer.Start();
					currentState = States.waiting;
                    return;
                }
                var targetpos = NavigationAgent3D.GetNextLocation();
                var direction = GlobalPosition.DirectionTo(targetpos);
                //GD.Print(direction);
                var velocity = direction * 2;
                //NavigationAgent3D.SetVelocity(velocity);
				faceDirection(targetpos, delta);
                Velocity = velocity;
                MoveAndSlide();
				if (playerInEarshotFar)
				{
					CheckForPlayer(playerInSightClose, playerInSightFar, playerInEarshotClose, playerInEarshotFar);
				}
				break;
			case States.chasing:
                var targetposchase = NavigationAgent3D.GetNextLocation();
                var directionchase = GlobalPosition.DirectionTo(targetposchase);
                //GD.Print(directionchase);
                var velocitychase = directionchase * NavigationAgent3D.MaxSpeed;
				//NavigationAgent3D.SetVelocity(velocity);
				NavigationAgent3D.SetTargetLocation(player.Position);
                Velocity = velocitychase;
                MoveAndSlide();
				break;
			case States.hunting:
			 	var targetposhunting = NavigationAgent3D.GetNextLocation();
                var directionhunting = GlobalPosition.DirectionTo(targetposhunting);
                //GD.Print(directionhunting);
                var velocityhunting = directionhunting * 2;
				//NavigationAgent3D.SetVelocity(velocity);
				NavigationAgent3D.SetTargetLocation(player.Position);
                Velocity = velocityhunting;
                MoveAndSlide();
				break;
			case States.waiting:
				break;
            default:
				break;
		}
		
	}

	private void CheckForPlayer(bool closeSight, bool farSight, bool closeSound, bool farSound)
	{
		PhysicsDirectSpaceState3D spaceState = GetWorld3d().DirectSpaceState;
		var result = spaceState.IntersectRay(new PhysicsRayQueryParameters3D()
		{
			From = GetNode<Node3D>("Head").GlobalPosition,
			To = player.GetNode<Camera3D>("Camera3d").GlobalPosition,
			Exclude = new Godot.Collections.Array { this }
		});

		if (result.Keys.Count > 0)
		{
			Node3D s = ((Node3D)result["collider"]) as Node3D;

			if (s is Player)
			{
                
                Player p = s as Player;
				if (closeSound)
				{
					if (p.NoiseLevel > 1)
					{
						GD.Print("I heard you Close");
						currentState = States.hunting;
						NavigationAgent3D.SetTargetLocation(player.Position);

					}
				}
                GD.Print(p.NoiseLevel);
                GD.Print(p.LightLevel);
				if (closeSight)
				{
					if(p.LightLevel > .2)
					{
                        GD.Print("I saw you CLOSE");
						currentState = States.chasing;
						NavigationAgent3D.SetTargetLocation(player.Position);
                    }
				}
				if (farSight)
				{
                    if (p.LightLevel > .6)
                    {
                        GD.Print("I saw you FAR!");
						currentState = States.chasing;
                    }
                }
				if (farSound)
				{
					if (p.NoiseLevel > 2)
					{
                        GD.Print("I heard you FAR");
						currentState = States.hunting;
                    }
				}
			}
		}
	}

	private void faceDirection(Vector3 direction, double delta)
	{
		//GD.Print(direction);
		Vector3 d = lastLookingDirection.Lerp(direction, .2f);
		LookAt(new Vector3(d.x, GlobalPosition.y, d.z), Vector3.Up);
		lastLookingDirection = d;
	}

	private void _on_patrol_timer_timeout()
	{
		
        waypointIndex += 1;
        if (waypointIndex > waypoints.Count - 1)
        {
            waypointIndex = 0;
        }
        NavigationAgent3D.SetTargetLocation(waypoints[waypointIndex].GlobalPosition);
		currentState = States.patrol;
    }

	private void onVelocityComputed(Vector3 velocity)
	{
		Velocity = velocity;
		MoveAndSlide();
	}

	private void _on_timer_timeout()
	{
		//GetNode<Marker3D>("../EnemyPos").Position;
		//NavigationAgent3D.SetTargetLocation(GetNode<Marker3D>("../EnemyPos").Position);
	}

	private void _on_fair_hearing_area_body_entered(Node3D obj)
	{
		if (obj is Player)
		{
			playerInEarshotFar = true;
			GD.Print("Player in earshot far");
		}
	}
	private void _on_fair_hearing_area_body_exited(Node3D obj)
	{
        if (obj is Player)
		{
			playerInEarshotFar = false;
		}  
    }
	private void _on_close_hearing_area_body_entered(Node3D obj)
	{
		if (obj is Player)
		{
			playerInEarshotClose = true;
		}
    }
	private void _on_close_hearing_area_body_exited(Node3D obj)
	{
		if (obj is Player) { 
			playerInEarshotClose = false;
		}
    }
	private void _on_far_seeing_area_body_entered(Node3D obj)
	{
		if (obj is Player)
		{
			playerInSightFar = true;
		}
    }
	private void _on_far_seeing_area_body_exited(Node3D obj)
	{
		if (obj is Player)
		{
			playerInSightFar = false;
		}
    }
	private void _on_close_seeing_area_body_entered(Node3D obj)
	{
		if (obj is Player)
		{
			playerInSightClose = true;
		}
    }
	private void _on_close_seeing_area_body_exited(Node3D obj)
	{
		if (obj is Player)
		{
			playerInSightClose = false;
		}
    }
}
