using Godot;
using System.Collections.Generic;
using System;
using System.Linq;

public partial class InventoryButton : Button
{
    public Item CurrentItem { get; set; }
    [Export]
    public string EmptyIcon = "res://UI/None.png";
    private TextureRect icon;
    private Label quantityLabel;
    private int index;

    public event EventHandler<ItemButtonEventArgs> ItemButtonClicked;
    public override void _Ready()
    {
        base._Ready();
        icon = GetNode<TextureRect>("TextureRect");
        quantityLabel = GetNode<Label>("Label");
    }

    public void OnPressed()
    {
        ItemButtonClicked.Invoke(null, new ItemButtonEventArgs(index, CurrentItem));
    }

    public void UpdateItem(Item item)
    {
        CurrentItem = item;

        if(CurrentItem!= null)
        {
            icon.Texture = ResourceLoader.Load<Texture2D>(EmptyIcon);
            quantityLabel.Text= string.Empty;
        }
        else
        {
            icon.Texture = item.Icon;
            quantityLabel.Text= item.Quantity.ToString();
        }
    }
}
