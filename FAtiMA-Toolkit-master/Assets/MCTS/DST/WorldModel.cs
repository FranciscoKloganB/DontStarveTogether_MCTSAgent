using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.Actions;
using MCTS.DST;
using System.Linq;
using MCTS.DST.Resources.Edibles;

namespace MCTS.DST.WorldModels
{
    public class WorldModelDST
    {
        public Character Walter;
        public List<ActionDST> AvailableActions;
        public List<WorldObjectData> WorldObjects;
        public List<FireData> Fire;

        public Dictionary<string, int> Fuel;
        public Dictionary<string, int> PossessedItems;
        
        public HashSet<string> EquippedItems;

        public float Cycle;
        public int[] CycleInfo;

        protected WorldModelDST Parent;

        public WorldModelDST(Character character, List<WorldObjectData> worldObjects, Dictionary<string, int> possessedItems, HashSet<string> equippedItems, float cycle, int[] cycleInfo, List<ActionDST> availableActions, WorldModelDST parent, Dictionary<string, int> fuel, List<FireData> fire)
        {
            this.Walter = character;
            this.WorldObjects = worldObjects; // stores in the name, quantity and the position of world objects
            this.PossessedItems = possessedItems;
            this.EquippedItems = equippedItems; // stores the name of equiped items
            this.Cycle = cycle; // represents the stage of the day
            this.CycleInfo = cycleInfo; // stores the amount of time each phase of the day lasts. The phases are day, dusk and night
            this.AvailableActions = availableActions; // list of actions that the character can do in the current state. It is created based on the PreWorldState information and it is updated when an action is done
            this.Parent = parent; // World Model which originates the current one by means of ActionApplyEffect
            this.Fuel = fuel;
            this.Fire = fire; // list of 3-tuples that contain the name and the position of the fires in the world
        }

        public WorldModelDST(PreWorldState preWorldState)
        {
            this.Parent = null;
            this.Walter = preWorldState.Walter;
            this.Cycle = preWorldState.Cycle;
            this.CycleInfo = preWorldState.CycleInfo;

            //Getting Fire Info
            this.Fire = new List<FireData>();
            for (int i = 0; i < preWorldState.Fire.Count; i++)
            {
                var fireData = new FireData(preWorldState.Fire[i].Item1, preWorldState.Fire[i].Item2, preWorldState.Fire[i].Item3);
                this.Fire.Add(fireData);
            }

            //Getting Fuel items from PreWorldState
            this.Fuel = new Dictionary<string, int>();
            for (int i = 0; i < preWorldState.Fuel.Count; i++)
            {
                this.Fuel[preWorldState.Fuel[i].Item1] = preWorldState.Fuel[i].Item3;
            }

            //Getting Inventory from PreWorldState  
            this.PossessedItems = new Dictionary<string, int>();
            for (int i = 0; i < preWorldState.Inventory.Count; i++)
            {
                this.PossessedItems[preWorldState.Inventory[i].Item1] = preWorldState.Inventory[i].Item3;   
            }

            //Getting Equipped items from PreWorldState
            this.EquippedItems = new HashSet<string>();
            for (int i = 0; i < preWorldState.Equipped.Count; i++)
            {                
                this.EquippedItems.Add(preWorldState.Equipped[i].Item1);
            }

            //Getting WorldObjects from PreWorldState's Entities
            this.WorldObjects = new List<WorldObjectData>();
            for (int i = 0; i < preWorldState.Entities.Count; i++)
            {
                var worldObjectData = new WorldObjectData(preWorldState.Entities[i].Prefab, preWorldState.Entities[i].Quantity, preWorldState.Entities[i].Position.Item1, preWorldState.Entities[i].Position.Item2);
                this.WorldObjects.Add(worldObjectData);
            }

            //Getting Available Actions
            ActionDST action = new Wander();
            this.AvailableActions = new List<ActionDST>();
            this.AvailableActions.Add(action);

            if (Possesses("berries"))
            {
                action = new Eat("berries");
                this.AvailableActions.Add(action);
            }
            if (Possesses("carrot"))
            {
                action = new Eat("carrot");
                this.AvailableActions.Add(action);
            }
        }

        public List<ActionDST> GetExecutableActions()
        {
            return this.AvailableActions;
        }

        public WorldModelDST()
        {
        }

        public WorldModelDST GenerateChildWorldModel()
        {
            var cycle = this.Cycle;
            var cycleInfo = this.CycleInfo.ToArray();
            var walter = this.Walter.Clone();
            var equippedItems = new HashSet<string>(this.EquippedItems);
            var possessedItems = this.PossessedItems.ToDictionary(entry => string.Copy(entry.Key), entry => entry.Value);
            var fuels = this.Fuel.ToDictionary(entry => string.Copy(entry.Key), entry => entry.Value);
            var fires = this.Fire.ConvertAll(fire => fire.Clone());
            var worldObjects = this.WorldObjects.ConvertAll(worldObject => worldObject.Clone());
            var actions = this.AvailableActions.ConvertAll(action => action.Clone());
            return new WorldModelDST(walter, worldObjects, possessedItems, equippedItems, cycle, cycleInfo, actions, this, fuels, fires);
        }

