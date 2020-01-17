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
        private static readonly string ActionName = "Unequip_";
        private readonly string Target;

        public Unequip(string target) : base(ActionName + target)
        {
            this.Target = target;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            worldState.Cycle += duration;
            worldState.RemoveFromEquipped(this.Target);
            worldState.AddToPossessedItems(this.Target, 1);
            ActionDST equipAction = new Equip(this.Target);
            worldState.AddAction(equipAction);
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            return new List<Pair<string, string>>(1)
            {
                new Pair<string, string>("Action(UNEQUIP, -, -, -, -)", preWorldState.GetInventoryGUID(this.Target).ToString())
            };
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
