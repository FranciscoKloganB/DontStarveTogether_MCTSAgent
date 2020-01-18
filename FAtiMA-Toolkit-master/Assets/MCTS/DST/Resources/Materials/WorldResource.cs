using MCTS.DST.Resources.Edibles;
using MCTS.DST.WorldModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCTS.DST.Resources.Materials
{
    public sealed class MaterialDict
    {
        public static MaterialDict Instance { get; } = new MaterialDict();

        public Dictionary<string, WorldResource> materialBase = new Dictionary<string, WorldResource>()
        {
            ["boulder"] = Boulder.Instance,
            ["tree"] = Tree.Instance,
            ["sapling"] = Sapling.Instance,
            ["rocks"] = Rock.Instance,
            ["flint"] = Flint.Instance,
            ["log"] = Log.Instance,
            ["twigs"] = Twig.Instance,
            ["grass"] = Grass.Instance,
            ["cutgrass"] = Cutgrass.Instance,
            ["berrybush"] = BerryBush.Instance,
            ["carrot_planted"] = PlantedCarrot.Instance,
            ["flower"] = Flower.Instance,
        };

        private MaterialDict() { }
    }

    public class WorldResource
    {

        public bool IsPrimitive { get; protected set; }
        public bool IsFuel { get; protected set; }
        public bool IsPickable { get; protected set; }

        public string ResourceName { get; protected set; }

        public Dictionary<string, int> Recipes { get; protected set; }

        public WorldResource(bool isPrimitive, bool isFuel, bool isPickable)
        {
            this.IsPrimitive = isPrimitive;
            this.IsFuel = isFuel;
            this.IsPickable = isPickable;
            this.Recipes = new Dictionary<string, int>();
        }

        public virtual void GetBonuses(WorldModelDST worldModel)
        {
            ;
        }

        public virtual void TryRemoveAction(WorldModelDST worldModel, string actionName)
        { 
            for (int i = 0; i < this.Recipes.Count; i++)
            {
                if (!worldModel.Possesses(this.ResourceName, this.Recipes.ElementAt(i).Value))
                {
                    worldModel.RemoveAction(string.Concat(actionName, this.Recipes.ElementAt(i).Key));
                }
            }
        }
    }

    public class BasicWorldResource : WorldResource
    {
        public int Quantity { get; private set; }

        public BasicWorldResource(string name, int quantity, bool isFuel) : base(true, isFuel, true)
        {
            this.ResourceName = name;
            this.Quantity = quantity;
        }
    }

    public class Tool : BasicWorldResource
    {
        public Tool(string name, int quantity) : base (name, quantity, false)
        {
            // TODO?
        }
    }

    public class CompoundWorldResource : WorldResource
    {
        public Tool RequiredTool { get; protected set; } = null;
        public string RequiredToolAction { get; protected set; } = "UNDEFINED REQUIRED TOOL ACTION";

        public CompoundWorldResource() : base(false, false, false) { }

        public CompoundWorldResource(bool isFuel) : base(false, isFuel, false) { }
    }

    public class GatherableCompoundWorldResource : CompoundWorldResource
    {
        public Object ResourceWhenPicked { get; protected set; }

        public GatherableCompoundWorldResource() : base() {  }
    }

    public sealed class Boulder : CompoundWorldResource
    {
        public Boulder()
        {
            this.ResourceName = "boulder";
            this.RequiredTool = Pickaxe.Instance;
            this.RequiredToolAction = "MINE";
        }

        public static Boulder Instance { get; } = new Boulder();
    }

    public sealed class Tree : CompoundWorldResource
    {
        public Tree() : base(true)
        {
            this.ResourceName = "tree";
            this.RequiredTool = Axe.Instance;
            this.RequiredToolAction = "CHOP";
        }

        public static Tree Instance { get; } = new Tree();
    }

    public sealed class Sapling : GatherableCompoundWorldResource
    {
        public Sapling() : base()
        {
            this.ResourceName = "sapling";
            this.ResourceWhenPicked = Twig.Instance;
            this.RequiredTool = null;
            this.RequiredToolAction = "PICK";
        }

        public static Sapling Instance { get; } = new Sapling();
    }

    public sealed class Grass : GatherableCompoundWorldResource
    {
        public Grass() : base()
        {
            this.ResourceName = "grass";
            this.ResourceWhenPicked = Cutgrass.Instance;
            this.RequiredTool = null;
            this.RequiredToolAction = "PICK";
        }

        public static Grass Instance { get; } = new Grass();
    }

    public sealed class BerryBush : GatherableCompoundWorldResource
    {
        public BerryBush() : base()
        {
            this.ResourceName = "berrybush";
            this.ResourceWhenPicked = Berries.Instance;
            this.RequiredTool = null;
            this.RequiredToolAction = "PICK";
        }

        public static BerryBush Instance { get; } = new BerryBush();
    }

    public sealed class PlantedCarrot : GatherableCompoundWorldResource
    {
        public PlantedCarrot() : base()
        {
            this.ResourceName = "carrot_planted";
            this.ResourceWhenPicked = Carrot.Instance;
            this.RequiredTool = null;
            this.RequiredToolAction = "PICK";
        }

        public static PlantedCarrot Instance { get; } = new PlantedCarrot();
    }

    public sealed class Flower : GatherableCompoundWorldResource
    {
        public Flower() : base()
        {
            this.ResourceName = "flower";
            this.ResourceWhenPicked = Petals.Instance;
            this.RequiredTool = null;
            this.RequiredToolAction = "PICK";
        }

        public override void GetBonuses(WorldModelDST worldModel)
        {
            worldModel.Walter.UpdateSanity(5.0f);
        }

        public static Flower Instance { get; } = new Flower();
    }

    public sealed class Rock : BasicWorldResource
    {
        public Rock(int quantity) : base("rocks", quantity, false)
        {
            base.Recipes = new Dictionary<string, int>()
            {
                ["hammer"] = 3,
                ["firepit"] = 12,
            };
        }

        public static Rock Instance { get; } = new Rock(1);
    }

    public sealed class Flint : BasicWorldResource
    {
        public Flint(int quantity) : base("flint", quantity, false)
        {
            base.Recipes = new Dictionary<string, int>()
            {
                ["axe"] = 1,
                ["pickaxe"] = 2,
            };
        }

        public static Flint Instance { get; } = new Flint(1);
    }

    public sealed class Log : BasicWorldResource
    {
        public Log(int quantity) : base("log", quantity, true)
        {
            base.Recipes = new Dictionary<string, int>()
            {
                ["campfire"] = 2,
                ["firepit"] = 2,
            };
        }

        public static Log Instance { get; } = new Log(1);
    }

    public sealed class Twig : BasicWorldResource
    {
        public Twig(int quantity) : base("twigs", quantity, true)
        {
            base.Recipes = new Dictionary<string, int>()
            {
                ["torch"] = 2,
                ["axe"] = 1,
                ["pickaxe"] = 2,
                ["hammer"] = 3,
                // ["trap"] = 2,
            };
        }

        public static Twig Instance { get; } = new Twig(1);
    }

    public sealed class Cutgrass : BasicWorldResource
    {
        public Cutgrass(int quantity) : base("cutgrass", quantity, true)
        {
            base.Recipes = new Dictionary<string, int>()
            {
                ["torch"] = 2,
                ["hammer"] = 6,
                ["campfire"] = 3,
                // ["trap"] = 6,
            };
        }

        public static Cutgrass Instance { get; } = new Cutgrass(1);
    }

    public sealed class Torch : Tool
    {
        public Torch(int quantity) : base("torch", quantity){ }

        public static Torch Instance { get; } = new Torch(1);
    }

    public sealed class Pickaxe : Tool
    {
        public Pickaxe(int quantity) : base("pickaxe", quantity) { }

        public static Pickaxe Instance { get; } = new Pickaxe(1);
    }

    public sealed class Axe : Tool
    {
        public Axe(int quantity) : base("axe", quantity) { }

        public static Axe Instance { get; } = new Axe(1);
    }

    public sealed class Campfire : Tool
    {
        public Campfire(int quantity) : base("campfire", quantity) { }

        public static Campfire Instance { get; } = new Campfire(1);
    }

    public sealed class Hammer : Tool
    {
        public Hammer(int quantity) : base("hammer", quantity) { }

        public static Hammer Instance { get; } = new Hammer(1);
    }
}
