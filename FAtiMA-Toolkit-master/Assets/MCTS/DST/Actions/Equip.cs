using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;


namespace MCTS.DST.Actions
{

    public class Equip : ActionDST
    {
        private static readonly string actionName = "Equip_";
        private static float Duration = 0.0f;

        private string Target;

        public Equip(string target) : base(actionName + target)
        {
            this.Target = target;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            worldState.Cycle += Duration;
            worldState.RemoveFromPossessedItems(this.Target, 1);
            worldState.AddToEquipped(this.Target);
            ActionDST unequipAction = new Unequip(this.Target);
            worldState.AddAction(unequipAction);
            if (!worldState.Possesses(this.Target))
            {
                worldState.RemoveAction(actionName + this.Target);
            }
            ActionDST dropAction = new Drop(this.Target);
            worldState.AddAction(dropAction);
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
