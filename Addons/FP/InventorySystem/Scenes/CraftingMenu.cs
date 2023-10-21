using Godot;
using System;

public partial class CraftingMenu : Control
{
	[Export]
	public Item[] CraftableItems;
	[Export]
	public PackedScene CraftableButton;

	private GridContainer gridContainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gridContainer = GetNode<GridContainer>("ScrollContainer/GridContainer");

		foreach (var item in CraftableItems)
		{
			InventoryButton inventoryButton = CraftableButton.Instantiate<InventoryButton>();
			inventoryButton.UpdateItem(item, 0, InventoryButton.InventoryButtonType.Craftable);
			gridContainer.AddChild(inventoryButton);
		}
	}

	
}
