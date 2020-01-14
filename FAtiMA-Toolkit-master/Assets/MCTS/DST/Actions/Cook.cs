using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;


namespace MCTS.DST.Actions
{

    public class Cook : ActionDST
    {
        private static readonly string actionName = "Cook_";
        private static readonly float Duration = 0.66f;

        private readonly string Target;

        private string Fire;

        public Cook(string name) : base(name)
        {
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
