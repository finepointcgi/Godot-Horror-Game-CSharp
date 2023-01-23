using Godot;
using System;

public partial class InventoryButton : Button
{
	public Item InventoryItem;
	private TextureRect icon;
	private Label quantityLabel;
	private int index;

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
			InventoryItem.UseItem();
		}
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	public void UpdateItem(Item item, int index)
	{
		this.index = index;
		InventoryItem = item;
		if (item == null)
		{
			icon.Texture = null;
			quantityLabel.Text = string.Empty;
		}
		else { 
			icon.Texture = item.Icon;
			quantityLabel.Text = item.Quantity.ToString();
		}
	}

}
