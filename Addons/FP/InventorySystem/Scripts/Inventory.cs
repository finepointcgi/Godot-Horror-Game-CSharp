using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
[GlobalClass]
public partial class Inventory : Control
{
    public enum States
    {
        inventory,
        investigation
    }
    private GridContainer gridContainer;
    private PackedScene inventoryButton;
    [Export]
    private string itemButtonPath = "res://UI/inventory_button.tscn";
    [Export]
    public int Capacity { get; set; } = 24;

    public InventoryButton GrabbedObject { get; set; }
    public InventoryButton HoverOverButton { get; set; }
    private Vector2 lastMouseClickedPos;
    private States currentState = States.inventory;

    private List<Item> items = new List<Item>();

    private bool overTrash;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        gridContainer = GetNode<GridContainer>("InventoryMenu/Panel/ScrollContainer/GridContainer");
        inventoryButton = ResourceLoader.Load<PackedScene>(itemButtonPath);
        populateButtons();

        if (GameManager.Inventory == null)
        {
            GameManager.Inventory = this;
        }
        else
        {
            QueueFree();
        }

    }

    private void populateButtons()
    {
        for (int i = 0; i < Capacity; i++)
        {
            InventoryButton currentInventoryButton = inventoryButton.Instantiate<InventoryButton>();
            gridContainer.AddChild(currentInventoryButton);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (currentState == States.inventory)
        {
            GetNode<Area2D>("MouseArea2d").Position = GetTree().Root.GetMousePosition();
            if (HoverOverButton != null)
            {
                if (HoverOverButton.CurrentInventoryButtonType == InventoryButton.InventoryButtonType.Inventory)
                {
                    if (Input.IsActionJustPressed("Throw"))
                    {
                        GrabbedObject = HoverOverButton;
                        lastMouseClickedPos = GetTree().Root.GetMousePosition();
                    }

                    if (lastMouseClickedPos.DistanceTo(GetTree().Root.GetMousePosition()) > 2)
                    {
                        if (Input.IsActionPressed("Throw"))
                        {
                            InventoryButton button = GetNode<Area2D>("MouseArea2d").GetNode<InventoryButton>("InventoryButton");
                            button.Show();
                            button.UpdateItem(GrabbedObject.InventoryItem, 0, InventoryButton.InventoryButtonType.Inventory);
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
                else if (HoverOverButton.CurrentInventoryButtonType == InventoryButton.InventoryButtonType.Craftable)
                {
                    GetNode<CraftableMenuPopup>("MouseArea2d/CraftableMenuPopup").SetUpMenu(HoverOverButton.InventoryItem);
                }
            }
            else
            {

                GetNode<CraftableMenuPopup>("MouseArea2d/CraftableMenuPopup").Hide();

            }
            if (Input.IsActionJustReleased("Throw") && overTrash)
            {
                DeleteButton(GrabbedObject);

            }
        }
        if (Input.IsActionJustPressed("RightMouseButtonDown"))
        {
            if (currentState == States.inventory)
            {
                if (HoverOverButton != null)
                {
                    GetNode<InvestigationBaseScene>("InvestigationBaseScene").ShowObject(HoverOverButton.InventoryItem);
                    GetNode<Control>("InventoryMenu").Hide();
                    currentState = States.investigation;
                }
            }
            else if (currentState == States.investigation)
            {
                GetNode<InvestigationBaseScene>("InvestigationBaseScene").Hide();
                GetNode<Control>("InventoryMenu").Show();
                currentState = States.inventory;
            }

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

    public void Add(Item item)
    {
        Item currentItem = item.Copy();

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == currentItem.ID && items[i].Quantity != items[i].StackSize)
            {
                if (items[i].Quantity + currentItem.Quantity > items[i].StackSize)
				{
					int remainingSpace = items[i].StackSize - items[i].Quantity;
					items[i].Quantity = items[i].StackSize;
					currentItem.Quantity -= remainingSpace;
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
    public bool Remove(Item item) => Remove(item, item.Quantity);

    public bool Remove(Item item, int quantity)
    {
        if (CanAfford(item))
        {
            Item currentItem = item.Copy();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == currentItem.ID)
                {
                    if (items[i].Quantity - quantity < 0)
                    {
                        quantity -= items[i].Quantity;
                        items[i].Quantity = 0;
                        UpdateButton(i);
                    }
                    else
                    {
                        items[i].Quantity -= quantity;
                        quantity = 0;
                        UpdateButton(i);
                    }
                }

                if (quantity <= 0)
                {
                    break;
                }
            }
            items.RemoveAll(x => x.Quantity <= 0);
            if (quantity > 0)
            {
                Remove(currentItem);
            }
            reflowButtons();
            return true;
        }
        return false;
    }

    public bool CanAfford(Item item) => CanAfford(item, item.Quantity);
    public bool CanAfford(Item item, int quantity)
    {
        List<Item> currentItems = items.Where(x => x.ID == item.ID).ToList();

        int i = 0;
        foreach (var item1 in currentItems)
        {
            i += item1.Quantity;
        }

        if (quantity <= i)
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
        if (items.ElementAtOrDefault(index) != null)
            gridContainer.GetChild<InventoryButton>(index).UpdateItem(items[index], index, InventoryButton.InventoryButtonType.Inventory);
        else
            gridContainer.GetChild<InventoryButton>(index).UpdateItem(null, index, InventoryButton.InventoryButtonType.Inventory);
    }

    public void _on_add_button_button_down()
    {
        Add(ResourceLoader.Load<Item>("res://Items/Iron.tres"));
    }

    public void _on_remove_button_button_down()
    {
        Remove(ResourceLoader.Load<Item>("res://Items/Iron.tres"));
    }

    public void _on_mouse_area_2d_area_entered(Area2D area)
    {
        Control button = area.GetParent<Control>();
        
        if (button is InventoryButton)
        {
            HoverOverButton = (InventoryButton)button;
        }
    }


    public void _on_mouse_area_2d_area_exited(Area2D area) => HoverOverButton = null;

    public void _on_trash_area_2d_area_entered(Area2D area) => overTrash = true;

    public void _on_trash_area_2d_area_exited(Area2D area) => overTrash = false;

    public void _on_add_button_2_button_down()
    {
        Add(ResourceLoader.Load<Item>("res://Items/GunPowder.tres"));
    }

    public void _on_remove_button_2_button_down()
    {
        Remove(ResourceLoader.Load<Item>("res://Items/GunPowder.tres"));
    }

    public void _on_save_game_button_down(){
        string date = Time.GetDatetimeStringFromSystem().ToString();
        date = date.Replace(":", "-");
        date = date.Replace("/", "-");
        SaveLoadManager.SaveGame("test " + date);
    }
}
