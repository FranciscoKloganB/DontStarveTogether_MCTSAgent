using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;
using MCTS.DST.Resources.NPCs;
using MCTS.DST.Resources.Materials;
using MCTS.DST.Resources.Buildables;

namespace MCTS.DST.Actions
{

    public class Fight : ActionDST
    {
        private static readonly float duration = 0.66f;
        private static readonly string actionName = "Fight_";
        private readonly string target;

        private Dictionary<string, Buildable> buildableBase { get; } = BuildablesDict.Instance.buildableBase;
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
            for (int i = 0; i < preWorldState.Equipped.Count; i++)
            {
                string equippedName = preWorldState.Equipped[i].Item1;
                if (buildableBase.ContainsKey(equippedName))
                {
                    return new List<Pair<string, string>>(1)
                    {
                        new Pair<string, string>("Action(ATTACK, -, -, -, -)", targetGUID)
                    };
                }
            }
            // Equips weapon and attacks.
            for (int i = 0; i < preWorldState.Inventory.Count; i++)
            {
                string possessedName = preWorldState.Equipped[i].Item1;
                if (buildableBase.ContainsKey(possessedName))
                {
                    return new List<Pair<string, string>>(2)
                    {
                        new Pair<string, string>("Action(EQUIP, " + preWorldState.GetInventoryGUID(possessedName).ToString() + ", -, -, -)", "-"),
                        new Pair<string, string>("Action(ATTACK, -, -, -, -)", targetGUID)
                    };
                }
            }
            Console.WriteLine("\n PickUp Decompose had to fall back to base.Decompose \n");
            return base.Decompose(preWorldState);
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
