using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Inventory : Control
{
<<<<<<< Updated upstream:Scenes/Inventory.cs

    public static readonly Dictionary<string, Item> Items = new Dictionary<string, Item>()
    {
        { "null",null }
    };

    public static int? currentIndex = null;
    public static readonly string ItemButtonPath = "res://InventoryButton.tscn";
    private GridContainer gridContainer;
    private List<Item> items = new List<Item>();
    public int Capacity { get; set; } = 20;
=======
    /// <summary>
    /// The possible states of the inventory system.
    /// </summary>
    public enum States
    {
        inventory,
        investigation,
        crafting
    }

    /// <summary>
    /// The grid container that holds the inventory buttons.
    /// </summary>
    private GridContainer gridContainer;

    /// <summary>
    /// The packed scene that represents the inventory button.
    /// </summary>
    private PackedScene inventoryButton;

    /// <summary>
    /// The path to the inventory button scene file.
    /// </summary>
    [Export]
    private string itemButtonPath = "res://addons/FPCGI/InventorySystem/Scenes/inventory_button.tscns";
    
    [Export]
    public string CraftingMenu = "res://addons/FPCGI/InventorySystem/Scenes/crafting_menu.tscn";

    /// <summary>
    /// The capacity of the inventory in terms of number of items.
    /// </summary>
    [Export]
    public int Capacity { get; set; } = 24;

    /// <summary>
    /// The inventory button that is currently being grabbed by the mouse.
    /// </summary>
>>>>>>> Stashed changes:Addons/FP/InventorySystem/Scripts/Inventory.cs
    public InventoryButton GrabbedObject { get; set; }

    /// <summary>
    /// The inventory button that is currently being hovered over by the mouse.
    /// </summary>
    public InventoryButton HoverOverButton { get; set; }
<<<<<<< Updated upstream:Scenes/Inventory.cs
    private Vector2 lastClickedMousePos { get; set; }
=======

    /// <summary>
    /// The last position where the mouse was clicked in 2D space.
    /// </summary>
    private Vector2 lastMouseClickedPos;

    /// <summary>
    /// The current state of the inventory system.
    /// </summary>
    private States currentState = States.inventory;

    /// <summary>
    /// The list of items that are in the inventory.
    /// </summary>
    public List<Item> items = new List<Item>();

    [Export]
    private Item[] initialItems;
    /// <summary>
    /// A flag that indicates whether the mouse is over the trash icon or not.
    /// </summary>
>>>>>>> Stashed changes:Addons/FP/InventorySystem/Scripts/Inventory.cs
    private bool overTrash;

    public static Inventory GlobalInventory;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
<<<<<<< Updated upstream:Scenes/Inventory.cs
        gridContainer = GetNode<GridContainer>("ScrollContainer/GridContainer");
=======
        if(GlobalInventory == null){
            GlobalInventory = this;
        }
        gridContainer = GetNode<GridContainer>("InventoryMenu/Panel/ScrollContainer/GridContainer");
        inventoryButton = ResourceLoader.Load<PackedScene>(itemButtonPath);
>>>>>>> Stashed changes:Addons/FP/InventorySystem/Scripts/Inventory.cs
        populateButtons();

        if(initialItems != null){
            foreach (var item in initialItems)
            {
                Add(item);
            }
        }
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
<<<<<<< Updated upstream:Scenes/Inventory.cs
                GrabbedObject = HoverOverButton;
                lastClickedMousePos = GetTree().Root.GetMousePosition();
            }

            if (lastClickedMousePos.DistanceTo(GetTree().Root.GetMousePosition()) > 2)
            {
                if (Input.IsActionPressed("Throw"))
=======
                if (Input.IsActionJustPressed("LeftMouseButtonDown"))
>>>>>>> Stashed changes:Addons/FP/InventorySystem/Scripts/Inventory.cs
                {
                    InventoryButton button = GetNode<Area2D>("Area2D").GetNode<InventoryButton>("InventoryButton");
                    button.Show();
                    button.UpdateItem(GrabbedObject.CurrentItem, 0);
                }

                if (Input.IsActionJustReleased("Throw"))
                {
<<<<<<< Updated upstream:Scenes/Inventory.cs
                    if (overTrash)
=======
                    if (Input.IsActionPressed("LeftMouseButtonDown"))
>>>>>>> Stashed changes:Addons/FP/InventorySystem/Scripts/Inventory.cs
                    {
                        DeleteButton(GrabbedObject);
                    }
<<<<<<< Updated upstream:Scenes/Inventory.cs
                    else
=======
                    if (Input.IsActionJustReleased("LeftMouseButtonDown"))
>>>>>>> Stashed changes:Addons/FP/InventorySystem/Scripts/Inventory.cs
                    {
                        SwapButtons(GrabbedObject, HoverOverButton);
                        InventoryButton button = GetNode<Area2D>("Area2D").GetNode<InventoryButton>("InventoryButton");
                        button.Hide();
                    }
                }
            }
