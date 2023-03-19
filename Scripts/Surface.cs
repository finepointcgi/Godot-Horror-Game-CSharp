using Godot;
using System;
/// <summary>
/// The main surface refrence class
/// </summary>
public partial class Surface : CsgMesh3D
{
	/// <summary>
	/// Holds the resource used to determine the surface type
	/// </summary>
	[Export]
	public SurfaceResource SurfaceResource { get; set; }
}
