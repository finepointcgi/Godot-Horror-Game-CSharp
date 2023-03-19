using Godot;
using System;

public partial class InvestigationObject : CharacterBody3D
{
	public Item currentItem;
	bool overKeyItem;
	[Signal]
	public delegate void FoundItemEventHandler(Item item, Item itemToRemove);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(overKeyItem){
			if(Input.IsActionJustPressed("LeftMouseButtonDown")){
				EmitSignal(SignalName.FoundItem, currentItem.SubItem, currentItem);
			}
		}
	}

	public void Spawn(Item item){
		currentItem = item;
		if(item.SubItem != null){
			PackedScene scene = ResourceLoader.Load(item.SubItem.ResourcePath) as PackedScene;
			InvestigationObject obj = scene.Instantiate() as InvestigationObject;
			GetNode<Node3D>("SubItem").AddChild(obj);
			obj.MouseEntered += subObjectEntered;
			obj.MouseExited += subObjectExited;
		}
	}

	private void subObjectEntered(){
		GD.Print("entered subobject");
		overKeyItem = true;
	}

	private void subObjectExited(){
		GD.Print("exited subobject");
		overKeyItem = false;
	}
}
