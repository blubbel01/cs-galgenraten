using System;
using Playground.Manager.Inventory;

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
            Inventory inv = InventoryWrapper.GetInventory(1);

            foreach (var itemStack in inv.Items)
            {
                string s = "";
                s += $"Type: {itemStack.Type}, Name: {itemStack.Name}, Amount: {itemStack.Amount}, Type: {itemStack.Type}, ";
                s += $"DisplayName: {itemStack.Meta.DisplayName}, Lore: {itemStack.Meta.Lore}, Damage: {itemStack.Meta.DisplayName}, ";
                
                s += "Attributes: [";
                foreach (var attributeModifier in itemStack.Meta.AttributeModifiers)
                {
                    s += $"{attributeModifier.Attribute}:{attributeModifier.Value},";
                }
                s += "]";
                Console.WriteLine(s);
            }
        }

    }
}