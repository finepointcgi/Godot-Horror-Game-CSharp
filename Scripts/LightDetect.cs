using Godot;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The player light level class used to determine how much light the player is in.
/// </summary>
public partial class LightDetect : Node3D
{
	/// <summary>
	/// The current light detects light level
	/// </summary>
	public double LightLevel { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Image image = GetNode<Godot.SubViewport>("SubViewportContainer/SubViewport").GetTexture().GetImage();
		List<float> lightnessList = new List<float>();
		for (int y = 0; y < image.GetHeight(); y++)
		{
			for (int x = 0; x < image.GetWidth(); x++)
			{
				Color pixel = image.GetPixel(x, y);
				float lightness = (pixel.R + pixel.G + pixel.B) / 3;
				lightnessList.Add(lightness);
			}
		}
		LightLevel = lightnessList.Average();

		GetNode<Camera3D>("SubViewportContainer/SubViewport/Camera3D").GlobalPosition = new Vector3(this.GlobalPosition.X, this.GlobalPosition.Y + .5f, GlobalPosition.Z);
	}
}
