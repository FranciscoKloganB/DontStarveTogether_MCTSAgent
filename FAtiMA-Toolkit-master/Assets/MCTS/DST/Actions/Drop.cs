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
        public static readonly string actionName = "Drop_";
        private static readonly float Duration = 0.33f;

        public readonly string Target;

        public Pair<int, int> Position;

        public Drop(string target) : base(actionName + target)
        {
            this.Target = target;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            worldState.Cycle += Duration;
            worldState.RemoveFromEquipped(this.Target);
            this.Position = new Pair<int, int>(worldState.Walter.GetPosX(), worldState.Walter.GetPosZ());
            // TODO - How do we get the quantity of items to drop?
            worldState.AddToWorld(this.Target, 1, this.Position.Item1, this.Position.Item2);
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            return new List<Pair<string, string>>(1)
            {
                new Pair<string, string>("Action(DROP, " + preWorldState.GetEquippedGUID(this.Target).ToString() + ", " + this.Position.Item1 + ", " + this.Position.Item2 + ", -)", "-")
            };
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
