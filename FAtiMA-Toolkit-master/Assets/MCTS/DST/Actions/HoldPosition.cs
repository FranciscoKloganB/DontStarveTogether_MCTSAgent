using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;


namespace MCTS.DST.Actions
{

    public class HoldPosition : ActionDST
    {
        private string Target;
        private float Duration;
        private static readonly string ActionName = "HoldPosition_";

        public HoldPosition(string target) : base(ActionName + target)
        {
            this.Target = target;
            this.Duration = 0.33f;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            worldState.Cycle += this.Duration;
            worldState.UpdateSatiation(-1);
            // TODO - Decrease Sanity based on lighting conditions: Day / Nigth with light source / Night with no ligth source ...
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            return new List<Pair<string, string>>(1)
            {
                new Pair<string, string>("Action(WALKTO, -, -, -, -)", preWorldState.GetInventoryGUID(this.Target).ToString())
            };
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
