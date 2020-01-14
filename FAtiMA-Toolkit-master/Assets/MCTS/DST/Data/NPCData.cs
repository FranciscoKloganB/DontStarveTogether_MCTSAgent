using System;
using System.Collections.Generic;

namespace MCTS.DST.WorldModels
{

    public class NPCData
    {
        public string NPCName { get; private set; }
        public string GUID { get; private set; }

        public NPCData(string name, string guid)
        {
            this.NPCName = name;
            this.GUID = guid;
        }

        public NPCData Clone()
        {
            return new NPCData(string.Copy(this.NPCName), string.Copy(this.GUID));
        }
    } 
}
