using System.Linq;
using MCTS.DST.WorldModels;
using System.Collections.Generic;
using MCTS.DST.Actions;
using MCTS.DST.Resources.Materials;
using MCTS.DST.Interfaces;

namespace MCTS.DST.Resources.NPCs
{
    public sealed class NPCDict
    {
        public static NPCDict Instance { get; } = new NPCDict();

        private NPCDict()
        {
        }

        public Dictionary<string, NPC> npcBase = new Dictionary<string, NPC>()
        {
            ["spider"] = Spider.Instance,
            ["spider_warrior"] = SpiderWarrior.Instance,
            ["pigman"] = Pig.Instance,
            ["hound"] = Hound.Instance,
            ["butterfly"] = Butterfly.Instance,
        };
    }

    public class NPC
    {
        public string NPCName { get; private set; }
        public int HP { get; protected set; }
        public int Damage { get; protected set; }
        public int AttackPeriod { get; protected set; }
        public int AttackRange { get; protected set; }
        public int RunningSpeed { get; protected set; }
        public int InsanityAura { get; protected set; }

        public NPC(string name)
        {
            this.NPCName = name;
        }

        public virtual ActionDST GetAction(WorldModelDST worldModel)
        {
            return new Fight(this.NPCName);
        }
    }

    public class Enemy : NPC
    {
        public Enemy(string name) : base(name) { }
    }

    public class Neutral : NPC
    {
        public Neutral(string name) : base(name) { }
    }

    public class Friendly : NPC
    {
        public Friendly(string name) : base(name) { }

        public override ActionDST GetAction(WorldModelDST worldModel)
        {
            return null;
        }
    }

    public sealed class Spider : Enemy
    {
        private Spider(string name) : base(name)
        {
            base.HP = 100;
            base.Damage = 20;
            base.AttackPeriod = 3;
            base.AttackRange = 3;
            base.RunningSpeed = 5;
            base.InsanityAura = -25;
        }

        public static NPC Instance { get; } = new Spider("spider");
    }

    public sealed class SpiderWarrior : Enemy
    {
        private SpiderWarrior(string name) : base(name)
        {
            base.HP = 200;
            base.Damage = 20;
            base.AttackPeriod = 5;
            base.AttackRange = 6;
            base.RunningSpeed = 5;
            base.InsanityAura = -40;
        }

        public static NPC Instance { get; } = new SpiderWarrior("spider_warrior");
    }

    public sealed class Hound : Enemy
    {
        private Hound(string name) : base(name)
        {
            base.HP = 150;
            base.Damage = 20;
            base.AttackPeriod = 2;
            base.AttackRange = 3;
            base.RunningSpeed = 10;
            base.InsanityAura = -40;
        }

        public static NPC Instance { get; } = new Hound("hound");
    }

    public sealed class Pig : Neutral, IDiet
    {
        private Pig(string name) : base(name)
        {
            base.HP = 250;
            base.Damage = 33;
            base.AttackPeriod = 3;
            base.AttackRange = 3;
            base.RunningSpeed = 5;
            base.InsanityAura = +25;
        }

        public static NPC Instance { get; } = new Pig("pigman");
        
        public override ActionDST GetAction(WorldModelDST worldModel)
        {
            return new Feed(this.GetDiet(), base.NPCName);
        }

        public HashSet<string> GetDiet()
        {
            return new HashSet<string>() { "berries", "meat", "cooked_meat", "monster_meat", "cooked_monster_meat", "morsel", "cooked_morsel" };
        }
    }

    public sealed class Butterfly : Enemy
    {
        private Butterfly(string name) : base(name)
        {
            base.HP = 1;
            base.Damage = 0;
            base.AttackPeriod = 0;
            base.AttackRange = 0;
            base.RunningSpeed = 5;
            base.InsanityAura = +0;
        }

        public static NPC Instance { get; } = new Butterfly("butterfly");
    }
}