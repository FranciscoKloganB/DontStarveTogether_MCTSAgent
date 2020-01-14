using System.Linq;
using MCTS.DST.WorldModels;
using System.Collections.Generic;
using MCTS.DST.Actions;
using MCTS.DST.Resources.Materials;

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
            ["pig"] = Pig.Instance,
            ["tentacle"] = Tentacle.Instance,
            ["hound"] = Hound.Instance,
            ["clockwork_bishop"] = ClockworkBishop.Instance,
            ["clockwork_knight"] = ClockworkKnight.Instance,
        };
    }

    public class NPC
    {
        protected string NPCName { get; private set; }
        
        public HashSet<string> PossibleInteractions { get; protected set; }

        public NPC(string name)
        {
            this.NPCName = name;
        }

        public virtual object GetInteraction(string target)
        {
            return null;
        }
    }

    public class Spider : NPC
    {
        private Spider(string name) : base(name)
        {
            base.PossibleInteractions = new HashSet<string>() { "fight" };
        }

        public static NPC Instance { get; } = new Spider("spider");
    }

    public class SpiderWarrior : NPC
    {
        private SpiderWarrior(string name) : base(name)
        {
            base.PossibleInteractions = new HashSet<string>() { "fight" };
        }

        public static NPC Instance { get; } = new SpiderWarrior("spider_warrior");
    }

    public class Tentacle : NPC
    {
        private Tentacle(string name) : base(name)
        {
            base.PossibleInteractions = new HashSet<string>() { "avoid" };
        }

        public static NPC Instance { get; } = new Tentacle("tentacle");
    }

    public class Hound : NPC
    {
        private Hound(string name) : base(name)
        {
            base.PossibleInteractions = new HashSet<string>() { "fight" };
        }

        public static NPC Instance { get; } = new Hound("hound");
    }

    public class ClockworkKnight : NPC
    {
        private ClockworkKnight(string name) : base(name)
        {
            base.PossibleInteractions = new HashSet<string>() { "avoid" };
        }

        public static NPC Instance { get; } = new ClockworkKnight("clockwork_knight");
    }

    public class ClockworkBishop : NPC
    {
        private ClockworkBishop(string name) : base(name)
        {
            base.PossibleInteractions = new HashSet<string>() { "avoid" };
        }

        public static NPC Instance { get; } = new ClockworkBishop("clockwork_bishop");
    }

    public class Pig : NPC
    {
        private Pig(string name) : base(name)
        {
            base.PossibleInteractions = new HashSet<string>() { "avoid", "fight", "feed"};
        }

        public static NPC Instance { get; } = new Pig("pig");
    }
}
