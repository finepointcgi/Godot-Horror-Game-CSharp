using Godot;
using System;

public partial class HealthPotion : Item
{
	[Export]
	public int HealAmount = 10;

	public override void UseItem()
	{
		GD.Print("heal the Player for " + HealAmount);
	}
}