<<<<<<< Updated upstream:Scenes/Inventory.cs

        }
        if (Input.IsActionJustReleased("Throw"))
        {
            if (overTrash)
=======
            if (Input.IsActionJustReleased("LeftMouseButtonDown") && overTrash)
>>>>>>> Stashed changes:Addons/FP/InventorySystem/Scripts/Inventory.cs
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

<<<<<<< Updated upstream:Scenes/Inventory.cs
    public void _on_button_button_down()
=======
    /// <summary>
    /// Deletes an InventoryButton from the list of items and hides the button.
    /// </summary>
    /// <param name="inventoryButton">The InventoryButton to be deleted.</param>
    public void DeleteButton(InventoryButton inventoryButton)
>>>>>>> Stashed changes:Addons/FP/InventorySystem/Scripts/Inventory.cs
    {

        Add(ResourceLoader.Load<Item>("res://new_resource.tres"));
    }

<<<<<<< Updated upstream:Scenes/Inventory.cs
    public void _on_button_2_button_down()
=======
    //This method swaps two InventoryButton objects in a gridContainer. It takes two InventoryButton objects as parameters and uses their GetIndex() methods to get their indices in the gridContainer. It then uses the MoveChild() method to swap the two buttons in the gridContainer.
    /// <summary>
    /// Swaps the positions of two InventoryButtons in the gridContainer.
    /// </summary>
    /// <param name="button1">The first InventoryButton to be swapped.</param>
    /// <param name="button2">The second InventoryButton to be swapped.</param>
    private void swapButtons(InventoryButton button1, InventoryButton button2)
>>>>>>> Stashed changes:Addons/FP/InventorySystem/Scripts/Inventory.cs
    {
        Remove(ResourceLoader.Load<Item>("res://new_resource.tres"));
    }

    /// <summary>
    /// Adds an item to the inventory, stacking it if possible.
    /// </summary>
    /// <param name="item">The item to add.</param>
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
<<<<<<< Updated upstream:Scenes/Inventory.cs

=======
    /// <summary>
    /// Removes an item from the inventory if the player can afford it.
    /// </summary>
    /// <returns>Returns true if the item was removed, false otherwise.</returns>
>>>>>>> Stashed changes:Addons/FP/InventorySystem/Scripts/Inventory.cs
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

<<<<<<< Updated upstream:Scenes/Inventory.cs
    public void DeleteButton(InventoryButton inventoryButton)
    {
        items.Remove(inventoryButton.CurrentItem);
        reflowButtons();
        InventoryButton button = GetNode<Area2D>("Area2D").GetNode<InventoryButton>("InventoryButton");
        button.Hide();
    }

    public bool CanAfford(Item item)
=======
    public bool Remove(Item item, int quantity)
    {
        if (CanAfford(item, quantity))
        {
            Item currentItem = item.Copy();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == currentItem.ID)
                {
                    if (items[i].Quantity - quantity < 0)
                    {
                        currentItem.Quantity -= items[i].Quantity;
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

    /// <summary>
    /// Checks if the user has enough of the item to purchase it.
    /// </summary>
    /// <returns>
    /// Returns true if the user has enough of the item, false otherwise.
    /// </returns>
    private bool canAfford(Item item)
>>>>>>> Stashed changes:Addons/FP/InventorySystem/Scripts/Inventory.cs
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

<<<<<<< Updated upstream:Scenes/Inventory.cs
    public void Clear()
=======
    public bool CanAfford(Item item, int quantity)
    {
        List<Item> currentItems = items.Where(x => x.ID == item.ID).ToList();

        int i = 0;
        foreach (var item1 in currentItems)
        {
            i += item.Quantity;
        }

        if (quantity <= i)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Refreshes the buttons in the container.
    /// </summary>
    private void reflowButtons()
>>>>>>> Stashed changes:Addons/FP/InventorySystem/Scripts/Inventory.cs
    {
        items.Clear();
    }

    /// <summary>
    /// Updates the button at the given index with the item from the items list.
    /// </summary>
    /// <param name="index">The index of the button to update.</param>
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

<<<<<<< Updated upstream:Scenes/Inventory.cs
    public void _on_area_2d_area_entered(Area2D area)
=======
    /// <summary>
    /// Adds a chest item to the inventory when the add button is pressed.
    /// </summary>
    public void _on_add_button_button_down()
    {
        Add(ResourceLoader.Load<Item>("res://ChestCombined.tres"));
    }

    /// <summary>
    /// Removes a chest item from the inventory when the remove button is pressed.
    /// </summary>
    public void _on_remove_button_button_down()
    {
        Remove(ResourceLoader.Load<Item>("res://ChestCombined.tres"));
    }

    /// <summary>
    /// Sets the hover over button to the inventory button that the mouse enters.
    /// </summary>
    /// <param name="area">The area that the mouse enters.</param>
    public void _on_mouse_area_2d_area_entered(Area2D area)
>>>>>>> Stashed changes:Addons/FP/InventorySystem/Scripts/Inventory.cs
    {
        
        Control button = area.GetParent<Control>();
        if(button is InventoryButton)
        {
            HoverOverButton = (InventoryButton)button;
        }
    }

    public void _on_area_2d_area_exited(Area2D area) => HoverOverButton = null;

<<<<<<< Updated upstream:Scenes/Inventory.cs
    public void _on_trash_area_entered(Area2D area)
=======
    /// <summary>
    /// Sets the hover over button to null when the mouse exits an area.
    /// </summary>
    /// <param name="area">The area that the mouse exits.</param>
    public void _on_mouse_area_2d_area_exited(Area2D area) => HoverOverButton = null;

    /// <summary>
    /// Sets the over trash flag to true when the mouse enters the trash area.
    /// </summary>
    /// <param name="area">The area that the mouse enters.</param>
    public void _on_trash_area_2d_area_entered(Area2D area) => overTrash = true;

    /// <summary>
    /// Sets the over trash flag to false when the mouse exits the trash area.
    /// </summary>
    /// <param name="area">The area that the mouse exits.</param>
    public void _on_trash_area_2d_area_exited(Area2D area) => overTrash = false;

    /// <summary>
    /// Adds a health potion item to the inventory when the add button is pressed.
    /// </summary>
    public void _on_add_button_2_button_down()
>>>>>>> Stashed changes:Addons/FP/InventorySystem/Scripts/Inventory.cs
    {
        overTrash = true;
    }

<<<<<<< Updated upstream:Scenes/Inventory.cs
    public void _on_trash_area_exited(Area2D area)
=======
    /// <summary>
    /// Removes a health potion item from the inventory when the remove button is pressed.
    /// </summary>
    public void _on_remove_button_2_button_down()
>>>>>>> Stashed changes:Addons/FP/InventorySystem/Scripts/Inventory.cs
    {
        overTrash = false;
    }

    public void _on_player_stats_2_button_down(){
        AddChild(ResourceLoader.Load<PackedScene>(CraftingMenu).Instantiate<Control>());
    }
}
