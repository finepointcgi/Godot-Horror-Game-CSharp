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
        Image image = GetNode<SubViewport>("SubViewportContainer/SubViewport").GetTexture().GetImage();
        List<float> lightnessList = new List<float>();
        for (int y = 0; y < image.GetHeight(); y++)
        {
            for (int x = 0; x < image.GetWidth(); x++)
            {
                Color pixel = image.GetPixel(x, y);
                float lightness = (pixel.r + pixel.g + pixel.b) / 3;
                lightnessList.Add(lightness);
            }
        }
        LightLevel = lightnessList.Average();

        GetNode<Camera3D>("SubViewportContainer/SubViewport/Camera3D").GlobalPosition = new Vector3(this.GlobalPosition.x, this.GlobalPosition.y + .5f, GlobalPosition.z);
    }
}
