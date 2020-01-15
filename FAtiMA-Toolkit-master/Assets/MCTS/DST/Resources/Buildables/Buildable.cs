﻿using System.Linq;
using MCTS.DST.WorldModels;
using System.Collections.Generic;
using MCTS.DST.Actions;
using MCTS.DST.Resources.Materials;

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
            ["endothermic_fire"] = EndothermicFire.Instance,
            ["fishing_rod"] = FishingRod.Instance,
            ["umbrella"] = Umbrella.Instance,
            ["whirly_fan"] = WhirlyFan.Instance,
        };
    }

    public class Buildable
    {
        public Dictionary<string, int> RequiredMaterials { get; private set; }
        protected string BuildableName { get; private set; }

        public Buildable(Dictionary<string, int> materialsQuantityDict, string name)
        {
            this.RequiredMaterials = materialsQuantityDict;
            this.BuildableName = name;
        }

        public void Build(WorldModelDST worldState)
        {
            for (int i = 0; i < this.RequiredMaterials.Count; i++)
            {
                var element = this.RequiredMaterials.ElementAt(i);
                var material = element.Key;
                worldState.RemoveFromPossessedItems(material, element.Value);
            }
            worldState.AddToPossessedItems(this.BuildableName, 1);
        }

        public virtual void PostProcessBuildable(WorldModelDST worldModel)
        {
            ;
        }
        
        public static void TryRemoveAction(WorldModelDST worldModel, string expendedResourceName)
        {
            MaterialDict materialDict = MaterialDict.Instance;
            Dictionary<string, WorldResource> materialBase = materialDict.materialBase;

            if (materialBase.ContainsKey(expendedResourceName))
            {
                WorldResource resource = materialBase[expendedResourceName];
                Dictionary<string, int> resourceRecipes = resource.Recipes;
                Dictionary<string, Buildable> buildableBase = BuildablesDict.Instance.buildableBase;

                for (int i = 0; i < resourceRecipes.Count; i++)
                {
                    string recipeName = resourceRecipes.ElementAt(i).Key;
                    Buildable buildable = buildableBase[recipeName];
                    Dictionary<string, int> requiredMaterials = buildable.RequiredMaterials;

                    for (int j = 0; j < requiredMaterials.Count; j++)
                    {
                        string requiredMaterialName = requiredMaterials.ElementAt(j).Key;
                        int requiredMaterialQuantity = requiredMaterials.ElementAt(j).Value;

                        // Commenting this as it isn't easily debugable.
                        /*
                        if (!(worldModel.Possesses(requiredMaterialName) && worldModel.PossessedItems[requiredMaterialName] >= requiredMaterialQuantity))
                        {
                            worldModel.RemoveAction(recipeName);
                            break;
                        }
                        */
                        if (!worldModel.Possesses(requiredMaterialName))
                        {
                            worldModel.RemoveAction(recipeName);
                            break;
                        }
                        else if (worldModel.PossessedItems[requiredMaterialName] < requiredMaterialQuantity)
                        {
                            worldModel.RemoveAction(recipeName);
                            break;
                        }
                    }
                }
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
                if (instance == null)
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
                if (instance == null)
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
                if (instance == null)
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
                if (instance == null)
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
                if (instance == null)
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
                if (instance == null)
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
                if (instance == null)
                {
                    instance = new Torch(new Dictionary<string, int>() { ["cutgrass"] = 2, ["twigs"] = 2 }, "torch");
                }
                return instance;
            }
        }

        private Torch(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }
    }

    public sealed class EndothermicFire : Structure
    {
        private static Buildable instance;

        public static Buildable Instance
        {
            get
            {
                if (instance == null)
                {                    
                    instance = new EndothermicFire(new Dictionary<string, int>() { ["cutgrass"] = 3, ["nitre"] = 2 }, "endothermic_fire");
                }
                return instance;
            }
        }

        private EndothermicFire(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }
    }

    public sealed class FishingRod : Item
    {
        private static Buildable instance;

        public static Buildable Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FishingRod(new Dictionary<string, int>() { ["twigs"] = 2, ["silk"] = 2 }, "fishing_rod");
                }
                return instance;
            }
        }

        private FishingRod(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }
    }

    public sealed class Umbrella : Item
    {
        private static Buildable instance;

        public static Buildable Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Umbrella(new Dictionary<string, int>() { ["twigs"] = 6, ["silk"] = 2, ["pig_skin"] = 1 }, "umbrella");
                }
                return instance;
            }
        }

        private Umbrella(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }
    }

    public sealed class WhirlyFan : Item
    {
        private static Buildable instance;

        public static Buildable Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WhirlyFan(new Dictionary<string, int>(){ ["twigs"] = 3, ["petals"] = 1}, "whirly_fan");
                }
                return instance;
            }
        }

        private WhirlyFan(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }
    }
}
