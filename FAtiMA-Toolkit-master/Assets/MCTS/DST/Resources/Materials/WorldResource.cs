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
            ["flower"] = Flower.Instance,
        };

        private MaterialDict() { }
    }

    public class WorldResource
    {
        public bool IsPrimitive { get; private set; }
        public bool IsFuel { get; private set; }
        public bool IsPickable { get; private set; }
        public Dictionary<string, int> Recipes { get; protected set; }

        public WorldResource(bool isPrimitive, bool isFuel, bool isPickable)
        {
            this.IsPrimitive = isPrimitive;
            this.IsFuel = isFuel;
            this.IsPickable = isPickable;
            this.Recipes = new Dictionary<string, int>();
        }

        public virtual void GetBonuses(WorldModelDST worldModel) { }
    }

    public class BasicWorldResource : WorldResource
    {
        public int Quantity { get; private set; }
        public string MaterialName { get; private set; }

        public BasicWorldResource(string name, int quantity, bool isFuel) : base(true, isFuel, true)
        {
            this.MaterialName = name;
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
        // Items gathered by picking up composed materials, such as boulders or trees.
        public List<BasicWorldResource> ComposingItems { get; private set; } = new List<BasicWorldResource>();

        // Items gathered by picking up by materials, that can burn.
        public List<BasicWorldResource> FuelItems { get; private set; } = new List<BasicWorldResource>();

        public Tool RequiredTool { get; protected set; }
        public string RequiredToolAction { get; protected set; }

        public CompoundWorldResource() : base(false, false, false) { }

        public CompoundWorldResource(bool isFuel) : base(false, isFuel, false) { }

        public CompoundWorldResource(List<BasicWorldResource> composingItems, List<BasicWorldResource> fuelItems) : base(false, false, false)
        {
            this.ComposingItems = composingItems;
            this.FuelItems = fuelItems;
        }
    }

    public class GatherableCompoundWorldResource : CompoundWorldResource
    {
        public Object ResourceWhenPicked { get; protected set; }

        public GatherableCompoundWorldResource() : base() {  }

        public GatherableCompoundWorldResource(bool isFuel) : base(isFuel) { }

        public GatherableCompoundWorldResource(List<BasicWorldResource> composingItems, List<BasicWorldResource> fuelItems) : base(composingItems, fuelItems) { }
    }

    public sealed class Boulder : CompoundWorldResource
    {
        public Boulder()
        {
            this.ComposingItems.Add(new Rock(2));
            this.ComposingItems.Add(new Flint(1));
            this.RequiredTool = Pickaxe.Instance;
            this.RequiredToolAction = "MINE";
        }

        public static Boulder Instance { get; } = new Boulder();
    }

    public sealed class Tree : CompoundWorldResource
    {
        public Tree() : base(true)
        {
            this.ComposingItems.Add(new Log(2));
            this.FuelItems.Add(new Log(2));
            this.RequiredTool = Axe.Instance;
            this.RequiredToolAction = "CHOP";
        }

        public static Tree Instance { get; } = new Tree();
    }

    public sealed class Sapling : GatherableCompoundWorldResource
    {
        public Sapling()
        {
            this.ResourceWhenPicked = Twig.Instance;
            this.ComposingItems.Add(new Twig(1));
            this.FuelItems.Add(new Twig(1));
            this.RequiredTool = null;
            this.RequiredToolAction = "PICK";
        }

        public static Sapling Instance { get; } = new Sapling();
    }

    public sealed class Grass : GatherableCompoundWorldResource
    {
        public Grass()
        {
            this.ResourceWhenPicked = Cutgrass.Instance;
            this.ComposingItems.Add(new Cutgrass(2));
            this.FuelItems.Add(new Cutgrass(2));
            this.RequiredTool = null;
            this.RequiredToolAction = "PICK";
        }

        public static Grass Instance { get; } = new Grass();
    }

    public sealed class BerryBush : GatherableCompoundWorldResource
    {
        public BerryBush()
        {
            this.ResourceWhenPicked = Berries.Instance;
            this.RequiredTool = null;
            this.RequiredToolAction = "PICK";
        }

        public static BerryBush Instance { get; } = new BerryBush();
    }

    public sealed class Flower : GatherableCompoundWorldResource
    {
        public Flower()
        {
            this.ResourceWhenPicked = Petals.Instance;
            this.RequiredTool = null;
            this.RequiredToolAction = "PICK";
        }

        public void GetBonuses(WorldModelDST worldModel)
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
