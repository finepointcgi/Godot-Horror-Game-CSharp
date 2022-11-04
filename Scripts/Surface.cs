using Godot;
using System;
using System.Collections.Generic;

public partial class Surface : CSGBox3D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public int SoundValue;
	[Export]
	public Godot.Collections.Array<AudioStream> WalkStreamWAV;
    [Export]
    public Godot.Collections.Array<AudioStream> JumpLandSteamWAV;
    [Export]
    public Godot.Collections.Array<AudioStream> JumpStreamWAV;
    [Export]
    public Godot.Collections.Array<AudioStream> RunStreamWAV;
    [Export]
    public Godot.Collections.Array<AudioStream> SneakStreamWAV;
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
