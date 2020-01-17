﻿using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;


namespace MCTS.DST.Actions
{

    public class HoldPosition : ActionDST
    {        
        private static readonly float duration = 0.33f;
        private static readonly string ActionName = "HoldPosition_";
        private readonly string Target;

        public HoldPosition(string target) : base(ActionName + target)
        {
            this.Target = target;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            worldState.Cycle += duration;
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
