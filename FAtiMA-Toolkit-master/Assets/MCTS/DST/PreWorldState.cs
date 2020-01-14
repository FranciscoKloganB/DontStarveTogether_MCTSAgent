using System;
using System.Collections.Generic;
using Utilities;
using KnowledgeBase;
using MCTS.DST.WorldModels;
using WellFormedNames;
using System.Linq;

namespace MCTS.DST {

    public class PreWorldState
    {
        private static HashSet<string> treeBase = new HashSet<string>() { 
            "evergreen", "mushtree_tall", "mushtree_medium",
            "mushtree_small", "mushtree_tall_webbed", "evergreen_sparse",
            "twiggy_short", "twiggy_normal", "twiggy_tall", 
            "twiggy_old", "deciduoustree", "twiggytree",
        };

        private static HashSet<string> rockBase = new HashSet<string>()
        {
            "rock1", "rock2", "rock_flintless",
            "rock_moon", "rock_petrified_tree_short", "rock_petrified_tree_med",
            "rock_petrified_tree_tall", "rock_petrified_tree_old",
        };

        private static HashSet<string> fireBase = new HashSet<string>()
        {
            "campfire", "firepit", "endothermic_fire",
        };

        private static HashSet<string> realEntityBase_wtf = new HashSet<string>() {
            "twigs", "sapling",
            "log", "torch", "grass",
            "cutgrass", "carrot", "carrot_planted",
            "rocks", "flint", "axe",
            "pickaxe", "campfire", "firepit"
        };

        public Character Walter;
        public float Cycle;
        public int[] CycleInfo;

