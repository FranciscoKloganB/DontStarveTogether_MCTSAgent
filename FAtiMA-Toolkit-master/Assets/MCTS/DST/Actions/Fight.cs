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
        public string Target;
        public float Duration;
        private static readonly string actionName = "Face_&_Attack_";

        public Fight(string target) : base(actionName + target)
        {
            this.Target = target;
            this.Duration = 1.0f;
        }

        public override void ApplyActionEffects(WorldModelDST worldModel)
        {
            worldModel.Cycle += this.Duration;
            worldModel.UpdateSatiation(-3.0f);
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            string targetGUID = preWorldState.GetEntitiesGUID(this.Target).ToString();
            return new List<Pair<string, string>>(3)
            {
                new Pair<string, string>("Action(WALKTO, -, -, -, -)", targetGUID),
                new Pair<string, string>("Action(LOOKAT, -, -, -, -)", targetGUID),
                new Pair<string, string>("Action(ATTACK, -, -, -, -)", targetGUID)
            };
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
