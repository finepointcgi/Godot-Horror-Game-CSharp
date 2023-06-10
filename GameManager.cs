using Godot;
using System;
using System.Collections.Generic;
using System.Resources;

public partial class GameManager : Node
{
	public static GameManager Instance;
	[Export]
	public string LoadScene = "res://UI/LoadingScreen.tscn";

	public bool Paused;

	public Node LevelBase;

	public Player Player;
	public string PlayerScenePath = "res://Scenes/player.tscn";
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		if(Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree();
		}
	}

	public void LoadLevel(string path, int index)
	{
		PackedScene loadScene = ResourceLoader.Load<PackedScene>(LoadScene);
		LoadingScreen loadSceneNode = loadScene.Instantiate() as LoadingScreen;
		LevelBase.AddChild(loadSceneNode);
		loadSceneNode.LoadLevel(path, index);
	}

	public void CheckForPlayer()
	{
		if(Player == null)
		{
			var playerPackedScene = ResourceLoader.Load(PlayerScenePath) as PackedScene;
			Player = playerPackedScene.Instantiate() as Player;
			GetTree().GetNodesInGroup("PlayerBase")[0].AddChild(Player);
		}
	}

	public void MovePlayer(int index)
	{
		Godot.Collections.Array<Node> nodes = GetTree().GetNodesInGroup("SpawnIndex");

		foreach (var item in nodes)
		{
			if(item is SpawnIndex spawnIndex)
			{
				if(spawnIndex.Index == index)
				{
					Player.GlobalPosition = spawnIndex.GlobalPosition;
					Player.GlobalRotation = spawnIndex.GlobalRotation;
				}
			}
			
		}

	}
	
}
