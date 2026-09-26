using Godot;

[GlobalClass]
public partial class Interactable : Area3D
{
    [Signal]
    public delegate void InteractedEventHandler(Node body);

    [Export]
    public string PromptMessage { get; set; } = "[ E ]";

    public void Interact(Node body)
    {
        EmitSignal(SignalName.Interacted, body);
    }
}
