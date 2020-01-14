using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;
using System.Linq;

namespace MCTS.DST.Actions
{

    public class AddFuel : ActionDST
    {
        private static readonly string actionName = "AddFuel_";
        private static readonly float Duration = 0.33f;

        private readonly string Target;

        private string Fuel;

        public AddFuel(string target) : base(actionName + target)
        {
            this.Target = target;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            if (worldState.Fuel.Count == 0)
            {
                return; // TODO?
            }

            this.Fuel = worldState.Fuel.ElementAt(0).Key;
            worldState.Cycle += this.Duration;
            worldState.RemoveFromPossessedItems(this.Fuel, 1);
            worldState.RemoveFromFuel(this.Fuel);
            worldState.Walter.Position = worldState.GetNextPosition(this.Target, "fire");
            // TODO - Remove Construct actions that are now impossible due to loss of material.
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
