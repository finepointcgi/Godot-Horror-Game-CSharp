using Godot;

public partial class InvestigationObject : CharacterBody3D
{
    /// <summary>
    /// The current item that the player is holding or interacting with.
    /// </summary>
    public Item currentItem;

    /// <summary>
    /// A flag that indicates whether the current item is a key item or not.
    /// </summary>
    private bool overKeyItem;

    /// <summary>
    /// A signal that is emitted when the player finds a new item and optionally removes an old item from their inventory.
    /// </summary>
    /// <param name="item">The new item that the player found.</param>
    /// <param name="itemToRemove">The old item that the player wants to remove from their inventory, or null if none.</param>
    [Signal]
    public delegate void FoundItemEventHandler(Item item, Item itemToRemove);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }


    /// <summary>
    /// A method that is called every frame and checks for user input and key item interaction.
    /// </summary>
    /// <param name="delta">The time elapsed since the last frame.</param>
    public override void _Process(double delta)
    {
        // If the player is over a key item
        if (overKeyItem)
        {
            // If the player clicks the left mouse button
            if (Input.IsActionJustPressed("LeftMouseButtonDown"))
            {
                // Emit the signal with the sub item and the current item as parameters
                EmitSignal(SignalName.FoundItem, currentItem.SubItem, currentItem);
            }
        }
    }

    /// <summary>
    /// A method that spawns a new item and assigns it to the current item variable.
    /// </summary>
    /// <param name="item">The item to spawn.</param>
    public void Spawn(Item item)
    {
        // Set the current item to the given item
        currentItem = item;
        // If the item has a sub item
        if (item.SubItem != null)
        {
            // Load the sub item scene from the resource path
            PackedScene scene = ResourceLoader.Load(item.SubItem.ResourcePath) as PackedScene;
            // Instantiate the sub item as an investigation object
            InvestigationObject obj = scene.Instantiate() as InvestigationObject;
            // Add the sub item as a child of the node named "SubItem"
            GetNode<Node3D>("SubItem").AddChild(obj);
            // Connect the mouse entered and exited signals of the sub item to the corresponding methods
            obj.MouseEntered += subObjectEntered;
            obj.MouseExited += subObjectExited;
        }
    }

    /// <summary>
    /// A method that is called when the mouse cursor enters the sub item area.
    /// </summary>
    private void subObjectEntered()
    {
        // Print a message to the console
        GD.Print("entered subobject");
        // Set the flag to indicate that the player is over a key item
        overKeyItem = true;
    }

    /// <summary>
    /// A method that is called when the mouse cursor exits the sub item area.
    /// </summary>
    private void subObjectExited()
    {
        // Print a message to the console
        GD.Print("exited subobject");
        // Set the flag to indicate that the player is not over a key item
        overKeyItem = false;
    }
}
