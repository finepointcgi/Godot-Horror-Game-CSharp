using Godot;
using System;

public partial class MainMenu : Control
{
	[Export]
	public string MainMenuFirstLevelLoad = "res://Levels/Level One WakeUp.tscn";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_start_game_button_down()
	{
		GetNode<Button>("StartGame").Hide();
		GetNode<LoadingScreen>("LoadingScreen").LoadLevel(MainMenuFirstLevelLoad, 0);
	}
}
