using Godot;
using System.Collections.Generic;
using System;
using System.Linq;

public partial class Inventory : Control
{
	
	public static readonly Dictionary<string, Item> Items = new Dictionary<string, Item>()
	{
		{ "null",null }
	};

	public static int? currentIndex = null;
	public static readonly string ItemButtonPath = "res://InventoryButton.tscn";
	private GridContainer gridContainer;
	private List<Item> items = new List<Item>();
	public int Capacity { get; set; } = 20;

	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gridContainer = GetNode<GridContainer>("ScrollContainer/GridContainer");
		populateButtons();
	}

	public void populateButtons()
	{
		for(int i = 0; i < Capacity; i++)
		{
            PackedScene packedScene = ResourceLoader.Load<PackedScene>(ItemButtonPath);
			InventoryButton itemButton = packedScene.Instantiate<InventoryButton>();
			itemButton.ItemButtonClicked += OnButtonClicked;
            itemButton._Ready();
            //itemButton.UpdateItem(item);

            gridContainer.AddChild(itemButton);
        }
	}

    public void reflowButtons()
    {
        for (int i = 0; i < Capacity; i++)
        {
            UpdateButton(i);
        }
    }

    private void OnButtonClicked(object sender, ItemButtonEventArgs e)
    {
        if(currentIndex == e.Index)
		{
			currentIndex = null;
			return;
		}

		if(currentIndex == null)
		{
			if (items[e.Index] != null)
			{
				currentIndex = e.Index;
				GD.Print("select Item for use or spawning");
			}
			return;
		}

		if(currentIndex != e.Index)
		{
			if (items[e.Index] == null)
			{
				items[e.Index] = items[currentIndex.Value];
				items[currentIndex.Value] = null;
			}
			else
			{
				items[currentIndex.Value] = items[e.Index].Stack(items[e.Index]);
			}
		}
		UpdateButton(currentIndex.Value);
		UpdateButton(e.Index);
		currentIndex = null;
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	public void _on_button_button_down()
	{

		Add(ResourceLoader.Load<Item>("res://new_resource.tres"));
	}

	public void _on_button_2_button_down()
	{
		Remove(ResourceLoader.Load<Item>("res://new_resource.tres"));
	}

    public void Add(Item item)
	{
        Item currentItem = item.Copy();
		int index = 0;

		for (int i = 0; i < items.Count; i++)
		{

			if (items[i].ID == currentItem.ID && items[i].Quantity != items[i].StackSize)
			{
				if (items[i].Quantity + currentItem.Quantity > items[i].StackSize)
				{
					items[i].Quantity = currentItem.StackSize;

                    currentItem.Quantity = -(currentItem.Quantity - items[i].StackSize);
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
			if(currentItem.Quantity < currentItem.StackSize){
                items.Add(currentItem);
                UpdateButton(items.Count - 1);
			}
			else
			{
				Item tempItem = currentItem;
				tempItem.Quantity = currentItem.StackSize;
                items.Add(currentItem);
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
		else { return false; }
		
    }
	
	private bool canAfford(Item item)
	{
		List<Item> currentItems = items.Where(x => x.ID == item.ID).ToList();

		int i = 0;
		foreach(Item item1 in currentItems)
		{
			i += currentItems.Count;
		}

		if(item.Quantity < i)
		{
			return true;
		}
		return false;
	}

	public void Clear()
	{
		items.Clear();
	}

    public void UpdateButton(int index)
    {
		if(items.ElementAtOrDefault(index) != null)
		{
			gridContainer.GetChild<InventoryButton>(index).UpdateItem(items[index], index);
		}
		else
		{
            gridContainer.GetChild<InventoryButton>(index).UpdateItem(null, index);

        }
    }
}
