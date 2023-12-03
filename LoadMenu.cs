using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class LoadMenu : Control
{
	[Export]
	private PackedScene LoadButton;

	private string saveToLoad;

	public event EventHandler LoadingLevel;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DirAccess baseDir = DirAccess.Open("user://");
		if (!baseDir.DirExists("SavedGames"))
			baseDir.MakeDir("SavedGames");
		
		string[] dirs = DirAccess.GetDirectoriesAt("user://SavedGames");
		foreach (var dir in dirs)
		{
			LoadButton button = LoadButton.Instantiate<LoadButton>();
			button.LoadButtonDown += OnLoadButtonDown;

			FileAccess file = FileAccess.Open($"user://SavedGames/{dir}/{dir}_LoadScreen.json", FileAccess.ModeFlags.Read);
			string content = file.GetAsText();
			Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

			button.SetUpButton(data);

			button.Text = data["name"];
			GetNode<VBoxContainer>("Panel/ScrollContainer/VBoxContainer").AddChild(button);
		}
	}

    private void OnLoadButtonDown(string name, string date, string imagePath)
    {
        GetNode<RichTextLabel>("SaveName").Text = name;
        GetNode<RichTextLabel>("SaveDate").Text = date;
		saveToLoad = name;
        GetNode<TextureRect>("TextureRect").Texture = LoadImageTexture(imagePath);
    }

    private Texture2D LoadImageTexture(string imagePath)
    {
        var loadedImage = new Image();
		var error = loadedImage.Load(imagePath);

		if(error != Error.Ok){
			GD.Print("error loading image");
			return null;
		}

		return ImageTexture.CreateFromImage(loadedImage);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.

    public override void _Process(double delta)
	{
	}

	public void _on_back_button_down(){
		Hide();
	}

	public void _on_load_button_down(){
		var obj = SaveLoadManager.LoadGame(saveToLoad);
		GameManager.Instance.LoadLevel(obj["LoadedLevel"], int.Parse(obj["SpawnIndex"]));

		LoadingLevel?.Invoke(this, EventArgs.Empty);
		GameManager.Instance.PauseGame(false);
		QueueFree();
	}
}
