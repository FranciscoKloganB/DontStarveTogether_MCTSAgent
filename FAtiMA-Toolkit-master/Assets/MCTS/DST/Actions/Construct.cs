﻿using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST.Resources.Buildables;
using MCTS.DST;


namespace MCTS.DST.Actions
{

    public class Construct : ActionDST
    {
        private static readonly float duration = 0.05f;
        private static readonly string actionName = "Construct_";
        private readonly string target;

        public Construct(string target) : base(string.Concat(actionName, target))
        {
            this.target = target;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            if (BuildablesDict.Instance.buildableBase.ContainsKey(this.target))
            {
                worldState.Cycle += duration;
                Buildable targetBuildable = BuildablesDict.Instance.buildableBase[this.target];
                if (targetBuildable.Build(worldState))
                {
                    string buildableName = targetBuildable.BuildableName;
                    worldState.AddToPossessedItems(targetBuildable.BuildableName, 1);
                    targetBuildable.PostProcessBuildable(worldState);
                    targetBuildable.TryRemoveAction(worldState);

                    if (worldState.IsEquipped(targetBuildable.BuildableName))
                    { // Item constructed went to one of the equipped slots.
                        worldState.AddAction(new Unequip(buildableName));
                    }
                    else
                    { // The item is in the inventory, but not equipped.
                        worldState.AddAction(new Equip(buildableName));
                    }
                }
            }
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            if (this.target.Equals("campfire") || this.target.Equals("firepit"))
            {
                var x = preWorldState.Walter.GetPosX();
                var z = preWorldState.Walter.GetPosZ();
                return new List<Pair<string, string>>(1)
                {
                    new Pair<string, string>("Action(BUILD, -, " + x + ", " + z + ", " + this.target +")", "-")
                };
            }

            return new List<Pair<string, string>>(1)
            {
                new Pair<string, string>("Action(BUILD, -, -, -, " + this.target +")", "-")
            };
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
