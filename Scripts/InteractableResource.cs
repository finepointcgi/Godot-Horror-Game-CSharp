using Godot;
using System;
/// <summary>
/// A resource used to describe our interactable Objects
/// </summary>
public partial class InteractableResource : Resource
{
    [Export]
    public AudioStream HitSoundWAV;
    /// <summary>
    /// The noise level the of the object when hitting something
    /// </summary>
    [Export]
    public int NoiseLevel;
}

