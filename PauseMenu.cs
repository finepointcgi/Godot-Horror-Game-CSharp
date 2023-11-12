using Godot;
using Newtonsoft.Json.Bson;
using System;

public partial class PauseMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_continue_button_down(){
		GameManager.Instance.PauseGame(false);
	}

	public void _on_save_button_down(){
		string date = Time.GetDatetimeStringFromSystem().ToString();
        date = date.Replace(":", "-");
        date = date.Replace("/", "-");
        SaveLoadManager.SaveGame("test " + date);
		GameManager.Instance.PauseGame(false);
	}

	public void _on_load_button_down(){
		GetNode<LoadMenu>("LoadMenu").Show();
	}

	public void _on_options_button_down(){
		
	}

	public void _on_exit_button_down(){
		GetTree().Quit();
	}
}
