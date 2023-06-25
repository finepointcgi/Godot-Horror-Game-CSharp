using Godot;

public partial class InvestigationBaseScene : Node3D
{
    /// <summary>
    /// A flag that indicates whether the camera is rotating or not.
    /// </summary>
    private bool isRotating;

    /// <summary>
    /// The offset of the mouse position from the center of the screen in 2D space.
    /// </summary>
    private Vector2 mouseOffeset = new Vector2();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    /// <summary>
    /// Processes the camera movement and rotation based on the mouse input.
    /// </summary>
    /// <param name="delta">The time elapsed since the last frame in seconds.</param>
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("LeftMouseButtonDown"))
        {
            // Sets the isRotating flag to true when the left mouse button is pressed.
            isRotating = true;
            // Stores the initial mouse position as the offset.
            mouseOffeset = GetTree().Root.GetMousePosition();
        }
        else if (Input.IsActionJustReleased("LeftMouseButtonDown"))
        {
            // Sets the isRotating flag to false when the left mouse button is released.
            isRotating = false;
        }
        if (isRotating)
        {
            // Calculates the difference between the current mouse position and the offset.
            mouseOffeset = GetTree().Root.GetMousePosition() - mouseOffeset;
            // Rotates the camera around the base node by adding the mouse offset to the rotation degrees.
            GetNode<Node3D>("RotationAroundBase").RotationDegrees += new Vector3(mouseOffeset.Y * .4f, mouseOffeset.X * .4f, 0);
            // Updates the offset with the current mouse position.
            mouseOffeset = GetTree().Root.GetMousePosition();
        }
    }

    /// <summary>
    /// Shows the given item as an investigation object in the scene.
    /// </summary>
    /// <param name="item">The item to show.</param>
    public void ShowObject(Item item)
    {
        // Makes the node visible.
        Show();
        if (GetNode<Node3D>("RotationAroundBase").GetChildCount() > 0)
        {
            // Removes any existing children of the rotation node.
            Utilities.RemoveChildren(GetNode<Node3D>("RotationAroundBase"));
        }
        if (item != null)
        {
            if (item.ResourcePath != null && item.ResourcePath != "")
            {
                // Loads the scene of the item from its resource path.
                PackedScene scene = ResourceLoader.Load(item.ResourcePath) as PackedScene;
                // Instantiates the scene as an investigation object.
                InvestigationObject instance = scene.Instantiate() as InvestigationObject;
                // Adds the instance as a child of the rotation node.
                GetNode<Node3D>("RotationAroundBase").AddChild(instance);
                // Spawns the instance with the item data.
                instance.Spawn(item);
                // Connects the found item signal to the handler method.
                instance.FoundItem += FoundItem;
            }
        }
    }

    /// <summary>
    /// Handles the found item signal from an investigation object.
    /// </summary>
    /// <param name="foundItem">The item that was found.</param>
    /// <param name="itemToRemove">The item that was investigated.</param>
    private void FoundItem(Item foundItem, Item itemToRemove)
    {
        // Shows the found item as an investigation object in the scene.
        ShowObject(foundItem);
        // Gets the parent node as an inventory.
        Inventory inventory = GetParent() as Inventory;
        if (itemToRemove.BaseItem != null)
        {
            // Adds the base item of the investigated item to the inventory.
            inventory.Add(itemToRemove.BaseItem);
        }
        // Removes the investigated item from the inventory.
        inventory.Remove(itemToRemove);
        // Adds the found item to the inventory.
        inventory.Add(foundItem);
    }
}
