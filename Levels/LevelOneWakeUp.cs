using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

public partial class LevelOneWakeUp : Node3D, Saveable
{
	private bool E_ChildroomDoorOpenEventPlayed;
	private bool E_ChildroomSoundPlayed;

	[Export]
	private AudioStream childRoomLaughFX;

    public override void _Ready()
    {
        base._Ready();

		if(GameManager.Instance.LoadingFromSave){
			LoadLevelData();
		}
    }
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

	public void LoadLevelData(){
		Dictionary<string, Dictionary<string,string>> levelData = SaveLoadManager.LoadSceneData();
		foreach (var item in GetTree().GetNodesInGroup("Saveable"))
		{
			GD.Print("loading level info");
			if(item is Saveable s){
				if(levelData.ContainsKey(item.GetPath())){
					s.Load(levelData[item.GetPath()]);
				}
			}
		}

		
	}

    public Dictionary<string, string> Save()
    {
        return new Dictionary<string, string>(){
			{"E_ChildroomDoorOpenEventPlayed",GD.VarToStr(E_ChildroomDoorOpenEventPlayed)},
			{"E_ChildroomSoundPlayed",GD.VarToStr(E_ChildroomSoundPlayed)}
		};
    }

    public void Load(Dictionary<string, string> data)
    {
       E_ChildroomDoorOpenEventPlayed = (bool)GD.StrToVar(data["E_ChildroomDoorOpenEventPlayed"]);
       E_ChildroomSoundPlayed = (bool)GD.StrToVar(data["E_ChildroomSoundPlayed"]);
    }

}
