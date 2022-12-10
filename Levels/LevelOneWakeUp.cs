using Godot;
using System;

public partial class LevelOneWakeUp : Node3D
{
	private bool E_ChildroomDoorOpenEventPlayed;
	private bool E_ChildroomSoundPlayed;

	[Export]
	private AudioStream childRoomLaughFX;
	public void _on_e_childroom_door_open_body_entered(Node3D body)
	{
		if(!E_ChildroomDoorOpenEventPlayed)
			GetNode<Node3D>("%ChildrenDoor").GetNode<AnimationPlayer>("AnimationPlayer").Play("SlowOpen");
		E_ChildroomDoorOpenEventPlayed = true;
	}

	public void _on_e_childroom_sound_body_entered(Node3D body)
	{
		if (!E_ChildroomSoundPlayed)
			Player.player.PlayNonPositionalSound(childRoomLaughFX, -30f);
		E_ChildroomSoundPlayed = true;
	}
}