        public int FoodQuantity()
        {
            int quantity = 0;
            for (int index = 0; index < this.PossessedItems.Count; index++)
            {
                var item = this.PossessedItems.ElementAt(index);
                if (FoodDict.Instance.foodBase.ContainsKey(item.Key))
                {
                    quantity += item.Value;
                }
            }
            return quantity;
        }

        public bool Possesses(string prefab)
        {
            return this.PossessedItems.ContainsKey(prefab);
        }

        public bool Possesses(string prefab, int quantity)
        {
            return this.PossessedItems.ContainsKey(prefab) ? this.PossessedItems[prefab] >= quantity : false;
        }

        public bool IsEquipped(string prefab)
        {
            return this.EquippedItems.Contains(prefab);
        }

        public bool WorldHas(string prefab)
        {
            for (int i = 0; i < this.WorldObjects.Count; i++)
            {
                if (this.WorldObjects[i].ObjectName == prefab)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddToFire(string prefab, int posX, int posZ)
        {
            this.Fire.Add(new FireData(prefab, posX, posZ));
        }

        public void RemoveFromFuel(string prefab)
        {
            if (this.Fuel.ContainsKey(prefab))
            {
                if (this.Fuel[prefab] <= 1)
                {
                    this.Fuel.Remove(prefab);
                } 
                else
                {
                    this.Fuel[prefab] -= 1;
                }
            }
        }

        public void AddToFuel(string prefab, int quantity)
        {
            if (this.Fuel.ContainsKey(prefab))
            {
                this.Fuel[prefab] += quantity;
            }
            else
            {
                this.Fuel[prefab] = quantity;
            }
        }

        public void RemoveFromPossessedItems(string prefab, int quantity)
        {
            if (this.PossessedItems.ContainsKey(prefab))
            {
                if (this.PossessedItems[prefab] < quantity)
                {
                    throw new ArgumentException("Insufficient Quantity");
                }
                else if (this.PossessedItems[prefab] == quantity)
                {
                    this.PossessedItems.Remove(prefab);
                }
                else
                {
                    this.PossessedItems[prefab] -= quantity;
                }
            }
        }

        
        public void AddToPossessedItems(string prefab, int quantity)
        {
            if (this.PossessedItems.ContainsKey(prefab))
            {
                this.PossessedItems[prefab] += quantity;
            }
            else
            {
                this.PossessedItems[prefab] = quantity;
            }
        }

        public void RemoveFromWorld(string prefab, int quantity)
        {
            // TODO This doesn't make sense. We are lacking PosX and PosZ and in DST you can only pick up the entire stack!
            for (int i = 0; i < this.WorldObjects.Count; i++)
            {
                if (this.WorldObjects[i].ObjectName == prefab)
                {
                    if (this.WorldObjects[i].Quantity < quantity)
                    {
                        throw new ArgumentException("Insufficient Quantity");
                    }
                    else if (this.WorldObjects[i].Quantity == quantity)
                    {
                        this.WorldObjects.RemoveAt(i);
                    }
                    else
                    {
                        this.WorldObjects[i].Quantity -= quantity;
                    }
                    return;
                }
            }
        }

        public void AddToWorld(string prefab, int quantity, int posX, int posZ)
        {
            for (int i = 0; i < this.WorldObjects.Count; i++)
            {
                if (this.WorldObjects[i].ObjectName == prefab)
                {
                    this.WorldObjects[i].Quantity += quantity;
                    return;
                }
            }
            this.WorldObjects.Add(new WorldObjectData(prefab, quantity, posX, posZ));
        }

        public void RemoveAction(string actionName)
        {
            for (int i = 0; i < this.AvailableActions.Count; i++)
            {
                if (this.AvailableActions[i].Name.Equals(actionName))
                {
                    this.AvailableActions.RemoveAt(i);
                    return;
                }
            }
        }

        public void AddAction(ActionDST action)
        {   // TODO: Use HASHSET instead of LIST<ACTION DST> for faster lookup. Add action if not in Hashset.
            for (int i = 0; i < this.AvailableActions.Count; i++)
            {
                if (this.AvailableActions[i].Name == action.Name)
                {
                    continue;
                }
            }
            this.AvailableActions.Add(action);
        }

        public void AddToEquipped(string item)
        {
            this.EquippedItems.Add(item);
        }

        public void RemoveFromEquipped(string item)
        {
            this.EquippedItems.Remove(item);
        }

        public bool IsNight()
        {
            return GetCycleStage() == 2;
        }

        public bool IsDusk()
        {
            return GetCycleStage() == 1;
        }

        public bool IsDay()
        {
            return GetCycleStage() == 0;
        }

        public short GetCycleStage()
        {
            float cycleProgress = Cycle;
            float duskLength = CycleInfo[1];
            float nightLength = CycleInfo[2];
            float cycleLength = CycleInfo.Sum();

            if (cycleProgress >= cycleLength - nightLength)
            {   // It's night time
                return 2;
            }
            else if (cycleProgress >= cycleLength - nightLength - duskLength)
            {   // It's dusk time
                return 1;
            }
            else
            {   // It's day time
                return 0;
            }
        }
      

        public float LightValueDay()
        {
            if (this.IsEquipped("torch"))
            {
                return 0.0f;
            }
            else if (this.Possesses("torch"))
            {
                return 1.0f;
            }
            else if ((this.Possesses("cutgrass", 2) && this.Possesses("twigs", 2)) || (this.Possesses("log", 2) && this.Possesses("cutgrass", 3)) || (this.Possesses("log", 2) && this.Possesses("rocks", 12)))
            {
                return 0.6f;
            }
            else if (this.Possesses("cutgrass") || this.Possesses("twigs") || this.Possesses("log") || this.Possesses("rocks"))
            {
                return 0.3f;
            }
            else
            {
                return 0.0f;
            }
        }

        public float LightValueNight()
        {
            float maxdistance = float.MaxValue;
            float dist = float.MaxValue;
            foreach (var fire in this.Fire)
            {
                dist = DistanceCalculator(fire.Item2, fire.Item3);
                if (dist < maxdistance)
                {
                    maxdistance = dist;
                }
            }

            if (this.IsEquipped("torch") || dist < 6)
            {
                return 1.0f;
            }
            else
            {
                return 0.0f;
            }
        }

        //public float AxePickaxeValue()
        //{
        //    Boolean b1 = this.Possesses("axe");
        //    Boolean b2 = this.IsEquipped("axe");
        //    Boolean b3 = this.Possesses("pickaxe");
        //    Boolean b4 = this.IsEquipped("pickaxe");

        //    if ((b1 || b2) && (b3 || b4))
        //    {
        //        return 1.0f;
        //    }
        //    else if ((WorldHas("tree") && (b1 || b2)) || (WorldHas("boulder") && (b3 || b4)))
        //    {
        //        return 0.75f;
        //    }
        //    else if (b1 || b2 || b3 || b4)
        //    {
        //        return 0.4f;
        //    }
        //    else
        //    {
        //        return 0.0f;
        //    }
        //}

        public float FoodValue()
        {
            int foodCount = this.FoodQuantity();
            float invFoodValue;

            if (foodCount >= 5)
            {
                invFoodValue = 1.0f;
            }
            else
            {
                float fc = Convert.ToSingle(foodCount);
                invFoodValue = fc / 5.0f;
            }

            float hungerValue;

            if (this.Walter.Satiation >= 100)
            {
                hungerValue = 1;
            }
            else
            {
                hungerValue = Convert.ToSingle(1.0 / (Math.Pow(Convert.ToDouble((Convert.ToSingle(this.Walter.Satiation) - 150.0)/50.0), 2)));
            }

            return hungerValue * 0.6f + invFoodValue * 0.4f;
        }

        public Boolean HasFuel()
        {
            return this.Fuel.Count > 0;
        }

        private float DistanceCalculator(int posxObject, int poszObject)
        {
            float Posx = Convert.ToSingle(posxObject);
            float Posz = Convert.ToSingle(poszObject);

            return Convert.ToSingle(Math.Pow(Convert.ToDouble(this.Walter.Position.Item1 - Posx), 2) + Math.Pow(Convert.ToDouble(this.Walter.Position.Item2 - Posz), 2));
        }

        public Pair<int, int> GetNextPosition(string prefab, string place)
        {
            if (place == "fire")
            {
                foreach (var item in this.Fire)
                {
                    if (item.Item1 == prefab)
                    {
                        Pair<int, int> pair = new Pair<int, int>(item.Item2, item.Item3);
                        return pair;
                    }                   
                }
            }
            else if (place == "world")
            {
                foreach (var item in this.WorldObjects)
                {
                    if (item.Item1.Item1 == prefab)
                    {
                        return item.Item2;
                    }
                }
            }

            return this.Walter.Position;
        }

        public void UpdateSatiation(float value)
        {
            this.Walter.UpdateSatiation(value);
        }

        public void UpdateHP(float value)
        {
            this.Walter.UpdateHP(value);
        }


        public void UpdateSanity(float value)
        {
            this.Walter.UpdateSanity(value);
        }

        public bool IsTerminal()
        {
            return this.Walter.HP == 0;
        }
    }
}