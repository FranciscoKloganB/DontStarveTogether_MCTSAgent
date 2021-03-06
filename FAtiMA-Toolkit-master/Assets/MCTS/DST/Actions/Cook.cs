﻿using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;


namespace MCTS.DST.Actions
{

    public class Cook : ActionDST
    {
        private static readonly float duration = 1.00f;
        private static readonly string actionName = "Cook_";
        private readonly string target;

        public Cook(string name) : base(name)
        {
            // TODO
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            base.ApplyActionEffects(worldState);
            // TODO - Remove Construct actions.
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            return base.Decompose(preWorldState);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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
