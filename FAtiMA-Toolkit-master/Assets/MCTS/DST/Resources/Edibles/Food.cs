﻿using System;
using System.Collections.Generic;
using System.Runtime;
using Utilities;

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
            ["aspargus"] = Asparagus.Instance,
            ["cooked_aspargus"] = CookedAsparagus.Instance,
            ["berries"] = Berries.Instance,
            ["roasted_berries"] = RoastedBerries.Instance,
            ["roasted_birchnut"] = RoastedBirchnut.Instance,
            ["butterfly_wings"] = ButterflyWings.Instance,
            ["cactus_flesh"] = CactusFlesh.Instance,
            ["cooked_cactus_flesh"] = CookedCactusFlesh.Instance,
            ["cactus_flower"] = CactusFlower.Instance,
            ["carrot"] = Carrot.Instance,
            ["roasted_carrot"] = RoastedCarrot.Instance,
            ["fish_morsel"] = FishMorsel.Instance,
            ["cooked_fish_morsel"] = CookedFishMorsel.Instance,
            ["frog_legs"] = FrogLegs.Instance,
            ["cooked_frog_legs"] = CookedFrogLegs.Instance,
            ["juicy_berries"] = JuicyBerries.Instance,
            ["roasted_juicy_berries"] = RoastedJuicyBerries.Instance,
            ["leafy_meat"] = LeafyMeat.Instance,
            ["cooked_leafy_meant"] = CookedLeafyMeat.Instance,
            ["mandrake"] = Mandrake.Instance,
            ["cooked_mandrake"] = CookedMandrake.Instance,
            ["meat"] = Meat.Instance,
            ["cooked_meat"] = CookedMeat.Instance,
            ["jerky"] = Jerky.Instance,
            ["monster_meat"] = MonsterMeat.Instance,
            ["cooked_monster_meat"] = CookedMonsterMeat.Instance,
            ["morsel"] = Morsel.Instance,
            ["cooked_morsel"] = CookedMorsel.Instance,
            ["blue_cap"] = BlueCap.Instance,
            ["cooked_blue_cap"] = CookedBlueCap.Instance,
            ["green_cap"] = GreenCap.Instance,
            ["cooked_green_cap"] = CookedGreenCap.Instance,
            ["red_cap"] = RedCap.Instance,
            ["cooked_red_cap"] = CookedRedCap.Instance,
            ["petals"] = Petals.Instance,
            ["fish"] = Fish.Instance,
            ["fish_steak"] = FishSteak.Instance,
            ["watermelon"] = Watermelon.Instance,
        };
    }

    public class Food
    {
        public float HP { get; private set; }
        public float Satiation { get; private set; }
        public float Sanity { get; private set; }

        public Food(float hp, float hunger, float sanity)
        {
            HP = hp;
            Satiation = hunger;
            Sanity = sanity;
        }
    }

    public sealed class Asparagus : Food
    {
        private Asparagus(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new Asparagus(3.0f, 12.5f, 0.0f);
    }

    public sealed class CookedAsparagus : Food
    {
        private CookedAsparagus(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new CookedAsparagus(3.0f, 25.0f, 0.0f);
    }

    public sealed class Berries : Food
    {
        private Berries(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new Berries(0.0f, 9.375f, 0.0f);
    }

    public sealed class RoastedBerries : Food
    {
        private RoastedBerries(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new RoastedBerries(1.0f, 12.5f, 0.0f);
    }

    public sealed class RoastedBirchnut : Food
    {
        private RoastedBirchnut(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new RoastedBirchnut(1.0f, 9.375f, 0.0f);
    }

    public sealed class ButterflyWings : Food
    {
        private ButterflyWings(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new ButterflyWings(8.0f, 9.375f, 0.0f);
    }

    public sealed class CactusFlesh : Food
    {
        private CactusFlesh(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new CactusFlesh(-3.0f, 12.5f, -5.0f);
    }

    public sealed class CookedCactusFlesh : Food
    {
        private CookedCactusFlesh(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new CookedCactusFlesh(1.0f, 12.5f, 15.0f);
    }

    public sealed class CactusFlower : Food
    {
        private CactusFlower(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new CactusFlower(8.0f, 12.5f, 5.0f);
    }

    public sealed class Carrot : Food
    {
        private Carrot(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new Carrot(0.0f, 12.5f, 0.0f);
    }

    public sealed class RoastedCarrot : Food
    {
        private RoastedCarrot(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new RoastedCarrot(3.0f, 12.5f, 0.0f);
    }

    public sealed class FishMorsel : Food
    {
        private FishMorsel(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new FishMorsel(1.0f, 12.5f, 0.0f);
    }

    public sealed class CookedFishMorsel : Food
    {
        private CookedFishMorsel(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new CookedFishMorsel(1.0f, 12.5f, 0.0f);
    }

    public sealed class FrogLegs : Food
    {
        private FrogLegs(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new FrogLegs(1.0f, 12.5f, -10.0f);
    }

    public sealed class CookedFrogLegs : Food
    {
        private CookedFrogLegs(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new CookedFrogLegs(1.0f, 12.5f, 0.0f);
    }

    public sealed class JuicyBerries : Food
    {
        private JuicyBerries(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new JuicyBerries(1.0f, 12.5f, 0.0f);
    }

    public sealed class RoastedJuicyBerries : Food
    {
        private RoastedJuicyBerries(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new RoastedJuicyBerries(3.0f, 18.75f, 0.0f);
    }

    public sealed class LeafyMeat : Food
    {
        private LeafyMeat(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new LeafyMeat(0, 12.5f, -10.0f);
    }

    public sealed class CookedLeafyMeat : Food
    {
        private CookedLeafyMeat(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new CookedLeafyMeat(1.0f, 18.75f, 0.0f);
    }

    public sealed class Mandrake : Food
    {
        private Mandrake(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new Mandrake(60.0f, 75.0f, 0.0f);
    }

    public sealed class CookedMandrake : Food
    {
        private CookedMandrake(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new CookedMandrake(100.0f, 150.0f, 0.0f);
    }

    public sealed class Meat : Food
    {
        private Meat(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new Meat(1.0f, 25.0f, -10.0f);
    }

    public sealed class CookedMeat : Food
    {
        private CookedMeat(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new CookedMeat(3.0f, 25.0f, 0.0f);
    }

    public sealed class Jerky : Food
    {
        private Jerky(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new Jerky(20.0f, 25.0f, 15.0f);
    }

    public sealed class MonsterMeat : Food
    {
        private MonsterMeat(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new MonsterMeat(-20.0f, 18.75f, -15.0f);
    }

    public sealed class CookedMonsterMeat : Food
    {
        private CookedMonsterMeat(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new CookedMonsterMeat(-3.0f, 18.75f, -10.0f);
    }

    public sealed class Morsel : Food
    {
        private Morsel(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new Morsel(0.0f, 12.5f, -10.0f);
    }

    public sealed class CookedMorsel : Food
    {
        private CookedMorsel(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new CookedMorsel(1.0f, 18.75f, 0.0f);
    }

    public sealed class BlueCap : Food
    {
        private BlueCap(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new BlueCap(20.0f, 12.5f, -15.0f);
    }

    public sealed class CookedBlueCap : Food
    {
        private CookedBlueCap(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new CookedBlueCap(-3.0f, 0.0f, 10.0f);
    }

    public sealed class GreenCap : Food
    {
        private GreenCap(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new GreenCap(0.0f, 12.5f, -50.0f);
    }

    public sealed class CookedGreenCap : Food
    {
        private CookedGreenCap(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new CookedGreenCap(-1.0f, 0.0f, 15.0f);
    }

    public sealed class RedCap : Food
    {
        private RedCap(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new RedCap(-20.0f, 12.5f, 0.0f);
    }

    public sealed class CookedRedCap : Food
    {
        private CookedRedCap(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new CookedRedCap(1.0f, 0.0f, -10.0f);
    }

    public sealed class Petals : Food
    {
        private Petals(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new Petals(1.0f, 0.0f, 0.0f);
    }

    public sealed class Fish : Food
    {
        private Fish(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new Fish(1.0f, 25.0f, 0.0f);
    }

    public sealed class FishSteak : Food
    {
        private FishSteak(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new FishSteak(20.0f, 25.0f, 0.0f);
    }

    public sealed class Watermelon : Food
    {
        private Watermelon(float hp, float hunger, float sanity) : base(hp, hunger, sanity) { }

        public static Food Instance { get; } = new Watermelon(3.0f, 12.5f, 5.0f);
    }
}
