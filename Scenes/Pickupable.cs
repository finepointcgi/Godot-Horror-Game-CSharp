using Godot;
using GodotHorrorGameCSharp.Scripts;
using System;

public partial class Pickupable : RigidBody3D, Interactable
{
    [Export]
    public string HoverOverText;
	[Export]
	private InteractableResource ItemResource;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public string GetInterfaceText()
	{
        return HoverOverText;
    }

	public void PlayAnimation(string animation)
	{
		throw new NotImplementedException();
	}

	public void Interact()
	{
		//GetParent().RemoveChild(this);
		//Player.player.Attachable.AddChild(this);
		Player.player.GrabObject(this);
	}

	private void _on_grabbable_object_body_entered(Node3D body)
	{
		GD.Print("hit");
		if (!GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D").IsPlaying())
		{
			if (LinearVelocity.Length() > 1)
			{
				GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D").Stream = ItemResource.HitSoundWAV;

				GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D").Play();
			}
		}
	}
}
