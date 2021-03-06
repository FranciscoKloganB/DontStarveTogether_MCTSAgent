using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.Actions;
using MCTS.DST;
using System.Linq;
using MCTS.DST.Resources.Edibles;
using MCTS.DST.Resources.Materials;
using MCTS.DST.Resources.Buildables;
using MCTS.DST.Resources.NPCs;

namespace MCTS.DST.WorldModels
{
    public class WorldModelDST
    {
        public Character Walter;
        public HashSet<ActionDST> AvailableActions;
        public List<WorldObjectData> WorldObjects;
        public List<FireData> Fire;

        public Dictionary<string, int> Fuel;
        public Dictionary<string, int> PossessedItems;
        
        public HashSet<string> EquippedItems;

        public float Cycle;
        public int[] CycleInfo;

        protected WorldModelDST Parent;

        public WorldModelDST(Character character, List<WorldObjectData> worldObjects, Dictionary<string, int> possessedItems, HashSet<string> equippedItems, float cycle, int[] cycleInfo, HashSet<ActionDST> availableActions, WorldModelDST parent, Dictionary<string, int> fuel, List<FireData> fire)
        {
            this.Walter = character;
            this.WorldObjects = worldObjects;
            this.PossessedItems = possessedItems;
            this.EquippedItems = equippedItems;
            this.Cycle = cycle; // represents the stage of the day
            this.CycleInfo = cycleInfo; // stores the amount of time each phase of the day lasts. The phases are day, dusk and night
            this.AvailableActions = availableActions;
            this.Parent = parent;
            this.Fire = fire;
            this.Fuel = fuel;
        }

        public WorldModelDST(PreWorldState preWorldState)
        {
            this.Parent = null;
            this.Walter = preWorldState.Walter;
            this.Cycle = preWorldState.Cycle;
            this.CycleInfo = preWorldState.CycleInfo;

            //Getting Fire Info
            this.Fire = new List<FireData>(preWorldState.Fire);

            //Getting Fuel items from PreWorldState
            this.Fuel = new Dictionary<string, int>();
            for (int i = 0; i < preWorldState.Fuel.Count; i++)
            {
                this.Fuel[preWorldState.Fuel.ElementAt(i).Key] = preWorldState.Fuel.ElementAt(i).Value.Item2;
            }

            //Getting Inventory from PreWorldState  
            this.PossessedItems = new Dictionary<string, int>();
            for (int i = 0; i < preWorldState.Inventory.Count; i++)
            {
                this.PossessedItems[preWorldState.Inventory.ElementAt(i).Key] = preWorldState.Inventory.ElementAt(i).Value.Item2;
            }

            //Getting Equipped items from PreWorldState
            this.EquippedItems = new HashSet<string>();
            for (int i = 0; i < preWorldState.Equipped.Count; i++)
            {                
                this.EquippedItems.Add(preWorldState.Equipped.ElementAt(i).Key);
            }

            //Getting WorldObjects from PreWorldState's Entities
            this.WorldObjects = new List<WorldObjectData>();
            for (int i = 0; i < preWorldState.Entities.Count; i++)
            {
                var worldObjectData = new WorldObjectData(preWorldState.Entities[i].Prefab, preWorldState.Entities[i].Quantity, preWorldState.Entities[i].Position.Item1, preWorldState.Entities[i].Position.Item2);
                this.WorldObjects.Add(worldObjectData);
            }

            //Getting Available Actions
            this.AddAvailableActions(preWorldState);
        }

