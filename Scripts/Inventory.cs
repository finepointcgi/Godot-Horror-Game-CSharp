using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

public partial class Inventory : Control
{

	private GridContainer gridContainer;
	private PackedScene inventoryButton;
	[Export]
	private string itemButtonPath = "res://UI/inventory_button.tscn";
	[Export]
	public int Capacity { get; set; } = 24;

	public InventoryButton GrabbedObject { get; set; }
	public InventoryButton HoverOverButton { get; set; }
	private Vector2 lastMouseClickedPos;

	private List<Item> items = new List<Item>();

	private bool overTrash;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		gridContainer = GetNode<GridContainer>("Panel/ScrollContainer/GridContainer");
		inventoryButton = ResourceLoader.Load<PackedScene>(itemButtonPath);
		populateButtons();
	}

	private void populateButtons()
	{
		for(int i = 0; i < Capacity; i++)
		{
			InventoryButton currentInventoryButton = inventoryButton.Instantiate<InventoryButton>();
			gridContainer.AddChild(currentInventoryButton);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetNode<Area2D>("MouseArea2d").Position = GetTree().Root.GetMousePosition();
		if(HoverOverButton != null)
		{
			if (Input.IsActionJustPressed("Throw"))
			{
				GrabbedObject = HoverOverButton;
				lastMouseClickedPos = GetTree().Root.GetMousePosition();
			}

			if(lastMouseClickedPos.DistanceTo(GetTree().Root.GetMousePosition()) > 2)
			{
				if (Input.IsActionPressed("Throw"))
				{
					InventoryButton button = GetNode<Area2D>("MouseArea2d").GetNode<InventoryButton>("InventoryButton");
					button.Show();
					button.UpdateItem(GrabbedObject.InventoryItem, 0);
				}
				if (Input.IsActionJustReleased("Throw"))
                {
					if (overTrash)
					{
						DeleteButton(GrabbedObject);
					}
					else
					{
						swapButtons(GrabbedObject, HoverOverButton);
						InventoryButton button = GetNode<Area2D>("MouseArea2d").GetNode<InventoryButton>("InventoryButton");
						button.Hide();
					}
                }
            }
		}
		if(Input.IsActionJustReleased("Throw" ) && overTrash)
        {
            DeleteButton(GrabbedObject);

        }
    }

	public void DeleteButton(InventoryButton inventoryButton)
	{
		items.Remove(inventoryButton.InventoryItem);
		reflowButtons();
		InventoryButton button = GetNode<Area2D>("MouseArea2d").GetNode<InventoryButton>("InventoryButton");
		button.Hide();
    }

    private void swapButtons(InventoryButton button1, InventoryButton button2)
    {
        int buttonindex = button1.GetIndex();
        int button2index = button2.GetIndex();
        gridContainer.MoveChild(button1, button2index);
        gridContainer.MoveChild(button2, buttonindex);
    }

    public void Add(Item item) {
		Item currentItem = item.Copy();

		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].ID == currentItem.ID && items[i].Quantity != items[i].StackSize)
			{
				if (items[i].Quantity + currentItem.Quantity > items[i].StackSize)
				{
					items[i].Quantity = currentItem.StackSize;
					currentItem.Quantity = -(currentItem.Quantity - items[i].StackSize );
					UpdateButton(i);
				}
				else
				{
					items[i].Quantity += currentItem.Quantity;
					currentItem.Quantity = 0;
					UpdateButton(i);
				}
			}
        }

        if (currentItem.Quantity > 0)
        {
			if (currentItem.Quantity < currentItem.StackSize)
			{
				items.Add(currentItem);
				UpdateButton(items.Count - 1);
			}
			else
			{
				Item tempItem = currentItem.Copy();
				tempItem.Quantity = currentItem.StackSize;
				items.Add(tempItem);
				UpdateButton(items.Count - 1);
				currentItem.Quantity -= currentItem.StackSize;
				Add(currentItem);
			}
        }

    }
	public bool Remove(Item item)
	{
		if (canAfford(item))
		{
			Item currentItem = item.Copy();
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].ID == currentItem.ID)
				{
					if (items[i].Quantity - currentItem.Quantity < 0)
					{
						currentItem.Quantity -= items[i].Quantity;
						items[i].Quantity = 0;
						UpdateButton(i);
					}
					else
					{
						items[i].Quantity -= currentItem.Quantity;
						currentItem.Quantity = 0;
						UpdateButton(i);
					}
				}

				if (currentItem.Quantity <= 0)
				{
					break;
				}
			}
			items.RemoveAll(x => x.Quantity <= 0);
			if (currentItem.Quantity > 0)
			{
				Remove(currentItem);
			}
			reflowButtons();
			return true;
		}
		return false;
	}

	private bool canAfford(Item item)
	{
		List<Item> currentItems = items.Where(x => x.ID == item.ID).ToList();

		int i = 0;
		foreach (var item1 in currentItems)
		{
			i += item1.Quantity;
		}

		if (item.Quantity < i)
		{
			return true;
		}
		return false;
	}

	private void reflowButtons()
	{
		for (int i = 0; i < Capacity; i++)
		{
			UpdateButton(i);
		}
	}

	public void UpdateButton(int index)
	{
		if(items.ElementAtOrDefault(index) != null)
			gridContainer.GetChild<InventoryButton>(index).UpdateItem(items[index], index);
		else
			gridContainer.GetChild<InventoryButton>(index).UpdateItem(null, index);
    }

	public void _on_add_button_button_down()
	{
		Add(ResourceLoader.Load<Item>("res://HealthPotionMega.tres"));
	}
	
	public void _on_remove_button_button_down()
	{
		Remove(ResourceLoader.Load<Item>("res://HealthPotionMega.tres"));
	}

	public void _on_mouse_area_2d_area_entered(Area2D area)
	{
		Control button = area.GetParent<Control>();
		if(button is InventoryButton)
		{
			HoverOverButton = (InventoryButton)button;
		}
	}


	public void _on_mouse_area_2d_area_exited(Area2D area) => HoverOverButton = null;

	public void _on_trash_area_2d_area_entered(Area2D area) => overTrash = true;

	public void _on_trash_area_2d_area_exited(Area2D area) => overTrash = false;

	public void _on_add_button_2_button_down() {
        Add(ResourceLoader.Load<Item>("res://HealthPotion.tres"));
    }

    public void _on_remove_button_2_button_down()
	{
        Remove(ResourceLoader.Load<Item>("res://HealthPotion.tres"));
    }
}
