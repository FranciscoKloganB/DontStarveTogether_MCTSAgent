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
        public List<Material> ComposingItems { get; private set; } = new List<Material>();

        // Items gathered by picking up by materials, that can burn.
        public List<Material> FuelItems { get; private set; } = new List<Material>();

        public ComposedMaterial() : base(false) { }

        public ComposedMaterial(List<Material> composingItems, List<Material> fuelItems) : base(false)
        {
            this.ComposingItems = composingItems;
            this.FuelItems = fuelItems;
        }
    }

    public sealed class Boulder : ComposedMaterial
    {
        public Boulder()
        {
            List<Material> composingItems = new List<Material>();
            composingItems.Add(new Rocks(2));
            composingItems.Add(new Flint(1));
            List<Material> fuelItems = new List<Material>();
        }

        public static Boulder Instance { get; } = new Boulder();
    }

    public sealed class Rocks : PrimitiveMaterial
    {
        public Rocks(int quantity) : base("rocks", quantity, false) {}

        public static Rocks Instance { get; } = new Rocks(1);
    }

    public sealed class Flint : PrimitiveMaterial
    {
        public Flint(int quantity) : base("flint", quantity, false) { }

        public static Rocks Instance { get; } = new Rocks(1);
    }
}
