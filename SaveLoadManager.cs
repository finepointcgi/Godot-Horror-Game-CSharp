using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class SaveLoadManager
{

	private static string loadedGameName;
	public static async void SaveGame(string name){
		DirAccess dir = DirAccess.Open("user://");
		if (!dir.DirExists("SavedGames")){
			dir.MakeDir("SavedGames");
		}

		dir = DirAccess.Open("user://SavedGames");

		if(!dir.DirExists(name)){
			dir.MakeDir(name);
		}
		
		Dictionary<string, string> saveGameData = new Dictionary<string, string>
        {
            { "LoadedLevel", GameManager.Instance.LoadedLevel },
            { "SpawnIndex", GameManager.Instance.LoadedLevelIndex.ToString() }
        };

		string saveJson = JsonConvert.SerializeObject(saveGameData);

		FileAccess file = FileAccess.Open($"user://SavedGames/{name}/{name}.json", FileAccess.ModeFlags.Write);
		GD.Print(saveJson);
		file.StoreString(saveJson);
		file.Close();

		Dictionary<string, string> loadScreenInfo = new Dictionary<string, string>
        {
            { "name", name },
			{ "imgPath", $"user://SavedGames/{name}/{name}.png"},
            { "dateTime", Time.GetUnixTimeFromSystem().ToString() }
        };

		string loadScreenJson = JsonConvert.SerializeObject(loadScreenInfo);

		FileAccess loadScreenFile = FileAccess.Open($"user://SavedGames/{name}/{name}_LoadScreen.json", FileAccess.ModeFlags.Write);

		loadScreenFile.StoreString(loadScreenJson);
		loadScreenFile.Close();

		GameManager.Instance.UIBase.HideUI();
	
		
		await Task.Delay(100);
		var screenshot = GameManager.Instance.GetViewport().GetTexture().GetImage();
		screenshot.SavePng($"user://SavedGames/{name}/{name}.png");

		Dictionary<string,Dictionary<string,string>> sceneItemsToSave = new Dictionary<string, Dictionary<string, string>>();
		foreach (var item in GameManager.Instance.GetTree().GetNodesInGroup("Saveable"))
		{
			if(item is Saveable s){
				sceneItemsToSave.Add(item.GetPath(), s.Save());
			}
		}

		
		string sceneItemsToSaveJson = JsonConvert.SerializeObject(sceneItemsToSave);

		FileAccess sceneItemsToSaveFile = FileAccess.Open($"user://SavedGames/{name}/{name}_sceneItems.json", FileAccess.ModeFlags.Write);

		sceneItemsToSaveFile.StoreString(sceneItemsToSaveJson);
		sceneItemsToSaveFile.Close();

	}

	public static Dictionary<string, string> LoadGame(string name){
		FileAccess file = FileAccess.Open($"user://SavedGames/{name}/{name}.json", FileAccess.ModeFlags.Read);
		string content = file.GetAsText();
		loadedGameName = name;
		GameManager.Instance.LoadingFromSave = true;
		return JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
	}

	public static Dictionary<string,Dictionary<string,string>> LoadSceneData(){
		FileAccess file = FileAccess.Open($"user://SavedGames/{loadedGameName}/{loadedGameName}_sceneItems.json", FileAccess.ModeFlags.Read);
		string content = file.GetAsText();
		return JsonConvert.DeserializeObject<Dictionary<string,Dictionary<string,string>>>(content);
	}
}
