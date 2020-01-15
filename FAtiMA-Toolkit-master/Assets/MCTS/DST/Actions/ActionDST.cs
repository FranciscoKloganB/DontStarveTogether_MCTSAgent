using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;
using System.Collections;

namespace MCTS.DST.Actions
{

    public class ActionDST : IEqualityComparer
    {
        public string Name { get; private set; }
        
        public ActionDST(string name)
        {
            this.Name = name;
        }

        public virtual List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            List<Pair<string, string>> list = new List<Pair<string, string>>(1);

            //Initialized it with a random action, there are better ways of doing this
            Pair<string, string> pair = new Pair<string, string>("Action(Wander, -, -, -, -)", "-");
            list.Add(pair);

            return list;
        }

        public virtual void ApplyActionEffects(WorldModelDST worldState)
        {
        }

        public virtual Pair<string, int> NextActionInfo()
        {
            return new Pair<string, int>("", 0);
        }

        public ActionDST Clone()
        {
            return new ActionDST(string.Copy(this.Name));
        }

        public override bool Equals(object obj)
        {
            var item = obj as ActionDST;

            if (item is null)
            {
                return false;
            }

            return this.Name.Equals(item.Name);
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(this.Name);
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            var one = x as ActionDST;
            var another = y as ActionDST;

            if (x is null || y is null)
            {
                return false;
            }

            return one.Name.Equals(another.Name);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            var _ = obj as ActionDST;

            if (_ is null)
            {
                return 0;
            }

            return 539060726 + EqualityComparer<string>.Default.GetHashCode(_.Name);
        }
    }
}
