using System;
using System.Collections.Generic;
using Utilities;

namespace MCTS.DST.WorldModels
{

    public class WorldObjectData
    {
        public string ObjectName { get; private set; }
        public int Quantity { get; set; }
        public int PosX { get; private set; }
        public int PosZ { get; private set; }


        public WorldObjectData(string name, int quantity, int posX, int posZ)
        {
            this.ObjectName = name;
            this.Quantity = quantity;
            this.PosX = posX;
            this.PosZ = posZ;
        }

        public WorldObjectData Clone()
        {
            return new WorldObjectData(string.Copy(this.ObjectName), this.Quantity, this.PosX, this.PosZ);
        }
    } 
}
