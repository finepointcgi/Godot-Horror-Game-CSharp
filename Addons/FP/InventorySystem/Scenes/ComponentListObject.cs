using Godot;
using System;

public partial class ComponentListObject : HBoxContainer
{
	public void SetupListObject(string name, int amount){
		GetNode<Label>("Name").Text = name;
		GetNode<Label>("Amount").Text = amount.ToString();

	}
}
