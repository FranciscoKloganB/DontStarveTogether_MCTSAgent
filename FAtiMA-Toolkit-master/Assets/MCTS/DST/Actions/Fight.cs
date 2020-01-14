using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;
using MCTS.DST.Resources.Materials;


namespace MCTS.DST.Actions
{

    public class Fight : ActionDST
    {
        private readonly string Target;
        private readonly string TargetGUID;
        private readonly float Duration;
        private static readonly string actionName = "Face_&_Attack_";

        public Fight(string target, string guid) : base(actionName + target)
        {
            this.Target = target;
            this.TargetGUID = guid;
            this.Duration = 1.0f;
        }

        public override void ApplyActionEffects(WorldModelDST worldModel)
        {
            worldModel.Cycle += this.Duration;
            worldModel.UpdateSatiation(-3.0f);
            worldModel.UpdateHP(-30.0f); // TODO Make this better.
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            // string targetGUID = preWorldState.GetEntitiesGUID(this.Target).ToString();
            return new List<Pair<string, string>>()
            {
                //new Pair<string, string>("Action(WALKTO, -, -, -, -)", TargetGUID),
                //new Pair<string, string>("Action(LOOKAT, -, -, -, -)", TargetGUID),
                new Pair<string, string>("Action(ATTACK, -, -, -, -)", this.TargetGUID)
            };
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
