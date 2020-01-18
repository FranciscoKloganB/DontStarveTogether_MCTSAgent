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
                    targetBuildable.PostProcessBuildable(worldState);
                    targetBuildable.TryRemoveAction(worldState);
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
