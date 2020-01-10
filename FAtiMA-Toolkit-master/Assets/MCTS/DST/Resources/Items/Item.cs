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
            ["axe"] = Torch.Instance,
            ["pickaxe"] = Torch.Instance,
            ["campfire"] = Torch.Instance,
            ["firepit"] = Torch.Instance,
            ["torch"] = Torch.Instance,
            ["some_item"] = Torch.Instance,
            ["some_item"] = Torch.Instance,
            ["some_item"] = Torch.Instance,
            ["some_item"] = Torch.Instance,
            ["some_item"] = Torch.Instance,
            ["some_item"] = Torch.Instance,
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

    public sealed class Torch : Item
    {
        private static Item instance;

        public static Item Instance
        {
            get
            {
                if (Instance == null)
                {
                    _ = new Dictionary<string, int>()
                    {
                        ["twigs"] = 2,
                    };
                    instance = new Torch(_, "torch", true);
                }
                return instance;
            }
        }

        private Torch(Dictionary<string, int> materialsQuantityDict, string name, bool equipable) : base(materialsQuantityDict, name, equipable) { }

            //new Dictionary<string, int>(), "", false);
    }


}
