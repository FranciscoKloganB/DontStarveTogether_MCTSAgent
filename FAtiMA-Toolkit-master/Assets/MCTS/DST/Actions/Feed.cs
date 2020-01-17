using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST.Resources.Buildables;
using MCTS.DST;


namespace MCTS.DST.Actions
{

    public class Feed : ActionDST
    {
        public static readonly string actionName = "Feed_";

        private static readonly float duration = 0.05f;
        
        private readonly HashSet<string> diet;
        private readonly string target;

        public Feed(HashSet<string> diet, string target) : base(string.Concat(actionName, target))
        {
            this.diet = diet;
            this.target = target;
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            // TODO: Apply action effects
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            // TODO: Replace this invobjectGUID "this.Target" with string<name> of the food we want to feed to target
            string targetGUID = preWorldState.GetEntitiesGUID(this.target).ToString();
            string invobjectGUID = preWorldState.GetEntitiesGUID(this.target).ToString(); 
            return new List<Pair<string, string>>(1)
            {
                new Pair<string, string>("Action(FEED, " + invobjectGUID + ", -, -, " + targetGUID +")", "-")
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
