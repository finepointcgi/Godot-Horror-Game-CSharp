using Godot;
using System;

public partial class InvestigationBaseScene : Node3D
{
	private bool isRotating;
	private Vector2 mouseOffeset = new Vector2();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("LeftMouseButtonDown")){
			isRotating = true;
			mouseOffeset = GetTree().Root.GetMousePosition();
		}else if (Input.IsActionJustReleased("LeftMouseButtonDown")){
			isRotating = false;
		} 
		if(isRotating){
			mouseOffeset = GetTree().Root.GetMousePosition() - mouseOffeset;
			GetNode<Node3D>("RotationAroundBase").RotationDegrees += new Vector3(mouseOffeset.Y  * .4f, mouseOffeset.X  * .4f, 0);
			mouseOffeset = GetTree().Root.GetMousePosition();
		}
	}

	public void ShowObject(Item item){
		Show();
		if(GetNode<Node3D>("RotationAroundBase").GetChildCount() > 0){
			Utitlies.RemoveChildren(GetNode<Node3D>("RotationAroundBase"));
		}
		if(item != null){
			if(item.ResourcePath != null && item.ResourcePath != ""){
				PackedScene scene = ResourceLoader.Load(item.ResourcePath) as PackedScene;
				InvestigationObject instance = scene.Instantiate() as InvestigationObject;
				GetNode<Node3D>("RotationAroundBase").AddChild(instance);
				instance.Spawn(item);
				instance.FoundItem += FoundItem;
			}
		}
	}

	private void FoundItem(Item foundItem, Item itemToRemove){
		ShowObject(foundItem);
		Inventory inventory = GetParent() as Inventory;
		if (itemToRemove.BaseItem != null){
			inventory.Add(itemToRemove.BaseItem);
		}
		inventory.Remove(itemToRemove);
		inventory.Add(foundItem);
	}
}
