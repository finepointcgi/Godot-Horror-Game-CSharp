using Godot;
using System;
using System.Collections.Generic;

public partial class Surface : CSGBox3D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public int SoundValue;
	[Export]
	public AudioStream WalkStreamWAV;
    [Export]
    public AudioStream JumpLandSteamWAV;
    [Export]
    public AudioStream JumpStreamWAV;
    [Export]
    public AudioStream RunStreamWAV;
    [Export]
    public AudioStream SneakStreamWAV;
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}