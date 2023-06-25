using Godot;
using System;

public partial class CharacterMenu : Panel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetupCharacterMenu(){
		//GetNode<Label>("HBoxContainer/VBoxContainer2/Level").Text = GameManager.Instance.LocalCharacter.Stats.Level.ToString();
		//GetNode<Label>("HBoxContainer/VBoxContainer2/HP").Text = GameManager.Instance.LocalCharacter.Stats.Health.ToString();
		//GetNode<Label>("HBoxContainer/VBoxContainer2/MP").Text = GameManager.Instance.LocalCharacter.Stats.Health.ToString();
		//GetNode<Label>("HBoxContainer/VBoxContainer2/ST").Text = GameManager.Instance.LocalCharacter.Stats.Stamina.ToString();
		//GetNode<Label>("HBoxContainer/VBoxContainer2/Attunement").Text = GameManager.Instance.LocalCharacter.Stats.Attunement.ToString();
		//GetNode<Label>("HBoxContainer/VBoxContainer2/Endurance").Text = GameManager.Instance.LocalCharacter.Stats.Endurance.ToString();
		//GetNode<Label>("HBoxContainer/VBoxContainer2/Vitality").Text = GameManager.Instance.LocalCharacter.Stats.Vitality.ToString();
		//GetNode<Label>("HBoxContainer/VBoxContainer2/Strength").Text = GameManager.Instance.LocalCharacter.Stats.Strength.ToString();
		//GetNode<Label>("HBoxContainer/VBoxContainer2/Dexterity").Text = GameManager.Instance.LocalCharacter.Stats.Dexterity.ToString();
		//GetNode<Label>("HBoxContainer/VBoxContainer2/Intelligence").Text = GameManager.Instance.LocalCharacter.Stats.Intelligence.ToString();
		//GetNode<Label>("HBoxContainer/VBoxContainer2/Faith").Text = GameManager.Instance.LocalCharacter.Stats.Faith.ToString();
		//GetNode<Label>("HBoxContainer/VBoxContainer2/Luck").Text = GameManager.Instance.LocalCharacter.Stats.Humanity.ToString();
	}
}
