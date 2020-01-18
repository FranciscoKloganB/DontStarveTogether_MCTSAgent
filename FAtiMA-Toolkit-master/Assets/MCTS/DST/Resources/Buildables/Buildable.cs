using System.Linq;
using MCTS.DST.WorldModels;
using System.Collections.Generic;
using MCTS.DST.Actions;
using MCTS.DST.Resources.Materials;
using System;

namespace MCTS.DST.Resources.Buildables
{
    public sealed class BuildablesDict
    {
        public static BuildablesDict Instance { get; } = new BuildablesDict();

        private BuildablesDict()
        {
        }

        public Dictionary<string, Buildable> buildableBase = new Dictionary<string, Buildable>()
        {
            ["axe"] = Axe.Instance,
            ["pickaxe"] = Pickaxe.Instance,
            ["shovel"] = Shovel.Instance,
            ["hammer"] = Hammer.Instance,
            ["campfire"] = Campfire.Instance,
            ["firepit"] = FirePit.Instance,
            ["torch"] = Torch.Instance,
        };
    }

    public class Buildable
    {
        public Dictionary<string, int> RequiredMaterials { get; private set; }
        public string BuildableName { get; private set; }

        public Buildable(Dictionary<string, int> materialsQuantityDict, string name)
        {
            this.RequiredMaterials = materialsQuantityDict;
            this.BuildableName = name;
        }

        public bool Build(WorldModelDST worldState)
        {
            for (int i = 0; i < this.RequiredMaterials.Keys.Count; i++)
            {
                var element = this.RequiredMaterials.ElementAt(i);
                if (!worldState.Possesses(element.Key, element.Value))
                {
                    return false;
                }
            }

            for (int i = 0; i < this.RequiredMaterials.Count; i++)
            {
                var element = this.RequiredMaterials.ElementAt(i);
                var material = element.Key;
                worldState.RemoveFromPossessedItems(material, element.Value);
            }
            return true;
        }

        public virtual void PostProcessBuildable(WorldModelDST worldModel)
        {
            ;
        }
        
        public void TryRemoveAction(WorldModelDST worldModel)
        {
            Dictionary<string, Buildable> buildableBase = BuildablesDict.Instance.buildableBase;
            Dictionary<string, WorldResource> materialBase = MaterialDict.Instance.materialBase;

            string recipeName = "";
            try
            {
                for (int i = 0; i < this.RequiredMaterials.Count; i++)
                {
                    string materialName = this.RequiredMaterials.ElementAt(i).Key;
                    if (!materialBase.ContainsKey(materialName))
                    {
                        continue;
                    }

                    WorldResource resource = materialBase[materialName];
                    Dictionary<string, int> resourceRecipes = resource.Recipes;
                    for (int j = 0; j < resourceRecipes.Count; j++)
                    {
                        recipeName = resourceRecipes.ElementAt(j).Key;
                        Buildable buildable = buildableBase[recipeName];
                        Dictionary<string, int> requiredMaterials = buildable.RequiredMaterials;

                        for (int k = 0; k < requiredMaterials.Count; k++)
                        {
                            string requiredMaterialName = requiredMaterials.ElementAt(k).Key;
                            int requiredMaterialQuantity = requiredMaterials.ElementAt(k).Value;

                            if (!worldModel.Possesses(requiredMaterialName, requiredMaterialQuantity))
                            {
                                worldModel.RemoveAction(recipeName);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Buildable.cs error with recipe " + recipeName + ": " + ex.Message + "\n" + ex.StackTrace);
            }
        }
    }

    public class Item : Buildable
    {
        public Item(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }

        public override void PostProcessBuildable(WorldModelDST worldModel)
        {
            if (worldModel.EquippedItems.Contains(this.BuildableName))
            {
                ActionDST action = new Unequip(this.BuildableName);
                worldModel.AddAction(action);
            }
        }
    }

    public class Structure : Buildable
    {
        public Structure(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }

        public override void PostProcessBuildable(WorldModelDST worldModel)
        {
            worldModel.AddToWorld(this.BuildableName, 1, worldModel.Walter.Position.Item1, worldModel.Walter.Position.Item2);
            worldModel.AddToFire(this.BuildableName, worldModel.Walter.Position.Item1, worldModel.Walter.Position.Item2);
        }
    }

    public sealed class Axe : Item
    {
        private static Buildable instance;

        public static Buildable Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = new Axe(new Dictionary<string, int>() { ["twigs"] = 1, ["flint"] = 1 }, "axe");
                }
                return instance;
            }
        }

        private Axe(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }
    }

    public sealed class Pickaxe : Item
    {
        private static Buildable instance;

        public static Buildable Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = new Pickaxe(new Dictionary<string, int>() { ["twigs"] = 2, ["flint"] = 2 }, "pickaxe");
                }
                return instance;
            }
        }

        private Pickaxe(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }
    }

    public sealed class Shovel : Item
    {
        private static Buildable instance;

        public static Buildable Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = new Shovel(new Dictionary<string, int>() { ["twigs"] = 2, ["flint"] = 2 }, "shovel");
                }
                return instance;
            }
        }

        private Shovel(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }
    }

    public sealed class Hammer : Item
    {
        private static Buildable instance;

        public static Buildable Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = new Hammer(new Dictionary<string, int>() { ["twigs"] = 3, ["rocks"] = 3, ["cutgrass"] = 6 }, "hammer");
                }
                return instance;
            }
        }

        private Hammer(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }
    }

    public sealed class Campfire : Structure
    {
        private static Buildable instance;

        public static Buildable Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = new Campfire(new Dictionary<string, int>() { ["cutgrass"] = 3, ["log"] = 2 }, "campfire");
                }
                return instance;
            }
        }

        private Campfire(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }
    }

    public sealed class FirePit : Structure
    {
        private static Buildable instance;

        public static Buildable Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = new FirePit(new Dictionary<string, int>() { ["log"] = 2, ["rocks"] = 12 }, "firepit");
                }
                return instance;
            }
        }

        private FirePit(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }
    }

    public sealed class Torch : Item
    {
        private static Buildable instance;

        public static Buildable Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = new Torch(new Dictionary<string, int>() { ["cutgrass"] = 2, ["twigs"] = 2 }, "torch");
                }
                return instance;
            }
        }

        private Torch(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }
    }
}
