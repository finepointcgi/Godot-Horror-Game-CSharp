using Godot;
using System;

public partial class Item : Resource
{
    [Export]
    public string ID { get; set; }
    [Export]
    public string Name { get; set; }
    [Export]
    public string ResourcePath { get; set; }
    [Export]
    public Texture2D Icon { get; set; }
    [Export]
    public int Quantity { get; set; }
    [Export]
    public int StackSize { get; set; }
    [Export]
    public bool IsStackable = false;
    [Export]
    public bool IsSplittable = false;

    public Item SwapItem(Item item)
    {
        Item temp = Copy();
        Name = item.Name;
        ResourcePath = item.ResourcePath;
        Icon = item.Icon;
        Quantity= item.Quantity;
        StackSize = item.StackSize;

        return temp;
    }

    public Item Stack(Item item)
    {
        if (item == null)
        {
            return null;
        }

        if (ID != item.ID)
        {
            return SwapItem(item);
        }

        if (!IsStackable || !item.IsStackable)
        {
            return SwapItem(item);
        }

        Quantity += item.Quantity;

        if (Quantity > StackSize)
        {
            // Stack size is too large.
            // Clamp the quantity and put the remainder back into the original stack.
            item.Quantity = Quantity - StackSize;
            Quantity = StackSize;
        }
        else
        {
            // No remainders in the original stack.
            return null;
        }

        // Return remainders.
        return item;
    }

    public Item Split()
    {
        if (!IsSplittable)
        {
            return null;
        }

        Item item = MemberwiseClone() as Item;

        int stack = Quantity;
        Quantity /= 2;
        item.Quantity = stack - Quantity;

        return item;
    }


    public Item Copy() => MemberwiseClone() as Item;
}
