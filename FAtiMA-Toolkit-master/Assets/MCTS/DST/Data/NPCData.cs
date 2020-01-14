using System;
using System.Collections.Generic;

namespace MCTS.DST.WorldModels
{

    public class NPCData
    {
        public string NPCName { get; private set; }
        public int PosX { get; private set; }
        public int PosZ { get; private set; }

        public NPCData(string name, int posX, int posZ)
        {
            this.NPCName = name;
            this.PosX = posX;
            this.PosZ = posZ;
        }

        public NPCData Clone()
        {
            return new NPCData(string.Copy(this.NPCName), this.PosX, this.PosZ);
        }
    } 
}
