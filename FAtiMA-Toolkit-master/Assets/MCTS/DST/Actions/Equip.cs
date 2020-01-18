using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;


namespace MCTS.DST.Actions
{

    public class Equip : ActionDST
    {
        private static readonly float duration = 0.0f;
        private static readonly string actionName = "Equip_";
        private readonly string target;

        public Equip(string target) : base(actionName + target)
        {
            this.target = target;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            worldState.Cycle += duration;
            // TODO - It works like this, but it would also make sense without this.
            worldState.RemoveFromPossessedItems(this.target, 1);
            worldState.AddToEquipped(this.target);
            ActionDST unequipAction = new Unequip(this.target);
            worldState.AddAction(unequipAction);
            if (!worldState.Possesses(this.target))
            {
                worldState.RemoveAction(actionName + this.target);
            }
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            return new List<Pair<string, string>>(1)
            {
                new Pair<string, string>("Action(EQUIP, " + preWorldState.GetInventoryGUID(this.target).ToString() + ", -, -, -)", "-")
            };
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
