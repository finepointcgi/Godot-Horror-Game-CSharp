using System;

public partial class ItemButtonEventArgs : EventArgs
{
    public int Index { get; set; }
    public Item Item { get; set; }

    public ItemButtonEventArgs(int index, Item item) : base()
    {
        Index = index;
        Item = item;
    }
}