using System.Collections.Generic;
using Godot;
using Godot.Collections;
/// <summary>
/// A class that represents an item that can be used, collected, or interacted with in the game.
/// </summary>
public partial class Item : Resource
{

    [Export]
    public int ID { get; set; } // The unique identifier of the item
    [Export]
    public string Name { get; set; } // The name of the item
    [Export]
    public string ResourcePath { get; set; } // The path to the scene file that contains the item
    [Export]
    public Texture2D Icon { get; set; } // The icon of the item that is displayed in the inventory
    [Export]
    public int Quantity { get; set; } // The number of items in a stack
    [Export]
    public int StackSize { get; set; } // The maximum number of items that can be stacked together
    [Export]
    public bool IsStackable { get; set; } // A flag that indicates whether the item can be stacked or not
    [Export]
    public Item SubItem { get; set; } // A reference to another item that is contained within this item
    [Export]
    public Item BaseItem { get; set; } // A reference to another item that this item is derived from
    [Export]
    public Godot.Collections.Dictionary<Resource, int> test;
    public int Cost;
    
    /// <summary>
    /// A method that creates a shallow copy of this item.
    /// </summary>
    /// <returns>A new item object with the same properties as this item.</returns>
    public Item Copy() => MemberwiseClone() as Item;

    /// <summary>
    /// A virtual method that defines what happens when the item is used by the player.
    /// </summary>
    public virtual void UseItem()
    {
        GD.Print("Used Item!");
    }

    public virtual void CraftItem(){

        bool ableToBuild = false;
        int i = 0;
        foreach (var item in test)
        {
            if(item.Key is Item){
                Item currentItem = item.Key as Item;
                if(Inventory.GlobalInventory.CanAfford(currentItem, item.Value)){
                    i++;
                }
            }
        }
        if(i >= test.Count){
            foreach (var item in test)
            {
                bool failed = Inventory.GlobalInventory.Remove(item.Key as Item, item.Value);
                if(failed){
                    //return;
                }
            }
            Inventory.GlobalInventory.Add(this);
        }
        

    }

    public virtual bool CanCraftItem(){
        int i = 0;
        foreach (var item in test)
        {
            if(item.Key is Item){
                Item currentItem = item.Key as Item;
                if(Inventory.GlobalInventory.CanAfford(currentItem, item.Value)){
                    i++;
                }
            }
        }
        if(i >= test.Count){
        return true;
        }
        return false;
    }

}
