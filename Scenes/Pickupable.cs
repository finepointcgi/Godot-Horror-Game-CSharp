using Godot;
using GodotHorrorGameCSharp.Scripts;
using System;

public partial class Pickupable : RigidBody3D, Interactable
{
    [Export]
    public string HoverOverText;
    [Export]
    public InteractableResource ItemResource;
    private AudioStreamPlayer3D audioStreamPlayer3D;
    private double noiseValue;
    private float currentCount = .2f;
    private float resetCount = .2f;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BodyEntered += Pickupable_BodyEntered;
        audioStreamPlayer3D = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");

    }

    private void Pickupable_BodyEntered(Node body)
    {
        audioStreamPlayer3D.Stream = ItemResource.HitSoundWAV;
        audioStreamPlayer3D.Play();
        noiseValue = ItemResource.NoiseLevel;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }

    public string GetInterfaceText()
    {
        return HoverOverText;
    }

    public void PlayAnimation(string animation)
    {
        throw new NotImplementedException();
    }

    public void Interact()
    {
        //GetParent().RemoveChild(this);
        //Player.player.Attachable.AddChild(this);
        Player.player.GrabObject(this);
    }

    private void _on_grabbable_object_body_entered(Node3D body)
    {
        GD.Print("hit");
        if (!GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D").IsPlaying())
        {
            if (LinearVelocity.Length() > 1)
            {
                GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D").Stream = ItemResource.HitSoundWAV;

                GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D").Play();
            }
        }
    }
}
