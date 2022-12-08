using Godot;
using System;
using GodotHorrorGameCSharp.Scripts;

public partial class LevelOneWakeUp : Node3D
{
	bool DoorOpen;
	bool childRoomPlay;
	[Export]
	private AudioStream childRoomLaughFX;	
	private void _on_area_3d_body_entered(Node3D body) {
		if (!DoorOpen && body is Player)
		{
			GetNode<Node3D>("%ChildrensDoor").GetNode<AnimationPlayer>("AnimationPlayer").Play("OpenSlow");
			GetNode<Door>("%ChildrensDoor").DoorOpen = true;
			DoorOpen = true;
		}
	}



private void _on_children_room_body_entered(Node3D body)
{
	if(!childRoomPlay){
		Player.player.PlayEffectSound(childRoomLaughFX, -13f);
		childRoomPlay = true;
		}
}
}
