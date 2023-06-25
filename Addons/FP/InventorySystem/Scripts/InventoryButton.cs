using Godot;

public partial class InventoryButton : Button
{
    public enum InventoryButtonState
    {
        Inventory,
        Craftable
    }
    /// <summary>
    /// The item that is displayed on the inventory button.
    /// </summary>
    public Item InventoryItem;

    /// <summary>
    /// The texture rect that shows the item's icon.
    /// </summary>
    private TextureRect icon;

    /// <summary>
    /// The label that shows the item's quantity.
    /// </summary>
    private Label quantityLabel;

    /// <summary>
    /// The index of the inventory button in the inventory.
    /// </summary>
    private int index;

    private InventoryButtonState currentState = InventoryButtonState.Inventory;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        icon = GetNode<TextureRect>("TextureRect");
        quantityLabel = GetNode<Label>("Label");
        Pressed += InventoryButton_Pressed;
    }

    /// <summary>
    /// Uses the inventory item when the inventory button is pressed.
    /// </summary>
    private void InventoryButton_Pressed()
    {
        if(currentState == InventoryButtonState.Inventory){
            if (InventoryItem != null)
            {
                InventoryItem.UseItem();
            }
        }else if(currentState == InventoryButtonState.Craftable){
            InventoryItem.CraftItem();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void SetupButton(Item Item, InventoryButtonState state){
        if(state == InventoryButtonState.Inventory){
            currentState = InventoryButtonState.Inventory;
        }else if(state == InventoryButtonState.Craftable){
            currentState = InventoryButtonState.Craftable;
            if(!Item.CanCraftItem()){
                Disabled = true;
            }
            InventoryItem = Item;
        }
        
    }

    /// <summary>
    /// Updates the inventory button with the given item and index.
    /// </summary>
    /// <param name="item">The item to display on the button.</param>
    /// <param name="index">The index of the button in the inventory.</param>
    public void UpdateItem(Item item, int index)
    {
        this.index = index;
        InventoryItem = item;
        if (item == null)
        {
            icon.Texture = null;
            quantityLabel.Text = string.Empty;
        }
        else
        {
            icon.Texture = item.Icon;
            quantityLabel.Text = item.Quantity.ToString();
        }
    }

}
