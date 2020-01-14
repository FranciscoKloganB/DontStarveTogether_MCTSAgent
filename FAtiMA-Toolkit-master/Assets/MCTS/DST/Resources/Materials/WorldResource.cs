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
            ["berry_bush"] = Grass.Instance,
            ["rocks"] = Rock.Instance,
            ["flint"] = Flint.Instance,
            ["log"] = Log.Instance,
            ["twigs"] = Twig.Instance,
            ["cutgrass"] = Cutgrass.Instance,
            ["berry"] = Berry.Instance,
            ["carrot"] = Carrot.Instance,
        };

        private MaterialDict() { }
    }

    public class WorldResource
    {
        public bool IsPrimitive { get; private set; }
        public bool IsFuel { get; private set; }
        public bool IsPickable { get; private set; }

        public WorldResource(bool isPrimitive, bool isFuel, bool isPickable)
        {
            this.IsPrimitive = isPrimitive;
            this.IsFuel = isFuel;
            this.IsPickable = isPickable;
        }
    }

    public class BasicWorldResource : WorldResource
    {
        public int Quantity { get; private set; }
        public string MaterialName { get; private set; }

        public Dictionary<string, int> Recipes { get; protected set; }

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

        public CompoundWorldResource() : base(false) { }

        public CompoundWorldResource(List<BasicWorldResource> composingItems, List<BasicWorldResource> fuelItems) : base(false, false, false)
        {
            this.ComposingItems = composingItems;
            this.FuelItems = fuelItems;
        }
    }

    public sealed class Boulder : CompoundWorldResource
    {
        public Boulder()
        {
            this.ComposingItems.Add(new Rock(2));
            this.ComposingItems.Add(new Flint(1));
        }

        public static Boulder Instance { get; } = new Boulder();
    }

    public sealed class Tree : CompoundWorldResource
    {
        public Tree()
        {
            this.IsFuel = true;
            this.ComposingItems.Add(new Log(2));
            this.FuelItems.Add(new Log(2));
        }

        public static Tree Instance { get; } = new Tree();
    }

    public sealed class Sapling : CompoundWorldResource
    {
        public Sapling()
        {
            this.ComposingItems.Add(new Twig(1));
            this.FuelItems.Add(new Twig(1));
        }

        public static Sapling Instance { get; } = new Sapling();
    }

    public sealed class Grass : CompoundWorldResource
    {
        public Grass()
        {
            this.ComposingItems.Add(new Cutgrass(2));
            this.FuelItems.Add(new Cutgrass(2));
        }

        public static Grass Instance { get; } = new Grass();
    }

    public sealed class BerryBush : CompoundWorldResource
    {
        public BerryBush()
        {
            this.ComposingItems.Add(new Berry(2));
        }

        public static BerryBush Instance { get; } = new BerryBush();
    }

    public sealed class Rock : BasicWorldResource
    {
        public Rock(int quantity) : base("rocks", quantity, false)
        {
            base.Recipes = new Dictionary<string, int>()
            {
                ["firepit"] = 12,
                ["hammer"] = 3,
                ["battle_helm"] = 2
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
                ["battle_helm"] = 2,
                ["battle_spear"] = 2,
                ["compass"] = 1,
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
                ["axe"] = 1,
                ["pickaxe"] = 2,
                ["hammer"] = 3,
                ["torch"] = 2,
                ["trap"] = 2,
                ["grass_suit"] = 2,
                ["battle_spear"] = 2,
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
                ["campfire"] = 3,
                ["torch"] = 2,
                ["trap"] = 6,
                ["hammer"] = 6,
                ["grass_suit"] = 10,
            };
        }

        public static Cutgrass Instance { get; } = new Cutgrass(1);
    }

    public sealed class Berry : BasicWorldResource
    {
        public Berry(int quantity) : base("berries", quantity, true)
        {
            base.Recipes = new Dictionary<string, int>()
            {
                ["roasted_berries"] = 1,
            };
        }

        public static Berry Instance { get; } = new Berry(1);
    }

    public sealed class Carrot : BasicWorldResource
    {
        public Carrot(int quantity) : base("carrot", quantity, true)
        {
            Recipes = new Dictionary<string, int>()
            {
                ["roasted_carrot"] = 1,
            };
        }

        public static Carrot Instance { get; } = new Carrot(1);
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
