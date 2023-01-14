using Godot;
/// <summary>
/// The main surface refrence class
/// </summary>
public partial class Surface : CSGMesh3D
{
    /// <summary>
    /// Holds the resource used to determine the surface type
    /// </summary>
    [Export]
    public SurfaceResource SurfaceResource { get; set; }
}
