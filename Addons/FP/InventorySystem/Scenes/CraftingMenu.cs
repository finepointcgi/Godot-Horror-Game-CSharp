using Godot;
using System;
using System.Collections.Generic;

public partial class CraftingMenu : Control
{

	[Export]
	public Item[] CraftableItems;
	[Export]
	public PackedScene craftableButton;
	private GridContainer gridContainer;
	[Export]
	private string craftButtonPath;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gridContainer = GetNode<GridContainer>("ScrollContainer/GridContainer");
		//craftableButton = ResourceLoader.Load<PackedScene>(craftButtonPath);
		
		foreach (var item in CraftableItems)
		{
			Add(item);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
    /// Adds an item to the inventory, stacking it if possible.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public async void Add(Item item)
    {
       
        for (int i = 0; i < CraftableItems.Length; i++)
        {
            InventoryButton currentInventoryButton = craftableButton.Instantiate<InventoryButton>();
			currentInventoryButton.SetupButton(item, InventoryButton.InventoryButtonState.Craftable);
            gridContainer.AddChild(currentInventoryButton);
        }
    

    }
}
