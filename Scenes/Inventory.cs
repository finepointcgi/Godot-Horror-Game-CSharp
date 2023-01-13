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

	public static int? currentIndex = 0;
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

    private void OnButtonClicked(object sender, ItemButtonEventArgs e)
    {
        if(currentIndex == e.Index)
		{
			currentIndex = null;
			return;
		}

		if(currentIndex == null)
		{
			if (items[e.Index]!=null)
			{
				GD.Print("select Item");
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

    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	public void _on_button_button_down()
	{

		Add(ResourceLoader.Load<Item>("res://new_resource.tres"));
	}


    public void Add(Item item)
	{
		if(items.Contains(item))
		{
			List<Item> currentItemStacks = items.Where(x => x.ID == item.ID).ToList();
			foreach (var stack in currentItemStacks)
			{
				if (stack.Quantity >= stack.StackSize) continue;

				if(stack.Quantity + item.Quantity > stack.StackSize)
				{
					item.Quantity = item.Quantity - (stack.StackSize - stack.Quantity);
				}
			}
		}
        if (item.Quantity > 0)
        {
            items.Add(item);
        }
		
		
	}

	public void Remove(Item item)
	{
		if (items.Contains(item))
		{
			//items.Where
		}
	}
	
	public void Clear()
	{
		items.Clear();
	}
}
