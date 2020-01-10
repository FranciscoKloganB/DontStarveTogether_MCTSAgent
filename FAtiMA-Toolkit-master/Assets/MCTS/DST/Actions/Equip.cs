using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;


namespace MCTS.DST.Actions
{

    public class Equip : ActionDST
    {
        private string Target;
        private float Duration;
        private static readonly string ActionName = "Equip_";

        public Equip(string target) : base(ActionName + target)
        {
            this.Target = target;
            this.Duration = 0.0f;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            worldState.Cycle += this.Duration;
            worldState.RemoveFromPossessedItems(this.Target, 1);
            worldState.AddToEquipped(this.Target);
            ActionDST unequipAction = new Unequip(this.Target);
            worldState.AddAction(unequipAction);
            if (!worldState.Possesses(this.Target))
            {
                worldState.RemoveAction(ActionName + this.Target);
            }
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            return new List<Pair<string, string>>(1)
            {
                new Pair<string, string>("Action(EQUIP, " + preWorldState.GetInventoryGUID(this.Target).ToString() + ", -, -, -)", "-")
            };
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
