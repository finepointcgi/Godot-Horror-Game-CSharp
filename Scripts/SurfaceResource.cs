using Godot;
using System;
/// <summary>
/// A resource used to describe our surfaces
/// </summary>
public partial class SurfaceResource : Resource
{
    /// <summary>
    /// The noise level the surface has when walking on it
    /// </summary>
    [Export]
    public int NoiseLevel;
    /// <summary>
    /// The noise level a surface has when jumping into it
    /// </summary>
    [Export]
    public int JumpLandNoiseLevel;
    /// <summary>
    /// The sound a surface makes when walked on
    /// </summary>
    [Export]
    public AudioStream WalkStreamWAV;
    /// <summary>
    /// The sound a surface makes when jumped on
    /// </summary>
    [Export]
    public AudioStream JumpLandStreamWAV;
    /// <summary>
    /// The sound a surface makes when sneaked on
    /// </summary>
    [Export]
    public AudioStream SneakStreamWAV;

}
