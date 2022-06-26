using Playground.Manager.Inventory;
using Playground.Manager.Inventory.Meta;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            MaterialObject.Init();

            Exampe();
        }

        static void Exampe()
        {
            Inventory inv = new Inventory("Spielerinventar: Shepard", 200);

            inv.AddItem(new ItemStack(Material.WEAPON_SMG, 1));
            
            inv.Items[0].Meta.FlagsList.Contains(ItemFlags.UNBREAKABLE);

            InventoryWrapper.SaveInventory(1, inv);
        }

    }
}