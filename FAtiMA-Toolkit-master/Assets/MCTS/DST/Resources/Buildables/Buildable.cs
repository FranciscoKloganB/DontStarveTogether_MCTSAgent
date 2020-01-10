using System.Linq;
using MCTS.DST.WorldModels;
using System.Collections.Generic;
using MCTS.DST.Actions;

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
            ["campfirre"] = Campfire.Instance,
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
        protected Dictionary<string, int> Materials { get; private set; }
        protected string BuildableName { get; private set; }

        public Buildable(Dictionary<string, int> materialsQuantityDict, string name)
        {
            this.Materials = materialsQuantityDict;
            this.BuildableName = name;
        }

        public void Build(WorldModelDST worldState)
        {
            for (int index = 0; index < this.Materials.Count; index++)
            {
                var element = this.Materials.ElementAt(index);
                worldState.RemoveFromPossessedItems(element.Key, element.Value);
            }
            worldState.AddToPossessedItems(this.BuildableName, 1);
        }

        public virtual void PostProcessBuildable(WorldModelDST worldModel)
        {
            ;
        }

        public void TryRemoveAction(WorldModelDST worldModel, string actionName)
        {

        }
    }

    public class Item : Buildable
    {
        public Item(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }

        public override void PostProcessBuildable(WorldModelDST worldModel)
        {
            if (worldModel.EquippedItems.Count == 0)
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
                if (Instance == null)
                {
                    _ = new Dictionary<string, int>()
                    {
                        ["twigs"] = 1,
                        ["flint"] = 1
                    };
                    instance = new Axe(_, "axe");
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
                if (Instance == null)
                {
                    _ = new Dictionary<string, int>()
                    {
                        ["twigs"] = 2,
                        ["flint"] = 2
                    };
                    instance = new Pickaxe(_, "pickaxe");
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
                if (Instance == null)
                {
                    _ = new Dictionary<string, int>()
                    {
                        ["twigs"] = 2,
                        ["flint"] = 2
                    };
                    instance = new Shovel(_, "shovel");
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
                if (Instance == null)
                {
                    _ = new Dictionary<string, int>()
                    {
                        ["twigs"] = 3,
                        ["rocks"] = 3,
                        ["cutgrass"] = 6
                    };
                    instance = new Hammer(_, "hammer");
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
                if (Instance == null)
                {
                    _ = new Dictionary<string, int>()
                    {
                        ["cutgrass"] = 3,
                        ["log"] = 2
                    };
                    instance = new Campfire(_, "campfire");
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
                if (Instance == null)
                {
                    _ = new Dictionary<string, int>()
                    {
                        ["log"] = 2,
                        ["rocks"] = 12
                    };
                    instance = new FirePit(_, "firepit");
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
                if (Instance == null)
                {
                    _ = new Dictionary<string, int>()
                    {
                        ["cutgrass"] = 2,
                        ["twigs"] = 2
                    };
                    instance = new Torch(_, "torch");
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
                if (Instance == null)
                {
                    _ = new Dictionary<string, int>()
                    {
                        ["cutgrass"] = 3,
                        ["nitre"] = 2
                    };
                    instance = new EndothermicFire(_, "endothermic_fire");
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
                if (Instance == null)
                {
                    _ = new Dictionary<string, int>()
                    {
                        ["twigs"] = 2,
                        ["silk"] = 2
                    };
                    instance = new FishingRod(_, "fishing_rod");
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
                if (Instance == null)
                {
                    _ = new Dictionary<string, int>()
                    {
                        ["twigs"] = 6,
                        ["silk"] = 2,
                        ["pig_skin"] = 1
                    };
                    instance = new Umbrella(_, "umbrella");
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
                if (Instance == null)
                {
                    _ = new Dictionary<string, int>()
                    {
                        ["twigs"] = 3,
                        ["petals"] = 1,
                    };
                    instance = new WhirlyFan(_, "whirly_fan");
                }
                return instance;
            }
        }

        private WhirlyFan(Dictionary<string, int> materialsQuantityDict, string name) : base(materialsQuantityDict, name) { }
    }
}
