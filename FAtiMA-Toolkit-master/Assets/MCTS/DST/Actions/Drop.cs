using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;
using MCTS.DST.Resources.Materials;


namespace MCTS.DST.Actions
{

    public class Drop : ActionDST
    {
        public string Target;
        public float Duration;
        private static readonly string actionName = "Drop_";

        public Drop(string target) : base(actionName + target)
        {
            this.Target = target;
            this.Duration = 0.33f;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            worldState.Cycle += this.Duration;
            worldState.RemoveFromEquipped(this.Target);
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            var x = preWorldState.Walter.GetPosX();
            var z = preWorldState.Walter.GetPosZ();
            return new List<Pair<string, string>>(1)
            {
                new Pair<string, string>("Action(DROP, " + preWorldState.GetEquippedGUID(this.Target).ToString() + ", " + x + ", " + z + ", -)", "-")
            };
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
