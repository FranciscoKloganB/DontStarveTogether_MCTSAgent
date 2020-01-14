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
        private Dictionary<string, Buildable> BuildableBase { get; } = BuildablesDict.Instance.buildableBase;
        private static readonly float duration = 0.05f;
        private static readonly string actionName = "Construct_";
        private readonly string Target;

        public Construct(string target) : base(string.Concat(actionName, target))
        {
            this.Target = target;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            try
            {
                Buildable targetBuildable = BuildableBase[this.Target];
                targetBuildable.Build(worldState);
                targetBuildable.PostProcessBuildable(worldState);
                targetBuildable.TryRemoveAction(worldState, actionName);
                worldState.Cycle += duration;
            }
            catch (KeyNotFoundException)
            {
                ;
            }
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            if (this.Target.Equals("campfire") || this.Target.Equals("firepit") || this.Target.Equals("endothermic_firepit"))
            {
                var x = preWorldState.Walter.GetPosX();
                var z = preWorldState.Walter.GetPosZ();
                return new List<Pair<string, string>>(1)
            {
                new Pair<string, string>("Action(BUILD, -, " + x + ", " + z + ", " + this.Target +")", "-")
            };
            }

            return new List<Pair<string, string>>(1)
            {
                new Pair<string, string>("Action(BUILD, -, -, -, " + this.Target +")", "-")
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