        public List<ObjectProperties> Entities;
        public List<Tuple<string, int, int>> Inventory;
        public List<Pair<string, int>> Equipped;
        public List<Tuple<string, int, int>> Fuel;
        public List<Tuple<string, int, int>> Fire;
        public List<NPCData> NPC;
        public KB KnowledgeBase;

       
        public PreWorldState(KB knowledgeBase)
        {
            this.KnowledgeBase = knowledgeBase;
            this.Entities = new List<ObjectProperties>(); // it is a list which stores the information of the objects that exist in the world
            this.Inventory = new List<Tuple<string, int, int>>(); // list of 3-tuples that contain the name, the GUID and the quantity of an object in the inventory
            this.Equipped = new List<Pair<string, int>>(); // it is a list of pairs (2-tuples) that contain the name and the GUID of each equipped object
            this.Fuel = new List<Tuple<string, int, int>>(); // it is a list of 3-tuples that contain the name, the GUID and the quantity of the items that can be used as fuel for res
            this.Fire = new List<Tuple<string, int, int>>(); // list of 3-tuples that contain the name and the position of the fires in the world
            this.NPC = new List<NPCData>();

            //Getting Character Stats

            var hp = knowledgeBase.AskProperty((Name)"Health(Walter)");           
            int HP = int.Parse(hp.Value.ToString());

            var hunger = knowledgeBase.AskProperty((Name)"Hunger(Walter)");
            int Hunger = int.Parse(hunger.Value.ToString());

            var sanity = knowledgeBase.AskProperty((Name)"Sanity(Walter)");
            int Sanity = int.Parse(sanity.Value.ToString());

            var posx = knowledgeBase.AskProperty((Name)"PosX(Walter)");
            var PosX = int.Parse(posx.Value.ToString());

            var posz = knowledgeBase.AskProperty((Name)"PosZ(Walter)");
            var PosZ = int.Parse(posz.Value.ToString());

            this.Walter = new Character(HP, Hunger, Sanity, PosX, PosZ);

            //Getting Day Properties

            var cycle = knowledgeBase.AskProperty((Name)"World(CurrentSegment)");
            this.Cycle = float.Parse(cycle.Value.ToString());

            var cycleinfo1 = knowledgeBase.AskProperty((Name)"World(PhaseLenght, day)");
            var Cycleinfo1 = int.Parse(cycleinfo1.Value.ToString());

            var cycleinfo2 = knowledgeBase.AskProperty((Name)"World(PhaseLenght, dusk)");
            var Cycleinfo2 = int.Parse(cycleinfo2.Value.ToString());

            var cycleinfo3 = knowledgeBase.AskProperty((Name)"World(PhaseLenght, night)");
            var Cycleinfo3 = int.Parse(cycleinfo3.Value.ToString());

            this.CycleInfo = new int[3];
            this.CycleInfo[0] = Cycleinfo1;
            this.CycleInfo[1] = Cycleinfo2;
            this.CycleInfo[2] = Cycleinfo3;

            //Getting Entities + Inventory + Equipped

            var subset = new List<SubstitutionSet> { new SubstitutionSet() };

            //Getting Equipped

            var equippeditems = knowledgeBase.AskPossibleProperties((Name)"IsEquipped([GUID])", Name.SELF_SYMBOL, subset);

            foreach (var item in equippeditems)
            {
                string strEntGuid = item.Item2.FirstOrDefault().FirstOrDefault().SubValue.Value.ToString();
                int entGuid = int.Parse(strEntGuid);
                string entPrefab = knowledgeBase.AskProperty((Name)("Entity(" + strEntGuid + ")")).Value.ToString();

                Pair<string, int> pair = new Pair<string, int>(entPrefab, entGuid);
                this.Equipped.Add(pair);
            }

            //Getting Inventory

            var inventory = knowledgeBase.AskPossibleProperties((Name)"InInventory([GUID])", Name.SELF_SYMBOL, subset);

            foreach (var item in inventory)
            {
                string strEntGuid = item.Item2.FirstOrDefault().FirstOrDefault().SubValue.Value.ToString();
                int entGuid = int.Parse(strEntGuid);
                string entPrefab = knowledgeBase.AskProperty((Name)("Entity(" + strEntGuid + ")")).Value.ToString();

                string strEntQuantity = "Quantity(" + strEntGuid + ")";
                var quantity = knowledgeBase.AskProperty((Name)strEntQuantity);
                int entQuantity = int.Parse(quantity.Value.ToString());

                Tuple<string, int, int> tuple = new Tuple<string, int, int>(entPrefab, entGuid, entQuantity);
                this.Inventory.Add(tuple);

                if (IsFuel(strEntGuid))
                {                    
                    this.Fuel.Add(tuple);
                }                
            }

            //Getting Entities

            var entities = knowledgeBase.AskPossibleProperties((Name)"Entity([GUID])", Name.SELF_SYMBOL, subset);

            foreach (var entity in entities)
            {
                bool b = false;
                string strEntGuid = entity.Item2.FirstOrDefault().FirstOrDefault().SubValue.Value.ToString();
                int entGuid = int.Parse(strEntGuid);
                string entPrefab = entity.Item1.Value.ToString();
                string realEntPrefab = RealEntityPrefab(entPrefab);

                if (IsNPC(entPrefab))
                {
                    string posXStr = "PosX(" + strEntGuid + ")";
                    int posX = int.Parse(knowledgeBase.AskProperty((Name)posXStr).Value.ToString());
                    string posZStr = "PosZ(" + strEntGuid + ")";
                    int posZ = int.Parse(knowledgeBase.AskProperty((Name)posZStr).Value.ToString());
                    this.NPC.Add(new NPCData(entPrefab, posX, posZ));
                }

                if (IsFire(entPrefab))
                {
                    string strEntPosx = "PosX(" + strEntGuid + ")";
                    var POSx = knowledgeBase.AskProperty((Name)strEntPosx);
                    int entPosx = int.Parse(POSx.Value.ToString());

                    string strEntPosz = "PosZ(" + strEntGuid + ")";
                    var POSz = knowledgeBase.AskProperty((Name)strEntPosz);
                    int entPosz = int.Parse(POSz.Value.ToString());

                    Tuple<string, int, int> tuple = new Tuple<string, int, int>(entPrefab, entPosx, entPosz);
                    this.Fire.Add(tuple);
                }
                else if (realEntPrefab != "" && DistanceCalculator(strEntGuid) > 0)
                {
                    string strEntIsCollectable = "IsCollectable(" + strEntGuid + ")";
                    var isCollectable = knowledgeBase.AskProperty((Name)strEntIsCollectable);
                    bool entIsCollectable = bool.Parse(isCollectable.Value.ToString());

                    string strEntIsPickable = "IsPickable(" + strEntGuid + ")";
                    var isPickable = knowledgeBase.AskProperty((Name)strEntIsPickable);
                    bool entIsPickable = bool.Parse(isPickable.Value.ToString());

                    string strEntIsMineable = "IsMineable(" + strEntGuid + ")";
                    var isMineable = knowledgeBase.AskProperty((Name)strEntIsMineable);
                    bool entIsMineable = bool.Parse(isMineable.Value.ToString());

                    string strEntIsChoppable = "IsChoppable(" + strEntGuid + ")";
                    var isChoppable = knowledgeBase.AskProperty((Name)strEntIsChoppable);
                    bool entIsChoppable = bool.Parse(isChoppable.Value.ToString());

                    if (entIsPickable || entIsCollectable || entIsMineable || entIsChoppable)
                    {
                        string strEntQuantity = "Quantity(" + strEntGuid + ")";
                        var quantity = knowledgeBase.AskProperty((Name)strEntQuantity);
                        int entQuantity = int.Parse(quantity.Value.ToString());

                        string strEntPosx = "PosX(" + strEntGuid + ")";
                        var POSx = knowledgeBase.AskProperty((Name)strEntPosx);
                        int entPosx = int.Parse(POSx.Value.ToString());

                        string strEntPosz = "PosZ(" + strEntGuid + ")";
                        var POSz = knowledgeBase.AskProperty((Name)strEntPosz);
                        int entPosz = int.Parse(POSz.Value.ToString());

                        foreach (ObjectProperties objectproperty in this.Entities)
                        {
                            if (objectproperty.Prefab == realEntPrefab)
                            {
                                objectproperty.Add(entQuantity, entPrefab, entGuid, entPosx, entPosz, this.Walter);
                                b = true;
                                break;
                            }
                        }
                        if (!b)
                        {
                            ObjectProperties newObjectproperty = new ObjectProperties(realEntPrefab, entPrefab, entGuid, entQuantity, entPosx, entPosz, entIsCollectable, entIsPickable, entIsMineable, entIsChoppable);
                            this.Entities.Add(newObjectproperty);
                        }
                    }
                }
            }          
        }

