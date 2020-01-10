using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCTS.DST.Resources.Materials
{
    public sealed class MaterialDict
    {
        public static MaterialDict Instance { get; } = new MaterialDict();

        public Dictionary<string, Material> materialBase = new Dictionary<string, Material>()
        {
            ["boulder"] = Boulder.Instance,
        };

        private MaterialDict() { }
    }

    public class Material
    {
        public bool IsPrimitive;

        public Material(bool isPrimitive)
        {
            this.IsPrimitive = isPrimitive;
        }
    }

    public class PrimitiveMaterial : Material
    {
        public string Name { get; private set; }
        public int Quantity { get; private set; }

        public bool IsFuel { get; private set; }

        public PrimitiveMaterial(string name, int quantity, bool isFuel) : base(true)
        {
            this.Name = name;
            this.Quantity = quantity;
            this.IsFuel = isFuel;
        }
    }

    public class ComposedMaterial : Material
    {
        // Items gathered by picking up composed materials, such as boulders or trees.
        public List<PrimitiveMaterial> ComposingItems { get; private set; } = new List<PrimitiveMaterial>();

        // Items gathered by picking up by materials, that can burn.
        public List<PrimitiveMaterial> FuelItems { get; private set; } = new List<PrimitiveMaterial>();

        public ComposedMaterial() : base(false) { }

        public ComposedMaterial(List<PrimitiveMaterial> composingItems, List<PrimitiveMaterial> fuelItems) : base(false)
        {
            this.ComposingItems = composingItems;
            this.FuelItems = fuelItems;
        }
    }

    public sealed class Boulder : ComposedMaterial
    {
        public Boulder()
        {
            this.ComposingItems.Add(new Rock(2));
            this.ComposingItems.Add(new Flint(1));
        }

        public static Boulder Instance { get; } = new Boulder();
    }

    public sealed class Tree : ComposedMaterial
    {
        public Tree()
        {
            this.ComposingItems.Add(new Log(2));
            this.FuelItems.Add(new Log(2));
        }

        public static Tree Instance { get; } = new Tree();
    }

    public sealed class Sapling : ComposedMaterial
    {
        public Sapling()
        {
            this.ComposingItems.Add(new Twig(1));
            this.FuelItems.Add(new Twig(1));
        }

        public static Sapling Instance { get; } = new Sapling();
    }

    public sealed class Grass : ComposedMaterial
    {
        public Grass()
        {
            this.ComposingItems.Add(new Cutgrass(2));
            this.FuelItems.Add(new Cutgrass(2));
        }

        public static Grass Instance { get; } = new Grass();
    }

    public sealed class BerryBush : ComposedMaterial
    {
        public BerryBush()
        {
            this.ComposingItems.Add(new Berry(2));
        }

        public static BerryBush Instance { get; } = new BerryBush();
    }

    public sealed class Rock : PrimitiveMaterial
    {
        public Rock(int quantity) : base("rocks", quantity, false) {}

        public static Rock Instance { get; } = new Rock(1);
    }

    public sealed class Flint : PrimitiveMaterial
    {
        public Flint(int quantity) : base("flint", quantity, false) { }

        public static Flint Instance { get; } = new Flint(1);
    }

    public sealed class Log : PrimitiveMaterial
    {
        public Log(int quantity) : base("log", quantity, true) { }

        public static Log Instance { get; } = new Log(1);
    }

    public sealed class Twig : PrimitiveMaterial
    {
        public Twig(int quantity) : base("twigs", quantity, true) { }

        public static Twig Instance { get; } = new Twig(1);
    }

    public sealed class Cutgrass : PrimitiveMaterial
    {
        public Cutgrass(int quantity) : base("cutgrass", quantity, true) { }

        public static Cutgrass Instance { get; } = new Cutgrass(1);
    }

    public sealed class Berry : PrimitiveMaterial
    {
        public Berry(int quantity) : base("berries", quantity, true) { }

        public static Berry Instance { get; } = new Berry(1);
    }

    public sealed class Carrot : PrimitiveMaterial
    {
        public Carrot(int quantity) : base("carrot", quantity, true) { }

        public static Carrot Instance { get; } = new Carrot(1);
    }
}
