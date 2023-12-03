using Godot;
using System;

public partial class MainMenu : Control
{
	[Export]
	public string MainMenuFirstLevelLoad = "res://Levels/Level One WakeUp.tscn";

	[Export]
	public Control LoadMenu;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		GetNode<LoadMenu>("LoadMenu").LoadingLevel += OnLoadingLevel;
	}

    private void OnLoadingLevel(object sender, EventArgs e)
    {
	    QueueFree();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	private void _on_start_game_button_down()
	{
		GetNode<Button>("StartGame").Hide();
		GetNode<Button>("Load Game").Hide();
		GameManager.Instance.LoadLevel(MainMenuFirstLevelLoad, 0);
		QueueFree();
	}

	private void _on_load_game_button_down(){
		GetNode<Control>("LoadMenu").Show();
	}
}
