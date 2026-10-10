using Godot;

namespace MeatPartyV0.Empty.Scripts.Resources;

[GlobalClass]
public partial class Item : Resource
{
    [Export] public string ItemName {get; set;}
    [Export] public string ItemDescription {get; set;}
    [Export] public Mesh ItemMesh {get; set;}
    
}