using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LightDetect : Node3D
{

	public double LightLevel { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        LightLevel = getLightLevel();
		GetNode<Camera3D>("SubViewportContainer/f/Camera3d").GlobalPosition = new Vector3(GetNode<Node3D>("MeshInstance3d").GlobalPosition.x, GetNode<Node3D>("MeshInstance3d").GlobalPosition.y + .3f, GetNode<Node3D>("MeshInstance3d").GlobalPosition.z);

    }

	private double getLightLevel()
	{
		var image = new Image();
		image = GetNode<SubViewport>("SubViewportContainer/f").GetTexture().GetImage();
		var p0 = image.GetPixel(0, 0);
		var hl = (p0.r + p0.g + p0.b) / 3;//0.2126 * p0.r + 0.7152 * p0.g + 0.0722 * p0.b;
		List<float> floats = new List<float>();
        for (int y = 0; y < image.GetHeight(); y++)
		{
            for (int x = 0; x < image.GetHeight(); x++)
            {
				var p = image.GetPixel(x, y);
				var l = (p.r + p.g + p.b) / 3;//0.2126 * p.r + 0.7152* p.g + 0.0722 * p.b;
				//if(l > hl)
				{
				//	hl = l;
				}
				floats.Add(l);
            }
        }

		return floats.Average();
	}
}
