using Godot;
using System;

public partial class InventoryButton : Button
{
	public enum InventoryButtonType
	{
		Inventory,
		Craftable
	}
	public Item InventoryItem;
	private TextureRect icon;
	private Label quantityLabel;
	private Label nameLabel;
	private int index;

	public InventoryButtonType CurrentInventoryButtonType;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		icon = GetNode<TextureRect>("TextureRect");
		quantityLabel = GetNode<Label>("Label");
        Pressed += InventoryButton_Pressed;
	}

    private void InventoryButton_Pressed()
    {
		if (InventoryItem != null)
		{
			if(CurrentInventoryButtonType == InventoryButtonType.Inventory){
				InventoryItem.UseItem();
			}else if(CurrentInventoryButtonType == InventoryButtonType.Craftable){
				InventoryItem.CraftItem();
			}
		}
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	public void UpdateItem(Item item, int index, InventoryButtonType type)
	{
		icon = GetNode<TextureRect>("TextureRect");
		quantityLabel = GetNode<Label>("Label");
		nameLabel = GetNode<Label>("NameLabel");
		this.index = index;
		InventoryItem = item;
		if (item == null)
		{
			icon.Texture = null;
			quantityLabel.Text = string.Empty;
			nameLabel.Text = string.Empty;
		}
		else { 
			icon.Texture = item.Icon;
			quantityLabel.Text = item.Quantity.ToString();
			nameLabel.Text = item.Name;
			
		}

		CurrentInventoryButtonType = type;
	}

}
