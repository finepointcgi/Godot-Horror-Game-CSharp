using Godot;
using System;
using System.Collections.Generic;

public partial class LoadButton : Button
{
	public string SaveName;
	public string Date;
	public string ImagePath;

	[Signal]
	public delegate void LoadButtonDownEventHandler(string name, string date, string imagePath);

	public void SetUpButton(Dictionary<string,string> data){
		SaveName = data["name"];
		Date = new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc).AddSeconds(double.Parse(data["dateTime"])).ToLocalTime().ToString();
		ImagePath = data["imgPath"];
	}

	public void _on_button_down(){
		EmitSignal(SignalName.LoadButtonDown, SaveName, Date, ImagePath);
	}

}
