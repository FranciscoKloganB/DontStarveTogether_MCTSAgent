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
        protected string ItemName { get; private set; }
        protected bool Equipable { get; private set; }

        public Buildable(Dictionary<string, int> materialsQuantityDict, string name, bool equipable)
        {
            this.Materials = materialsQuantityDict;
            this.ItemName = name;
            this.Equipable = equipable;
        }

        public void Build(WorldModelDST worldState)
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

    public sealed class Axe : Buildable
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
                    instance = new Axe(_, "axe", true);
                }
                return instance;
            }
        }

        private Axe(Dictionary<string, int> materialsQuantityDict, string name, bool equipable) : base(materialsQuantityDict, name, equipable) { }
    }

    public sealed class Pickaxe : Buildable
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
                    instance = new Pickaxe(_, "pickaxe", true);
                }
                return instance;
            }
        }

        private Pickaxe(Dictionary<string, int> materialsQuantityDict, string name, bool equipable) : base(materialsQuantityDict, name, equipable) { }
    }

    public sealed class Shovel : Buildable
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
                    instance = new Shovel(_, "shovel", true);
                }
                return instance;
            }
        }

        private Shovel(Dictionary<string, int> materialsQuantityDict, string name, bool equipable) : base(materialsQuantityDict, name, equipable) { }
    }

    public sealed class Hammer : Buildable
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
                    instance = new Hammer(_, "hammer", true);
                }
                return instance;
            }
        }

        private Hammer(Dictionary<string, int> materialsQuantityDict, string name, bool equipable) : base(materialsQuantityDict, name, equipable) { }
    }

    public sealed class Campfire : Buildable
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
                    instance = new Campfire(_, "campfire", false);
                }
                return instance;
            }
        }

        private Campfire(Dictionary<string, int> materialsQuantityDict, string name, bool equipable) : base(materialsQuantityDict, name, equipable) { }
    }

    public sealed class FirePit : Buildable
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
                    instance = new FirePit(_, "firepit", false);
                }
                return instance;
            }
        }

        private FirePit(Dictionary<string, int> materialsQuantityDict, string name, bool equipable) : base(materialsQuantityDict, name, equipable) { }
    }

    public sealed class Torch : Buildable
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
                    instance = new Torch(_, "torch", true);
                }
                return instance;
            }
        }

        private Torch(Dictionary<string, int> materialsQuantityDict, string name, bool equipable) : base(materialsQuantityDict, name, equipable) { }
    }

    public sealed class EndothermicFire : Buildable
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
                    instance = new EndothermicFire(_, "endothermic_fire", false);
                }
                return instance;
            }
        }

        private EndothermicFire(Dictionary<string, int> materialsQuantityDict, string name, bool equipable) : base(materialsQuantityDict, name, equipable) { }
    }

    public sealed class FishingRod : Buildable
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
                    instance = new FishingRod(_, "fishing_rod", true);
                }
                return instance;
            }
        }

        private FishingRod(Dictionary<string, int> materialsQuantityDict, string name, bool equipable) : base(materialsQuantityDict, name, equipable) { }
    }

    public sealed class Umbrella : Buildable
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
                    instance = new Umbrella(_, "umbrella", true);
                }
                return instance;
            }
        }

        private Umbrella(Dictionary<string, int> materialsQuantityDict, string name, bool equipable) : base(materialsQuantityDict, name, equipable) { }
    }

    public sealed class WhirlyFan : Buildable
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
                    instance = new WhirlyFan(_, "whirly_fan", true);
                }
                return instance;
            }
        }

        private WhirlyFan(Dictionary<string, int> materialsQuantityDict, string name, bool equipable) : base(materialsQuantityDict, name, equipable) { }
    }
}
