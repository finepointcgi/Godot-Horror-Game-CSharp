using Godot;
using System.Collections.Generic;
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
    public InventoryButton GrabbedObject { get; set; }
    public InventoryButton HoverOverButton { get; set; }
    private Vector2 lastClickedMousePos { get; set; }
    private bool overTrash;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        gridContainer = GetNode<GridContainer>("ScrollContainer/GridContainer");
        populateButtons();
    }

    public void populateButtons()
    {
        for (int i = 0; i < Capacity; i++)
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
        if (items.Count > 0)
        {
            if (items[e.Index] != null)
            {
                currentIndex = e.Index;
                e.Item.Use();
            }
            return;
        }
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        GetNode<Area2D>("Area2D").Position = GetTree().Root.GetMousePosition();
        if (HoverOverButton != null)
        {
            if (Input.IsActionJustPressed("Throw"))
            {
                GrabbedObject = HoverOverButton;
                lastClickedMousePos = GetTree().Root.GetMousePosition();
            }

            if (lastClickedMousePos.DistanceTo(GetTree().Root.GetMousePosition()) > 2)
            {
                if (Input.IsActionPressed("Throw"))
                {
                    InventoryButton button = GetNode<Area2D>("Area2D").GetNode<InventoryButton>("InventoryButton");
                    button.Show();
                    button.UpdateItem(GrabbedObject.CurrentItem, 0);
                }

                if (Input.IsActionJustReleased("Throw"))
                {
                    if (overTrash)
                    {
                        DeleteButton(GrabbedObject);
                    }
                    else
                    {
                        SwapButtons(GrabbedObject, HoverOverButton);
                        InventoryButton button = GetNode<Area2D>("Area2D").GetNode<InventoryButton>("InventoryButton");
                        button.Hide();
                    }
                }
            }

        }
        if (Input.IsActionJustReleased("Throw"))
        {
            if (overTrash)
            {
                DeleteButton(GrabbedObject);
            }
        }
        }

    private void SwapButtons(InventoryButton button1, InventoryButton button2)
    {
        //UpdateButton(items.FindIndex(x => x.Name == button1.Name);

        int button1Index = button1.GetIndex();
        int button2Index = button2.GetIndex();
        gridContainer.MoveChild(button1, button2Index);
        gridContainer.MoveChild(button2, button1Index);
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
            if (currentItem.Quantity < currentItem.StackSize)
            {
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
        if (CanAfford(item))
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

    public void DeleteButton(InventoryButton inventoryButton)
    {
        items.Remove(inventoryButton.CurrentItem);
        reflowButtons();
        InventoryButton button = GetNode<Area2D>("Area2D").GetNode<InventoryButton>("InventoryButton");
        button.Hide();
    }

    public bool CanAfford(Item item)
    {
        List<Item> currentItems = items.Where(x => x.ID == item.ID).ToList();

        int i = 0;
        foreach (Item item1 in currentItems)
        {
            i += currentItems.Count;
        }

        if (item.Quantity < i)
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
        if (items.ElementAtOrDefault(index) != null)
        {
            gridContainer.GetChild<InventoryButton>(index).UpdateItem(items[index], index);
        }
        else
        {
            gridContainer.GetChild<InventoryButton>(index).UpdateItem(null, index);

        }
    }

    public void _on_area_2d_area_entered(Area2D area)
    {
        
        Control button = area.GetParent<Control>();
        if(button is InventoryButton)
        {
            HoverOverButton = (InventoryButton)button;
        }
    }

    public void _on_area_2d_area_exited(Area2D area) => HoverOverButton = null;

    public void _on_trash_area_entered(Area2D area)
    {
        overTrash = true;
    }

    public void _on_trash_area_exited(Area2D area)
    {
        overTrash = false;
    }
}
