using Godot;
using System;

public partial class Item : Resource
{
    [Export]
    public int ID { get; set; }
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
    public bool IsStackable { get; set; }
    [Export]
    public Item SubItem { get; set; }
    [Export]
    public Item BaseItem {get; set;}
    [Export]
    public Godot.Collections.Dictionary<Resource, int> ItemCraftableMakeup; //items and cost

    public Item Copy() => MemberwiseClone() as Item;

    public virtual void UseItem()
    {
        GD.Print("Used Item!");
    }

    public bool CanCraftItem(){
        int countOfAffordedItems = 0;
        foreach (var item in ItemCraftableMakeup)
        {
            if(GameManager.Inventory.CanAfford((Item)item.Key, item.Value)){
                countOfAffordedItems ++;
            }
        }

        if(countOfAffordedItems >= ItemCraftableMakeup.Count){
            return true;
        }
        return false;
    }

    public void CraftItem(){
        if(CanCraftItem()){
            foreach (var item in ItemCraftableMakeup)
            {
                bool success = GameManager.Inventory.Remove((Item)item.Key, item.Value);
                if(!success){
                    GD.Print("Failed To Remove " + item.Key.ResourceName);
                    return;
                }
            }
            GameManager.Inventory.Add(this);
        }
    }
}
