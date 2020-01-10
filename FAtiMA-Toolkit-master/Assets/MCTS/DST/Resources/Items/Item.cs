using System.Linq;
using MCTS.DST.WorldModels;
using System.Collections.Generic;
using MCTS.DST.Actions;

namespace MCTS.DST.Resources.Items
{
    public sealed class ItemDict
    {
        public static ItemDict Instance { get; } = new ItemDict();

        private ItemDict()
        {
        }

        public Dictionary<string, Item> itemBase = new Dictionary<string, Item>()
        {
            ["axe"] = SomeItem.Instance,
            ["pickaxe"] = SomeItem.Instance,
            ["campfire"] = SomeItem.Instance,
            ["firepit"] = SomeItem.Instance,
            ["torch"] = SomeItem.Instance,
            ["some_item"] = SomeItem.Instance,
            ["some_item"] = SomeItem.Instance,
            ["some_item"] = SomeItem.Instance,
            ["some_item"] = SomeItem.Instance,
            ["some_item"] = SomeItem.Instance,
            ["some_item"] = SomeItem.Instance,
        };
    }

    public class Item
    {
        protected Dictionary<string, int> Materials { get; private set; }
        protected string ItemName { get; private set; }
        protected bool Equipable { get; private set; }

        public Item(Dictionary<string, int> materialsQuantityDict, string name, bool equipable)
        {
            this.Materials = materialsQuantityDict;
            this.ItemName = name;
            this.Equipable = equipable;
        }
        
        public void ConstructItem(WorldModelDST worldState)
        {
            for (int index = 0; index < this.Materials.Count; index++)
            {
                var element = this.Materials.ElementAt(index);
                worldState.RemoveFromPossessedItems(element.Key, element.Value);
            }
            worldState.AddToPossessedItems(this.ItemName, 1);
        }

        public void TryMakeUnequipable(WorldModelDST worldModel)
        {
            if (Equipable && worldModel.EquippedItems.Count == 0)
            {
                ActionDST action = new Unequip(this.ItemName);
                worldModel.AddAction(action);
            }
        }

        public void TryRemoveAction(WorldModelDST worldModel, string actionName)
        {
            
        }
    }

    public sealed class SomeItem : Item
    {
        private SomeItem(Dictionary<string, int> materialsQuantityDict, string name, bool equipable) : base(materialsQuantityDict, name, equipable) { }

        public static Item Instance { get; } = new SomeItem(new Dictionary<string, int>(), "", false);
    }


}
