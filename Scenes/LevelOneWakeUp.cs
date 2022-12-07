using Godot;
using System;

public partial class LevelOneWakeUp : Node3D
{
	bool DoorOpen;
	
	private void _on_area_3d_body_entered(Node3D body) {
		if (!DoorOpen && body is Player)
		{
            GetNode<Node3D>("%ChildrensDoor").GetNode<AnimationPlayer>("AnimationPlayer").Play("OpenSlow");
            DoorOpen = true;
		}
	}
}
