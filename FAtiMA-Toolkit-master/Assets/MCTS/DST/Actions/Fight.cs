using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;


namespace MCTS.DST.Actions
{

    public class Fight : ActionDST
    {
        private static readonly float duration = 0.33f;
        private static readonly string actionName = "Fight_";
        private readonly string target;

        public Fight(string name) : base(name)
        {
            // TODO
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            base.ApplyActionEffects(worldState);
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
