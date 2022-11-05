using Godot;
using System;

public partial class AudioManagerTwo : Node3D
{
	[Export]
	public Godot.Collections.Dictionary<string, Surface> Sounds = new Godot.Collections.Dictionary<string, Surface>(); 
	public static AudioManagerTwo manager;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		manager = this;
		Surface s =	 new()
		{
			WalkStreamWAV = ResourceLoader.Load<AudioStream>("res://19292__martian__footstep-on-wood-foley.wav"),
			JumpLandSteamWAV = ResourceLoader.Load<AudioStream>("res://422857__ipaddeh__jump-landing-wood-01.wav")
		};

        Sounds.Add("Wood", s);
	}

	// Called every=po frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
