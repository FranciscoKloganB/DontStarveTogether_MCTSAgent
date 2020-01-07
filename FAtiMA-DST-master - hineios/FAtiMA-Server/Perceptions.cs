﻿using Newtonsoft.Json;
using RolePlayCharacter;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using WellFormedNames;

namespace FAtiMA_Server
{
    public class Perceptions
    {
        //string GUID { get; set; }
        List<Entity> Vision { get; set; }
        List<Entity> ItemSlots { get; set; }
        List<Entity> EquipSlots { get; set; }
        public int Hunger { get; set; }
        public int Sanity { get; set; }
        public int Health { get; set; }
        public bool IsBusy { get; set; }
        public int PosX { get; set; }
        public int PosZ { get; set; }
        public readonly int PosY { get; private set; } = 0;

        [JsonConstructor]
        public Perceptions(List<Entity> equipslots, List<Entity> vision, List<Entity> itemslots, float hunger, float sanity, float health, bool isbusy, float posx, float posy, float posz)
        {
            Vision = vision;
            ItemSlots = itemslots;
            EquipSlots = equipslots;
            Hunger = (int) hunger;
            Health = (int) health;
            Sanity = (int) sanity;
            IsBusy = isbusy;
            PosX = (int) posx;
            PosZ = (int) posz;
        }
        
        public void UpdatePerceptions(RolePlayCharacterAsset rpc)
        {
            // Find every InSight, InInventory, and IsEquipped belief and set them to false
            CleanBeliefs(rpc);

            // Update the KB with the new beliefs
            string bv = rpc.GetBeliefValue("Hunger(" + rpc.CharacterName.ToString() + ")");
            if (bv == null || !bv.Equals(Hunger.ToString()))
            {
                Debug.WriteLine("Hunger: " + bv + " -> " + Hunger.ToString());
                rpc.Perceive(EventHelper.PropertyChange("Hunger(" + rpc.CharacterName.ToString() + ")", Hunger.ToString(), rpc.CharacterName.ToString()));
            }

            bv = rpc.GetBeliefValue("Health(" + rpc.CharacterName.ToString() + ")");
            if (bv == null || !bv.Equals(Health.ToString()))
            {
                Debug.WriteLine("Health: " + bv + " -> " + Health.ToString());
                rpc.Perceive(EventHelper.PropertyChange("Health(" + rpc.CharacterName.ToString() + ")", Health.ToString(), rpc.CharacterName.ToString()));
            }

            bv = rpc.GetBeliefValue("Sanity(" + rpc.CharacterName.ToString() + ")");
            if (bv == null || !bv.Equals(Sanity.ToString()))
            {
                Debug.WriteLine("Sanity: " + bv + " -> " + Sanity.ToString());
                rpc.Perceive(EventHelper.PropertyChange("Sanity(" + rpc.CharacterName.ToString() + ")", Sanity.ToString(), rpc.CharacterName.ToString()));
            }

            bv = rpc.GetBeliefValue("IsBusy(" + rpc.CharacterName.ToString() + ")");
            if (bv == null || !bv.Equals(IsBusy.ToString()))
            {
                Debug.WriteLine("IsBusy: " + bv + " -> " + IsBusy.ToString());
                rpc.Perceive(EventHelper.PropertyChange("IsBusy(" + rpc.CharacterName.ToString() + ")", IsBusy.ToString(), rpc.CharacterName.ToString()));
            }

            bv = rpc.GetBeliefValue("PosX(" + rpc.CharacterName.ToString() + ")");
            if (bv == null || !bv.Equals(PosX.ToString()))
                rpc.Perceive(EventHelper.PropertyChange("PosX(" + rpc.CharacterName.ToString() + ")", PosX.ToString(), rpc.CharacterName.ToString()));


            bv = rpc.GetBeliefValue("PosZ(" + rpc.CharacterName.ToString() + ")");
            if (bv == null || !bv.Equals(PosZ.ToString()))
                rpc.Perceive(EventHelper.PropertyChange("PosZ(" + rpc.CharacterName.ToString() + ")", PosZ.ToString(), rpc.CharacterName.ToString()));


            foreach (Entity i in Vision)
            {
                if(i != null)
                {
                    bv = rpc.GetBeliefValue("InSight(" + i.GUID + ")");
                    if (bv == null || !bv.Equals("True"))
                        rpc.Perceive(EventHelper.PropertyChange("InSight(" + i.GUID + ")", "True", rpc.CharacterName.ToString()));
                    i.UpdatePerception(rpc);
                }
            }

            foreach (Entity i in ItemSlots)
            {
                if (i != null)
                {
                    bv = rpc.GetBeliefValue("InInventory(" + i.GUID + ")");
                    if (bv == null || !bv.Equals("True"))
                        rpc.Perceive(EventHelper.PropertyChange("InInventory(" + i.GUID + ")", "True", rpc.CharacterName.ToString()));
                    i.UpdatePerception(rpc);
                }
            }

            foreach (Entity i in EquipSlots)
            {
                if (i != null)
                {
                    bv = rpc.GetBeliefValue("IsEquipped(" + i.GUID + ")");
                    if (bv == null || !bv.Equals("True"))
                        rpc.Perceive(EventHelper.PropertyChange("IsEquipped(" + i.GUID + ")", "True", rpc.CharacterName.ToString()));
                    i.UpdatePerception(rpc);
                }
            }

            rpc.Update();
        }

        private void CleanBeliefs(RolePlayCharacterAsset rpc)
        {
            // Find every InSight, InInventory, and IsEquipped belief and delete them

            var subset = new List<SubstitutionSet> { new SubstitutionSet() };

            var beliefs = rpc.m_kb.AskPossibleProperties((Name)"InSight([x])", (Name)"SELF", subset);
            foreach (var b in beliefs)
            {
                foreach (var s in b.Item2)
                {
                    rpc.RemoveBelief("InSight(" + s[(Name)"[x]"] + ")", "SELF");
                }
            }

            beliefs = rpc.m_kb.AskPossibleProperties((Name)"InInventory([x])", (Name)"SELF", subset);
            foreach (var b in beliefs)
            {
                foreach (var s in b.Item2)
                {
                    rpc.RemoveBelief("InInventory(" + s[(Name)"[x]"] + ")", "SELF");
                }
            }

            beliefs = rpc.m_kb.AskPossibleProperties((Name)"IsEquipped([x])", (Name)"SELF", subset);
            foreach (var b in beliefs)
            {
                foreach (var s in b.Item2)
                {
                    rpc.RemoveBelief("IsEquipped(" + s[(Name)"[x]"] + ")", "SELF");
                }
            }
        }

        /**
         * Just a quick way to show the Perceptions that we just got from the 'body'
         **/
        public override string ToString()
        {
            string s = "Perceptions:\n";
            s += "\tHunger: " + Hunger;
            s += "\tSanity: " + Sanity;
            s += "\tHealth: " + Health;
            s += "\tIsBusy: " + IsBusy;
            s += "\tPos: (" + PosX + ", " + PosY + ", " + PosZ + ")";
            s += "\n\tVision:\n";
            foreach (Entity v in Vision)
            {
                if (v != null) s += "\t\t" + v.ToString() + "\n";
            }
            s += "\tItemSlots:\n";
            foreach (Entity i in ItemSlots)
            {
                if (i != null) s += "\t\t" + i.ToString() + "\n";
            }
            s += "\tEquipSlots:\n";
            foreach (Entity e in EquipSlots)
            {
                if (e != null) s += "\t\t" + e.ToString() + "\n";
            }
            return s;
        }
    }
}

