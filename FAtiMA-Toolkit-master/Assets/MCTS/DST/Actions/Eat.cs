using System.Collections.Generic;
using MCTS.DST.Resources.Edibles;
using MCTS.DST.WorldModels;
using Utilities;

namespace MCTS.DST.Actions
{

    public class Eat : ActionDST
    {
        private static readonly float duration = 0.05f;
        private static readonly string actionName = "Eat_";
        private readonly string target;
        private Dictionary<string, Food> foodBase { get; } = FoodDict.Instance.foodBase;

        public Eat(string target) : base(string.Concat(actionName, target))
        {
            this.target = target;
        }

        public override void ApplyActionEffects(WorldModelDST worldModel)
        {
            try
            {
                Food targetFood = foodBase[this.target];
                targetFood.Eat(worldModel);
                targetFood.TryRemoveAction(worldModel, actionName);
                worldModel.Cycle += duration;
            }
            catch (KeyNotFoundException)
            {
                worldModel.RemoveAction(string.Concat(actionName, this.target));
            }
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            int guid = preWorldState.GetInventoryGUID(this.target);

            List<Pair<string, string>> ListOfActions = new List<Pair<string, string>>(1);
            Pair<string, string> pair;

            pair = new Pair<string, string>("Action(EAT, -, -, -, -)", guid.ToString());

            ListOfActions.Add(pair);

            return ListOfActions;
        }

        public override Pair<string, int> NextActionInfo()
        {
            return new Pair<string, int>("", 0);
        }
    }
}
