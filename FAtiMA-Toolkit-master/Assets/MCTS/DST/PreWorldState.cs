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
        public Character Walter;
        public float Cycle;
        public int[] CycleInfo;

        public List<ObjectProperties> Entities;
        public Dictionary<string, int> Equipped;
        public Dictionary<string, Pair<int, int>> Inventory;
        public Dictionary<string, Pair<int, int>> Fuel;
        public List<Tuple<string, int, int>> Fire;
        public KB KnowledgeBase;

        private static HashSet<string> minableBase = new HashSet<string>()
        {
            "rock1",
            "rock2",
            "rock_flintless",
            "rock_charcoal",
            "rock_obsidian",
            "pig_ruins_artichoke",
            "rock_moon",
            "rock_moon_shell",
            "rock_petrified_tree_short",
            "rock_petrified_tree_med",
            "rock_petrified_tree_tall",
            "rock_petrified_tree_old"
        };

        private static HashSet<string> choppableBase = new HashSet<string>()
        {
            "evergreen",
            "evergreen_tall",
            "evergreen_sparse",
            "deciduoustree",
            "marsh_tree",
            "mushtree_small",
            "mushtree_medium",
            "mushtree_tall",
            "mushtree_tall_webbed",
            "evergreen_sparse",
            "livingtree",
            "palmtree",
            "jungletree",
            "mangrovetree",
            "livingjungletree",
            "volcano_shrub",
            "rainforesttree_short",
            "rainforesttree_normal",
            "rainforesttree_tall",
            "rainforesttree_rot",
            "rainforesttree_short",
            "rainforesttree_rot_normal",
            "rainforesttree_rot_tall",
            "twiggy_short",
            "twiggy_normal",
            "twiggy_tall",
            "twiggy_old",
            "twiggytree",
        };

        public PreWorldState(KB knowledgeBase)
        {
            this.KnowledgeBase = knowledgeBase;
            this.Entities = new List<ObjectProperties>(); // it is a list which stores the information of the objects that exist in the world
            this.Equipped = new Dictionary<string, int>(); // Maps the name and the GUID of each equipped object
            this.Inventory = new Dictionary<string, Pair<int, int>>(); // maps prefab name to Pair<GUID, quantity of an object in the inventory>
            this.Fuel = new Dictionary<string, Pair<int, int>>(); // maps prefab name to Pair<GUID, quantity of an object that can be used as fuel>
            this.Fire = new List<Tuple<string, int, int>>(); // list of 3-tuples that contain the name and the position of the fires in the world

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
                this.Equipped[entPrefab] = entGuid;
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

                Pair<int, int> pair = new Pair<int, int>(entGuid, entQuantity);
                this.Inventory[entPrefab] = pair;

                if (IsFuel(strEntGuid))
                {                    
                    this.Fuel[entPrefab] = pair;
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
                            if (objectproperty.Prefab.Equals(realEntPrefab))
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
            return choppableBase.Contains(tree);
        }

        public bool IsBoulder(string boulder)
        {
            return minableBase.Contains(boulder);
        }

        public string RealEntityPrefab(string entity)
        {
            if (IsTree(entity))
            {
                return "tree";
            }
            else if (IsBoulder(entity))
            {
                return "boulder";
            }
            else if (entity.Equals("sapling"))
            {
                return "sapling";
            }
            else if (entity.Equals("twigs"))
            {
                return "twigs";
            }
            else if (entity.Equals("butterfly"))
            {
                return "butterfly";
            }
            else if (entity.Equals("berrybush"))
            {
                return "berrybush";
            }
            else if (entity.Equals("berrybush2"))
            {
                return "berrybush";
            }
            else if (entity.Equals("berrybush_juicy"))
            {
                return "berrybush";
            }
            else if (entity.Equals("log"))
            {
                return "log";
            }
            else if (entity.Equals("torch"))
            {
                return "torch";
            }
            else if (entity.Equals("grass"))
            {
                return "grass";
            }
            else if (entity.Equals("cutgrass"))
            {
                return "cutgrass";
            }
            else if (entity.Equals("carrot"))
            {
                return "carrot";
            }
            else if (entity.Equals("carrot_planted"))
            {
                return "carrot_planted";
            }
            else if (entity.Equals("berries"))
            {
                return "berries";
            }
            else if (entity.Equals("seeds"))
            {
                return "seeds";
            }
            else if (entity.Equals("flower"))
            {
                return "flower";
            }
            else if (entity.Equals("berries_juicy"))
            {
                return "berries";
            }
            else if (entity.Equals("rocks"))
            {
                return "rocks";
            }
            else if (entity.Equals("flint"))
            {
                return "flint";
            }
            else if (entity.Equals("axe"))
            {
                return "axe";
            }
            else if (entity.Equals("pickaxe"))
            {
                return "pickaxe";
            }
            else if (entity.Equals("campfire"))
            {
                return "campfire";
            }
            else if (entity.Equals("firepit"))
            {
                return "firepit";
            }
            else if (entity.Equals("pigman"))
            {
                return "pigman";
            }
            else
            {
                return "";
            }

        }

        public bool IsFire(string prefab)
        {
            return prefab.Equals("campfire") || prefab.Equals("firepit");
        }

        public bool IsFuel(string guid)
        {
            string strEntFuel = "IsFuel(" + guid + ")";
            var entFuel = KnowledgeBase.AskProperty((Name)strEntFuel);
            var fuelQ = entFuel.Value.ToString();
            return fuelQ.Equals("True");
        }

        public int GetEntitiesGUID(string prefab)
        {
            foreach (ObjectProperties entity in this.Entities)
            {
                if (entity.Prefab.Equals(prefab))
                {
                    return entity.GUID;
                }               
            }
            return 0;
        }

        public int GetEquippableGUID(string prefab)
        {
            int guid = 0;
            
            this.Equipped.TryGetValue(prefab, out guid);

            if (guid == 0 && this.Inventory.ContainsKey(prefab))
            {
                guid = this.Inventory[prefab].Item1;
            }

            return guid;
        }

        public int GetInventoryGUID(string prefab)
        {
            int guid = 0;

            if (this.Inventory.ContainsKey(prefab))
            {
                guid = this.Inventory[prefab].Item1;
            }

            return guid;
        }

        public int GetEquippedGUID(string prefab)
        {
            int guid = 0;

            this.Equipped.TryGetValue(prefab, out guid);

            return guid;
        }

        public bool EntityIsPickable(string entity)
        {
            foreach (ObjectProperties item in this.Entities)
            {
                if (item.Prefab.Equals(entity))
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
                if (item.Prefab.Equals(entity))
                {
                    return item.IsCollectable;
                }
            }
            return false;
        }

        public bool IsEquipped(string item)
        {
            return this.Equipped.ContainsKey(item);
        }

        public string CompleteNextActionInfo(string info)
        {
            if (info.Equals("berrybush"))
            {
                foreach (var entity in this.Entities)
                {
                    if (entity.Prefab.Equals("berrybush"))
                    {
                        string realprefab = entity.RealPrefab;
                        if (realprefab.Equals("berrybush") || realprefab.Equals("berrybush2"))
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
