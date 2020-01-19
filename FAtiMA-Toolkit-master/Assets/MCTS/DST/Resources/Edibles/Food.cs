using MCTS.DST.WorldModels;
using System.Collections.Generic;

namespace MCTS.DST.Resources.Edibles
{
    public sealed class FoodDict
    {
        public static FoodDict Instance { get; } = new FoodDict();

        private FoodDict()
        {
        }

        public Dictionary<string, Food> foodBase = new Dictionary<string, Food>()
        {
            ["berries"] = Berries.Instance,
            ["blue_cap"] = BlueCap.Instance,
            ["carrot"] = Carrot.Instance,
            ["green_cap"] = GreenCap.Instance,
            ["berries_juicy"] = JuicyBerries.Instance,
            ["petals"] = Petals.Instance,
            ["red_cap"] = RedCap.Instance,
            ["seeds"] = Seeds.Instance,
            /*
            ["fish_cooked"] = CookedFish.Instance,
            ["watermelon"] = Watermelon.Instance,
            ["watermelon_cooked"] = CookedWatermelon.Instance,
            ["seeds_cooked"] = CookedSeeds.Instance,
            ["aspargus"] = Asparagus.Instance,
            ["asparagus_cooked"] = CookedAsparagus.Instance,
            ["berries_cooked"] = RoastedBerries.Instance,
            ["acorn_cooked"] = RoastedBirchnut.Instance,
            ["butterflywings"] = ButterflyWings.Instance,
            ["cactus_meat"] = CactusFlesh.Instance,
            ["cactus_meat_cooked"] = CookedCactusFlesh.Instance,
            ["cactus_flower"] = CactusFlower.Instance,
            ["carrot_cooked"] = RoastedCarrot.Instance,
            ["fish_raw_small"] = FishMorsel.Instance,
            ["fish_raw_small_cooked"] = CookedFishMorsel.Instance,
            ["froglegs"] = FrogLegs.Instance,
            ["froglegs_cooked"] = CookedFrogLegs.Instance,
            ["berries_juicy_cooked"] = RoastedJuicyBerries.Instance,
            ["plantmeat"] = LeafyMeat.Instance,
            ["cooked_leafy_meant"] = CookedLeafyMeat.Instance,
            ["mandrake_active"] = Mandrake.Instance,
            ["mandrake_cooked"] = CookedMandrake.Instance,
            ["meat"] = Meat.Instance,
            ["cookedmeat"] = CookedMeat.Instance,
            ["meat_dried"] = Jerky.Instance,
            ["monstermeat"] = MonsterMeat.Instance,
            ["cookedmonstermeat"] = CookedMonsterMeat.Instance,
            ["smallmeat"] = Morsel.Instance,
            ["cookedsmallmeat"] = CookedMorsel.Instance,
            ["blue_cap_cooked"] = CookedBlueCap.Instance,
            ["green_cap_cooked"] = CookedGreenCap.Instance,
            ["red_cap_cooked"] = CookedRedCap.Instance,
            ["fish"] = Fish.Instance,
            ["fish_med_cooked"] = FishSteak.Instance,
            */
        };
    }

    public class Food
    {
        protected float HP { get; private set; }
        protected float Satiation { get; private set; }
        protected float Sanity { get; private set; }
        public string FoodName { get; private set; }

        public Food(float hp, float satiation, float sanity, string name)
        {
            HP = hp;
            Satiation = satiation;
            Sanity = sanity;
            FoodName = name;
        }

        public void Eat(WorldModelDST worldModel)
        {
            worldModel.RemoveFromPossessedItems(this.FoodName, 1);
            worldModel.UpdateSatiation(this.Satiation);
            worldModel.UpdateHP(this.HP);
            worldModel.UpdateSanity(this.Sanity);
        }

        public void TryRemoveAction(WorldModelDST worldModel, string actionName)
        {
            if (!worldModel.Possesses(this.FoodName))
            {
                worldModel.RemoveAction(string.Concat(actionName, this.FoodName));
            }
        }
    }
    public sealed class Asparagus : Food
    {
        private Asparagus(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new Asparagus(3.0f, 12.5f, 0.0f, "asparagus");
    }

    public sealed class CookedAsparagus : Food
    {
        private CookedAsparagus(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CookedAsparagus(3.0f, 25.0f, 0.0f, "asparagus_cooked");
    }

    public sealed class Berries : Food
    {
        private Berries(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new Berries(0.0f, 9.375f, 0.0f, "berries");
    }

    public sealed class RoastedBerries : Food
    {
        private RoastedBerries(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new RoastedBerries(1.0f, 12.5f, 0.0f, "berries_cooked");
    }

    public sealed class RoastedBirchnut : Food
    {
        private RoastedBirchnut(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new RoastedBirchnut(1.0f, 9.375f, 0.0f, "acorn_cooked");
    }

    public sealed class ButterflyWings : Food
    {
        private ButterflyWings(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new ButterflyWings(8.0f, 9.375f, 0.0f, "butterflywings");
    }

    public sealed class CactusFlesh : Food
    {
        private CactusFlesh(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CactusFlesh(-3.0f, 12.5f, -5.0f, "cactus_meat");
    }

    public sealed class CookedCactusFlesh : Food
    {
        private CookedCactusFlesh(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CookedCactusFlesh(1.0f, 12.5f, 15.0f, "cactus_meat_cooked");
    }

    public sealed class CactusFlower : Food
    {
        private CactusFlower(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CactusFlower(8.0f, 12.5f, 5.0f, "cactus_flower");
    }

