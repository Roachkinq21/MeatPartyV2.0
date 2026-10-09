using Godot;

public partial class TableItem : Area3D
{
    private void _on_input_event(Node camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButton &&
            mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
        {
            this.QueueFree();
        }
    }
}
