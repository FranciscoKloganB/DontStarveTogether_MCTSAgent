using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;


namespace MCTS.DST.Actions
{

    public class Unequip : ActionDST
    {
        private string Target;
        private float Duration;
        private static readonly string ActionName = "Unequip_";

        public Unequip(string target) : base(ActionName + target)
        {
            this.Target = target;
            this.Duration = 0.0f;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            worldState.Cycle += this.Duration;
            worldState.RemoveFromEquipped(this.Target);
            worldState.AddToPossessedItems(this.Target, 1);
            ActionDST equipAction = new Equip(this.Target);
            worldState.AddAction(equipAction);
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            return new List<Pair<string, string>>(1)
            {
                new Pair<string, string>("Action(Unequip, " + preWorldState.GetInventoryGUID(this.Target).ToString() + ", -, -, -)", "-")
            };
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
