using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST.Resources.Buildables;
using MCTS.DST;
using MCTS.DST.Resources.Edibles;

namespace MCTS.DST.Actions
{

    public class Feed : ActionDST
    {
        private static readonly float duration = 0.2f;
        private static readonly string actionName = "Feed_";
        private readonly string target;
        public readonly HashSet<string> diet;
        public string food { get; set; }
        private Dictionary<string, Food> foodBase { get; } = FoodDict.Instance.foodBase;

        public Feed(HashSet<string> diet, string target) : base(string.Concat(actionName, target))
        {
            this.diet = diet;
            this.target = target;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            if (this.foodBase.ContainsKey(this.food))
            {
                Food targetFood = this.foodBase[this.food];
                worldState.RemoveFromPossessedItems(targetFood.FoodName, 1);
                targetFood.TryRemoveAction(worldState, actionName);
                worldState.Cycle += duration;
            }
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            string targetGUID = preWorldState.GetEntitiesGUID(this.target).ToString();
            // TODO - Better way of setting food.
            string invobjectGUID = preWorldState.GetInventoryGUID(this.food).ToString(); 
            return new List<Pair<string, string>>(1)
            {
                new Pair<string, string>("Action(FEED, " + invobjectGUID + ", -, -, -)", targetGUID)
            };
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
