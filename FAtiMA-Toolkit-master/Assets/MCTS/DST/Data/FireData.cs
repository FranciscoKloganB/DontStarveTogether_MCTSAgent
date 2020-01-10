using System;
using System.Collections.Generic;
using Utilities;

namespace MCTS.DST.WorldModels
{

    public class FireData : ICloneable
    {
        public string FireName { get; private set; }
        public int PosX { get; private set; }
        public int PosZ { get; private set; }

        public FireData(string name, int posX, int posZ)
        {
            this.FireName = name;
            this.PosX = posX;
            this.PosZ = posZ;
        }

        public FireData Clone()
        {
            return new FireData(string.Copy(this.FireName), this.PosX, this.PosZ);
        }
    } 
}
