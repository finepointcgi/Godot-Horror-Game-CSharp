using Godot;
using System;

public partial class Surface : CSGBox3D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public int SoundValue;
	[Export]
	public AudioStreamWAV AudioStreamWAV;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