    public sealed class Carrot : Food
    {
        private Carrot(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new Carrot(0.0f, 12.5f, 0.0f, "carrot");
    }

    public sealed class RoastedCarrot : Food
    {
        private RoastedCarrot(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new RoastedCarrot(3.0f, 12.5f, 0.0f, "carrot_cooked");
    }

    public sealed class FishMorsel : Food
    {
        private FishMorsel(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new FishMorsel(1.0f, 12.5f, 0.0f, "fish_raw_small");
    }

    public sealed class CookedFishMorsel : Food
    {
        private CookedFishMorsel(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CookedFishMorsel(1.0f, 12.5f, 0.0f, "fish_raw_small_cooked");
    }

    public sealed class FrogLegs : Food
    {
        private FrogLegs(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new FrogLegs(1.0f, 12.5f, -10.0f, "froglegs");
    }

    public sealed class CookedFrogLegs : Food
    {
        private CookedFrogLegs(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CookedFrogLegs(1.0f, 12.5f, 0.0f, "froglegs_cooked");
    }

    public sealed class JuicyBerries : Food
    {
        private JuicyBerries(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new JuicyBerries(1.0f, 12.5f, 0.0f, "berries_juicy");
    }

    public sealed class RoastedJuicyBerries : Food
    {
        private RoastedJuicyBerries(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new RoastedJuicyBerries(3.0f, 18.75f, 0.0f, "berries_juicy_cooked");
    }

    public sealed class LeafyMeat : Food
    {
        private LeafyMeat(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new LeafyMeat(0, 12.5f, -10.0f, "plantmeat");
    }

    public sealed class CookedLeafyMeat : Food
    {
        private CookedLeafyMeat(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CookedLeafyMeat(1.0f, 18.75f, 0.0f, "plantmeat_cooked");
    }

    public sealed class Mandrake : Food
    {
        private Mandrake(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new Mandrake(60.0f, 75.0f, 0.0f, "mandrake_active");
    }

    public sealed class CookedMandrake : Food
    {
        private CookedMandrake(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CookedMandrake(100.0f, 150.0f, 0.0f, "mandrake_cooked");
    }

    public sealed class Meat : Food
    {
        private Meat(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new Meat(1.0f, 25.0f, -10.0f, "meat");
    }

    public sealed class CookedMeat : Food
    {
        private CookedMeat(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CookedMeat(3.0f, 25.0f, 0.0f, "cookedmeat");
    }

    public sealed class Jerky : Food
    {
        private Jerky(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new Jerky(20.0f, 25.0f, 15.0f, "meat_dried");
    }

    public sealed class MonsterMeat : Food
    {
        private MonsterMeat(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new MonsterMeat(-20.0f, 18.75f, -15.0f, "monstermeat");
    }

    public sealed class CookedMonsterMeat : Food
    {
        private CookedMonsterMeat(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CookedMonsterMeat(-3.0f, 18.75f, -10.0f, "cookedmonstermeat");
    }

    public sealed class Morsel : Food
    {
        private Morsel(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new Morsel(0.0f, 12.5f, -10.0f, "smallmeat");
    }

    public sealed class CookedMorsel : Food
    {
        private CookedMorsel(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CookedMorsel(1.0f, 18.75f, 0.0f, "cookedsmallmeat");
    }

    public sealed class BlueCap : Food
    {
        private BlueCap(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new BlueCap(20.0f, 12.5f, -15.0f, "blue_cap");
    }

    public sealed class CookedBlueCap : Food
    {
        private CookedBlueCap(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CookedBlueCap(-3.0f, 0.0f, 10.0f, "blue_cap_cooked");
    }

    public sealed class GreenCap : Food
    {
        private GreenCap(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new GreenCap(0.0f, 12.5f, -50.0f, "green_cap");
    }

    public sealed class CookedGreenCap : Food
    {
        private CookedGreenCap(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CookedGreenCap(-1.0f, 0.0f, 15.0f, "green_cap_cooked");
    }

    public sealed class RedCap : Food
    {
        private RedCap(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new RedCap(-20.0f, 12.5f, 0.0f, "red_cap");
    }

    public sealed class CookedRedCap : Food
    {
        private CookedRedCap(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CookedRedCap(1.0f, 0.0f, -10.0f, "red_cap_cooked");
    }

    public sealed class Petals : Food
    {
        private Petals(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new Petals(1.0f, 0.0f, 0.0f, "petals");
    }

    public sealed class Fish : Food
    {
        private Fish(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new Fish(1.0f, 12.5f, 0.0f, "fish");
    }

    public sealed class CookedFish : Food
    {
        private CookedFish(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CookedFish(1.0f, 12.5f, 0.0f, "fish_cooked");
    }

    public sealed class FishSteak : Food
    {
        private FishSteak(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new FishSteak(20.0f, 25.0f, 0.0f, "fish_med_cooked");
    }

    public sealed class Watermelon : Food
    {
        private Watermelon(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new Watermelon(3.0f, 12.5f, 5.0f, "watermelon");
    }

    public sealed class CookedWatermelon : Food
    {
        private CookedWatermelon(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CookedWatermelon(1.0f, 12.5f, 7.5f, "watermelon_cooked");
    }
    
    public sealed class Seeds : Food
    {
        private Seeds(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new Seeds(0.0f, 4.6875f, 0.0f, "seeds");
    }

    public sealed class CookedSeeds : Food
    {
        private CookedSeeds(float hp, float satiation, float sanity, string name) : base(hp, satiation, sanity, name) { }

        public static Food Instance { get; } = new CookedSeeds(0.0f, 4.6875f, 0.0f, "seeds_cooked");
    }

}