        public bool IsTree(string tree)
        {
            return PreWorldState.treeBase.Contains(tree);
        }

        public bool IsBoulder(string boulder)
        {
            return PreWorldState.rockBase.Contains(boulder);
        }

        public bool IsFire(string prefab)
        {
            return PreWorldState.fireBase.Contains(prefab);
        }

        public string RealEntityPrefab(string entity)
        {
            if (PreWorldState.realEntityBase_wtf.Contains(entity))
            {
                return entity;
            }
            else if (PreWorldState.treeBase.Contains(entity))
            {
                return "tree";
            }
            else if (PreWorldState.rockBase.Contains(entity))
            {
                return "boulder";
            }
            else if (entity.Contains("berrybush"))
            {
                return "berrybush";
            }
            else if (entity.Contains("berries"))
            {
                return "berries";
            }
            else
            {
                return "";
            }
        }

        public bool IsFuel(string guid)
        {
            string strEntFuel = "IsFuel(" + guid + ")";
            var entFuel = KnowledgeBase.AskProperty((Name)strEntFuel);
            string fuelQ = entFuel.Value.ToString();
            return fuelQ.Equals("True");
        }

        public int GetEntitiesGUID(string prefab)
        {
            for (int i = 0; i < this.Entities.Count; i++)
            {
                if (this.Entities[i].Prefab.Equals(prefab))
                {
                    return this.Entities[i].GUID;
                }
            }
            return 0;
        }

        public int GetEquippableGUID(string prefab)
        {
            foreach (Pair<string,int> item in this.Equipped)
            {
                if (item.Item1 == prefab)
                {
                    return item.Item2;
                }
            }
            foreach (Tuple<string, int, int> item in this.Inventory)
            {
                if (item.Item1 == prefab)
                {
                    return item.Item2;
                }
            }
            return 0;
        }

        public int GetInventoryGUID(string prefab)
        {
            foreach (Tuple<string, int, int> item in this.Inventory)
            {
                if (item.Item1 == prefab)
                {
                    return item.Item2;
                }
            }
            return 0;
        }

        public int GetEquippedGUID(string prefab)
        {
            foreach (var item in this.Equipped)
            {
                if (item.Item1 == prefab)
                {
                    return item.Item2;
                }
            }
            return 0;
        }

        public bool EntityIsPickable(string entity)
        {
            foreach (ObjectProperties item in this.Entities)
            {
                if (item.Prefab == entity)
                {
                    return item.IsPickable;
                }
            }
            return false;
        }

        public bool EntityIsCollectable(string entity)
        {
            foreach (ObjectProperties item in this.Entities)
            {
                if (item.Prefab == entity)
                {
                    return item.IsCollectable;
                }
            }
            return false;
        }

        public bool IsEquipped(string item)
        {
            foreach (var equip in this.Equipped)
            {
                if (equip.Item1 == item)
                {
                    return true;
                }
            }
            return false;
        }

        public string CompleteNextActionInfo(string info)
        {
            if (info == "berrybush")
            {
                foreach (var entity in this.Entities)
                {
                    if (entity.Prefab == "berrybush")
                    {
                        string realprefab = entity.RealPrefab;
                        if (realprefab == "berrybush" || realprefab == "berrybush2")
                        {
                            return "";
                        }
                        else
                        {
                            return "berries_juicy";
                        }
                    }                   
                }
                return info;
            }
            else
            {
                return info;
            }
        }

        private float DistanceCalculator(string guid)
        {
            string searchPosx = "PosX(" + guid + ")";
            var posx = this.KnowledgeBase.AskProperty((Name)searchPosx).Value.ToString();
            string searchPosz = "PosZ(" + guid + ")";
            var posz = this.KnowledgeBase.AskProperty((Name)searchPosz).Value.ToString();

            float Posx = float.Parse(posx);
            float Posz = float.Parse(posz);

            return Convert.ToSingle(Math.Pow(Convert.ToDouble(this.Walter.Position.Item1 - Posx), 2) + Math.Pow(Convert.ToDouble(this.Walter.Position.Item2 - Posz), 2));
        }
    }
}
