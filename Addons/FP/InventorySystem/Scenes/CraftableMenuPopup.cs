using Godot;
using System;

public partial class CraftableMenuPopup : Control
{
	[Export]
	public PackedScene componentListObject;

	public void SetUpMenu(Item item){
		this.Show();
		GetNode<Label>("ItemName").Text = item.Name;
		//GetNode<Label>("WeightValue").Text = item.;
		foreach (var child in GetNode<VBoxContainer>("VBoxContainer").GetChildren())
		{
			child.QueueFree();
		}
		foreach (var craftableComponent in item.ItemCraftableMakeup)
		{
			ComponentListObject c = componentListObject.Instantiate() as ComponentListObject;
			Item component = craftableComponent.Key as Item;
			c.SetupListObject(component.Name, craftableComponent.Value);
			GetNode<VBoxContainer>("VBoxContainer").AddChild(c);
		}
	}

}
