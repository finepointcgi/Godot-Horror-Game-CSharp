using Godot;
using System;

public partial class UIBase : Node
{

	[Export]
	public PackedScene PauseMenu;
	PauseMenu currentPauseMenu;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		 base._Ready();
        GameManager.Instance.UIBase = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void HideUI(){
		foreach (var item in GetChildren())
		{
			if(item is Control c){
				c.Hide();
			}
		}
	}

	public void ShowUI(){
		foreach (var item in GetChildren())
		{
			if(item is Control c){
				c.Show();
			}
		}
	}

	public void Pause(bool paused){
		if(paused){
			currentPauseMenu = PauseMenu.Instantiate<PauseMenu>();
			AddChild(currentPauseMenu);
		}else{
			if(currentPauseMenu != null){
				currentPauseMenu.QueueFree();
			}
		}
	}
}
