using Godot;
using System;

public partial class InvestagationCamera : Camera3D
{
	[Export]
	private float zoomSpeed = .2f;
	private Vector3 zoomDirection = new Vector3(0,0,-1);

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
		if(@event is InputEventMouseButton){
			InputEventMouseButton mouseEvent = @event as InputEventMouseButton;
			if(mouseEvent.ButtonIndex == MouseButton.WheelUp){
				Translate(Utitlies.MulitplyVectorByFloat(zoomDirection, zoomSpeed));
			}else if(mouseEvent.ButtonIndex == MouseButton.WheelDown){
				Translate(Utitlies.MulitplyVectorByFloat(zoomDirection, -zoomSpeed));

			}
		}
    }
}