        private void AddAvailableActions(PreWorldState preWorldState)
        {
            Dictionary<string, WorldResource> materialBase = MaterialDict.Instance.materialBase;
            Dictionary<string, Food> foodBase = FoodDict.Instance.foodBase;
            Dictionary<string, Buildable> buildableBase = BuildablesDict.Instance.buildableBase;
            Dictionary<string, NPC> npcBase = NPCDict.Instance.npcBase;

            this.AvailableActions = new HashSet<ActionDST>();
            this.AvailableActions.Add(new Wander());

            // Adds actions regarding World items / entities.
            for (int i = 0; i < this.WorldObjects.Count; i++)
            {
                string objectName = this.WorldObjects[i].ObjectName;
                Console.WriteLine("new world object - " + objectName);

                if (objectName.Equals("firepit") || objectName.Equals("campfire"))
                {
                    if (this.HasFuel())
                    {
                        this.AvailableActions.Add(new AddFuel(objectName));
                    }

                    if (this.IsNight())
                    {
                        this.AvailableActions.Add(new HoldPosition(objectName));
                    }
                }
                else if (npcBase.ContainsKey(objectName))
                {
                    NPC npc = npcBase[objectName];
                    ActionDST action = npc.GetAction(this);
                    if (action is Feed feedAction)
                    {
                        HashSet<string> diet = feedAction.diet;
                        IEnumerable<string> possessedDiet = diet.Intersect(PossessedItems.Keys);
                        if (!possessedDiet.IsEmpty())
                        {
                            Console.WriteLine("possessed diet is empty");
                            feedAction.food = possessedDiet.First();
                            AddAction(feedAction);
                        }
                    }
                    else
                    {
                        AddAction(action);
                    }
                }
                else if (foodBase.ContainsKey(objectName))
                {
                    PickUpFoodBehavior(objectName);
                }
                else if (materialBase.ContainsKey(objectName))
                {
                    PickUpMaterialBehavior(materialBase, objectName);
                }
            }

            // Adds actions regarding Equipped items.
            foreach (string equippedItemName in this.EquippedItems)
            {
                AddAction(new Unequip(equippedItemName));
            }

            // Adds actions regarding Possessed items.
            for (int i = 0; i < this.PossessedItems.Count; i++)
            {
                string possessedItem = this.PossessedItems.ElementAt(i).Key;
                if (foodBase.ContainsKey(possessedItem))
                {
                    this.AvailableActions.Add(new Eat(possessedItem));
                }
                else if (buildableBase.ContainsKey(possessedItem))
                { // Possessed Item can now be equipped.
                    AddAction(new Equip(possessedItem));
                }

                // TODO - Does it make any sense for this to be here? PickUp objects already possessed?
                else if (materialBase.ContainsKey(possessedItem))
                {
                    WorldResource material = materialBase[possessedItem];
                    if (material.IsPrimitive)
                    {
                        BasicWorldResource targetMaterial = (BasicWorldResource) material;
                        PickUp.TryAddAction(this, targetMaterial);
                    }
                    else if (material is GatherableCompoundWorldResource)
                    {
                        GatherableCompoundWorldResource gatherableMaterial = (GatherableCompoundWorldResource)material;
                        Object basicMaterial = gatherableMaterial.ResourceWhenPicked;
                        if (!(basicMaterial is Food))
                        {
                            BasicWorldResource bMaterial = (BasicWorldResource)basicMaterial;
                            PickUp.TryAddAction(this, bMaterial);
                        }
                    }
                }
            }
        }

        private void PickUpFoodBehavior(string objectName)
        {
            this.AvailableActions.Add(new PickUp(objectName));
        }

        private void PickUpMaterialBehavior(Dictionary<string, WorldResource> materialBase, string objectName)
        {
            WorldResource material = materialBase[objectName];

            if (material.IsPickable)
            { // Material can be picked up by hand, no need to check if has tool.
                this.AvailableActions.Add(new PickUp(objectName));
            }
            else if (material is GatherableCompoundWorldResource)
            {
                this.AvailableActions.Add(new PickUp(objectName));
            }
            else if (material is CompoundWorldResource compoundMaterial)
            { // Material needs tools to be gathered / can only use PICK action. Gets the required tool.
                Tool tool = compoundMaterial.RequiredTool;
                if (tool is null)
                { // If tool can be hands, there is no need to check if tool is in inventory.
                    this.AvailableActions.Add(new PickUp(objectName));
                }
                else if (this.Possesses(tool.ResourceName))
                { // The necessary tool is already equiped.
                    this.AvailableActions.Add(new PickUp(objectName));
                }
                // If the material needs a tool that is not in the inventory, no action can be done.
            }
        }

        public HashSet<ActionDST> GetExecutableActions()
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
            var actions = new HashSet<ActionDST>(this.AvailableActions);
            var equippedItems = new HashSet<string>(this.EquippedItems);
            var possessedItems = this.PossessedItems.ToDictionary(entry => string.Copy(entry.Key), entry => entry.Value);
            var fuels = this.Fuel.ToDictionary(entry => string.Copy(entry.Key), entry => entry.Value);
            var fires = this.Fire.ConvertAll(fire => fire.Clone());
            var worldObjects = this.WorldObjects.ConvertAll(worldObject => worldObject.Clone());
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
                    this.PossessedItems[prefab] = this.PossessedItems[prefab] - quantity;
                }
            }
        }

        
        public void AddToPossessedItems(string prefab, int quantity)
        {
            if (this.PossessedItems.ContainsKey(prefab))
            {
                this.PossessedItems[prefab] = this.PossessedItems[prefab] + quantity;
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
                if (this.WorldObjects[i].ObjectName.Equals(prefab))
                {
                    if (this.WorldObjects[i].Quantity <= quantity)
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
            this.AvailableActions.RemoveWhere(obj => obj.Name.Equals(actionName));
            // this.AvailableActions.Remove(new ActionDST(actionName));
        }

        public void AddAction(ActionDST action)
        {   
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
            for (int i = 0; i < this.Fire.Count; i++)
            {
                dist = DistanceCalculator(this.Fire[i].PosX, this.Fire[i].PosZ);
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

        public bool HasFuel()
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
            if (place.Equals("fire"))
            {
                for (int i = 0; i < this.Fire.Count; i++)
                {
                    if (this.Fire[i].FireName.Equals(prefab))
                    {
                        return new Pair<int, int>(this.Fire[i].PosX, this.Fire[i].PosZ);
                    }
                }
            }
            else if (place.Equals("world"))
            {
                for (int i = 0; i < this.Fire.Count; i++)
                {
                    if (this.WorldObjects[i].ObjectName.Equals(prefab))
                    {
                        return new Pair<int, int>(this.WorldObjects[i].PosX, this.WorldObjects[i].PosZ);
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
    }
}