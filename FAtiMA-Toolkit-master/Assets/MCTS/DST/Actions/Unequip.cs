using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;


namespace MCTS.DST.Actions
{

    public class Unequip : ActionDST
    {
        private static readonly float duration = 0.0f;
        private static readonly string actionName = "Unequip_";
        private readonly string target;

        public Unequip(string target) : base(actionName + target)
        {
            this.target = target;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            worldState.RemoveAction(actionName + this.target);
            worldState.RemoveFromEquipped(this.target);
            worldState.AddToPossessedItems(this.target, 1);
            ActionDST equipAction = new Equip(this.target);
            worldState.AddAction(equipAction);
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            return new List<Pair<string, string>>(1)
            {
                new Pair<string, string>("Action(UNEQUIP, " + preWorldState.GetEquippedGUID(this.target).ToString() + ", -, -, -)", "-")
            };
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
