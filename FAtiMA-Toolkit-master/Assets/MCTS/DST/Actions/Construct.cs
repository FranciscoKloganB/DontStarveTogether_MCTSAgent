using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST.Resources.Buildables;
using MCTS.DST;


namespace MCTS.DST.Actions
{

    public class Construct : ActionDST
    {
        private Dictionary<string, Buildable> ItemBase { get; } = BuildablesDict.Instance.buildableBase;
        private static readonly float duration = 0.05f;
        private static readonly string actionName = "Build_";
        private readonly string Target;

        public Construct(string target) : base(string.Concat(actionName, target))
        {
            this.Target = target;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            try
            {
                Buildable targetBuildable = ItemBase[this.Target];
                targetBuildable.Build(worldState);
                targetBuildable.TryMakeUnequipable(worldState);
                targetBuildable.TryRemoveAction(worldState, actionName);
                worldState.Cycle += duration;
            }
            catch (KeyNotFoundException)
            {
                ;
            }

            if (this.Target == "torch")
            {
                worldState.RemoveFromPossessedItems("twigs", 2);
                worldState.RemoveFromPossessedItems("cutgrass", 2);
                worldState.AddToPossessedItems("torch", 1);
                if (!worldState.Possesses("twigs", 2))
                    worldState.RemoveAction("Build_pickaxe");
                if (!worldState.Possesses("twigs", 1))
                    worldState.RemoveAction("Build_axe");
                if (!worldState.Possesses("twigs", 2) || !worldState.Possesses("cutgrass", 2))
                    worldState.RemoveAction("Build_torch");
                if (worldState.EquippedItems.Count != 0)
                    return;
                ActionDST action = (ActionDST)new Unequip("torch");
                worldState.AddAction(action);
            }
            else if (this.Target == "pickaxe")
            {
                worldState.RemoveFromPossessedItems("twigs", 2);
                worldState.RemoveFromPossessedItems("flint", 2);
                worldState.AddToPossessedItems("pickaxe", 1);
                if (!worldState.Possesses("twigs", 2) || !worldState.Possesses("flint", 2))
                    worldState.RemoveAction("Build_pickaxe");
                if (!worldState.Possesses("twigs", 1) || !worldState.Possesses("flint", 1))
                    worldState.RemoveAction("Build_axe");
                if (worldState.Possesses("twigs", 2))
                    return;
                worldState.RemoveAction("Build_torch");
            }
            else if (this.Target == "axe")
            {
                worldState.RemoveFromPossessedItems("twigs", 1);
                worldState.RemoveFromPossessedItems("flint", 1);
                worldState.AddToPossessedItems("axe", 1);
                if (!worldState.Possesses("twigs", 2) || !worldState.Possesses("flint", 2))
                    worldState.RemoveAction("Build_pickaxe");
                if (!worldState.Possesses("twigs", 1) || !worldState.Possesses("flint", 1))
                    worldState.RemoveAction("Build_axe");
                if (worldState.Possesses("twigs", 2))
                    return;
                worldState.RemoveAction("Build_torch");
            }
            else if (this.Target == "campfire")
            {
                worldState.RemoveFromPossessedItems("log", 2);
                worldState.RemoveFromPossessedItems("cutgrass", 3);
                worldState.AddToWorld("campfire", 1, worldState.Walter.Position.Item1, worldState.Walter.Position.Item2);
                worldState.AddToFire("campfire", worldState.Walter.Position.Item1, worldState.Walter.Position.Item2);
                if (!worldState.Possesses("log", 2))
                    worldState.RemoveAction("Build_firepit");
                if (!worldState.Possesses("log", 2) || !worldState.Possesses("cutgrass", 3))
                    worldState.RemoveAction("Build_campfire");
                if (worldState.Possesses("cutgrass", 2))
                    return;
                worldState.RemoveAction("Build_torch");
            }
            else
            {
                if (!(this.Target == "firepit"))
                    return;
                worldState.RemoveFromPossessedItems("log", 2);
                worldState.RemoveFromPossessedItems("rocks", 12);
                worldState.AddToWorld("firepit", 1, worldState.Walter.Position.Item1, worldState.Walter.Position.Item2);
                worldState.AddToFire("firepit", worldState.Walter.Position.Item1, worldState.Walter.Position.Item2);
                if (!worldState.Possesses("log", 2) || !worldState.Possesses("rocks", 12))
                    worldState.RemoveAction("Build_firepit");
                if (worldState.Possesses("log", 2))
                    return;
                worldState.RemoveAction("Build_campfire");
            }
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            return base.Decompose(preWorldState);
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
