using System;
using System.Collections.Generic;

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

        public override bool Equals(object obj)
        {
            var item = obj as WorldObjectData;

            if (item == null)
            {
                return false;
            }

            return this.ObjectName.Equals(item.ObjectName);
        }

        public override int GetHashCode()
        {
            return this.ObjectName.GetHashCode();
        }
    } 
}
