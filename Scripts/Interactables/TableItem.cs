using Godot;
using MeatPartyV0.Empty.Scripts.Resources;

public partial class TableItem : Area3D
{
    private MeshInstance3D _tableMesh {get; set;}
    [Export] public Resource Stats;


    public override void _Ready()
    {
        _tableMesh = GetNode<MeshInstance3D>("TableMesh");

        if (Stats is Item item)
        {
            _tableMesh.Mesh = item.ItemMesh;
        }
        
        
    }

    private void _on_input_event(Node camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButton &&
            mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
        {
            this.QueueFree();
        }
    }
}
