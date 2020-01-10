using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;


namespace MCTS.DST.Actions
{

    public class AddFuel : ActionDST
    {
        private string Fuel;
        private string Target;
        private float Duration;
        private static readonly string ActionName = "AddFuel_";

        public AddFuel(string target) : base(ActionName + target)
        {
            this.Target = target;
            this.Duration = 0.33f;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            this.Fuel = worldState.Fuel[0].Item1;
            worldState.Cycle += this.Duration;
            worldState.RemoveFromPossessedItems(this.Fuel, 1);
            worldState.RemoveFromFuel(this.Fuel);
            worldState.Walter.Position = worldState.GetNextPosition(this.Target, "fire");
            // TODO - Removed Construct actions that are now impossible due to loss of material.
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            string fuel = preWorldState.GetInventoryGUID(this.Fuel).ToString();
            string fire = preWorldState.GetInventoryGUID(this.Target).ToString();
            
            return new List<Pair<string, string>>(1)
            {
                new Pair<string, string>("Action(ADDFUEL, " + fuel + ", -, -, -)", fire)
            };
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
