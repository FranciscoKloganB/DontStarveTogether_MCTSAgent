using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;
using MCTS.DST.Resources.NPCs;

namespace MCTS.DST.Actions
{

    public class Fight : ActionDST
    {
        private static readonly float duration = 0.66f;
        private static readonly string actionName = "Fight_";
        private readonly string target;

        private Dictionary<string, NPC> npcBase { get; } = NPCDict.Instance.npcBase;

        public Fight(string target) : base(actionName + target)
        {
            this.target = target;        }

        public override void ApplyActionEffects(WorldModelDST worldModel)
        {
            if (npcBase.ContainsKey(target)) {
                NPC targetNPC = npcBase[target];
                worldModel.Cycle += duration;
                worldModel.UpdateSatiation(-1.33f);
                worldModel.UpdateHP(- (targetNPC.Damage * targetNPC.AttackPeriod / 4));
                worldModel.UpdateSanity(- (duration * (targetNPC.InsanityAura / 60)));
            }
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            string targetGUID = preWorldState.GetEntitiesGUID(this.target).ToString();
            return new List<Pair<string, string>>()
            {
                new Pair<string, string>("Action(WALKTO, -, -, -, -)", targetGUID),
                new Pair<string, string>("Action(LOOKAT, -, -, -, -)", targetGUID),
                new Pair<string, string>("Action(ATTACK, -, -, -, -)", targetGUID)
            };
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
