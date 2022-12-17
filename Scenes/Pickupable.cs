using Godot;
using GodotHorrorGameCSharp.Scripts;
using System;

public partial class Pickupable : RigidBody3D, Interactable
{
	[Export]
	public string HoverOverText;
	[Export]
	public InteractableResouce ItemResource;
	private AudioStreamPlayer3D audioStreamPlayer3D;
	private double noiseValue;
	private float currentCount = .2f;
	private float resetCount = .2f;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		BodyEntered += Pickupable_BodyEntered;
		audioStreamPlayer3D = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");

    }

	private void Pickupable_BodyEntered(Node body)
	{
		audioStreamPlayer3D.Stream = ItemResource.HitSoundWAV;
		audioStreamPlayer3D.Play();
		noiseValue = ItemResource.NoiseLevel;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(noiseValue > 0)
		{
			currentCount -= (float)delta;
			if(currentCount <= 0)
			{
				noiseValue = 0;
				currentCount = resetCount;
			}
		}
	}

	public string GetInterfaceText()
	{
		return HoverOverText;
	}

	public void Interact()
	{
		Player.player.GrabObject(this);
	}

	public double GetSound()
	{
		return noiseValue;
	}
}
