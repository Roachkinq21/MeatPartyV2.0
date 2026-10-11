using Godot;
using MeatPartyV0.Empty.Scripts.Resources;

namespace MeatPartyV0.Empty.Scripts;

public partial class Inventory
{
    [Signal]
    delegate void UpdatedEventHandler();
    
    const int SIZE = 12;
    //private Godot.Collections.Array<ItemStack> items = new();

    public Inventory()
    {
        for (int slot = 0; slot < SIZE; slot++)
        {
            //items.Add(new ItemStack(Items.EMPTY, 0));
        }
    }


    public void AddItem(Item item)
    {
        
    }

    public void RemoveItem(Item item)
    {
        
    }
    
    
}