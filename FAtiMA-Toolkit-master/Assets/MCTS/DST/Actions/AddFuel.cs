using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;
using System.Linq;
using MCTS.DST.Resources.Materials;

namespace MCTS.DST.Actions
{

    public class AddFuel : ActionDST
    {
        private static readonly float duration = 0.33f;
        private static readonly string actionName = "AddFuel_";
        private readonly string target;
        private string fuel;

        public AddFuel(string target) : base(actionName + target)
        {
            this.target = target;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            if (worldState.Fuel.Count == 0)
            {
                return; // TODO?
            }

            this.fuel = worldState.Fuel.ElementAt(0).Key;
            worldState.Cycle += duration;
            worldState.RemoveFromPossessedItems(this.fuel, 1);
            worldState.RemoveFromFuel(this.fuel);
            worldState.Walter.Position = worldState.GetNextPosition(this.target, "fire");

            if (MaterialDict.Instance.materialBase.ContainsKey(this.fuel))
            {
                WorldResource resource = MaterialDict.Instance.materialBase[this.fuel];
                resource.TryRemoveAction(worldState, Construct.actionName);
            }
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            string fuel = preWorldState.GetInventoryGUID(this.fuel).ToString();
            string fire = preWorldState.GetInventoryGUID(this.target).ToString();
            
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
