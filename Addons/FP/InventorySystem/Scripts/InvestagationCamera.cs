using Godot;

public partial class InvestagationCamera : Camera3D
{
    /// <summary>
    /// The speed at which the camera zooms in and out.
    /// </summary>
    [Export]
    private float zoomSpeed = .2f;

    /// <summary>
    /// The direction that the camera zooms in and out.
    /// </summary>
    private Vector3 zoomDirection = new Vector3(0, 0, -1);

    /// <summary>
    /// Handles the input events for the camera.
    /// </summary>
    /// <param name="event">The input event that occurred.</param>
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseButton)
        {
            InputEventMouseButton mouseEvent = @event as InputEventMouseButton;
            if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
            {
                // Zooms in the camera by translating it along the zoom direction.
                Translate(Utilities.MulitplyVectorByFloat(zoomDirection, zoomSpeed));
            }
            else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
            {
                // Zooms out the camera by translating it along the opposite of the zoom direction.
                Translate(Utilities.MulitplyVectorByFloat(zoomDirection, -zoomSpeed));

            }
        }
    }
}
