using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;
using MCTS.DST.Resources.Materials;
using MCTS.DST.Resources.Buildables;
using System.Linq;
using MCTS.DST.Resources.Edibles;

namespace MCTS.DST.Actions
{

    public class PickUp : ActionDST
    {

        private static readonly float duration = 0.33f;
        private static readonly string action = "Pickup_";
        private readonly string target;
        private Dictionary<string, Food> foodBase { get; } = FoodDict.Instance.foodBase;
        private Dictionary<string, WorldResource> materialBase { get; } = MaterialDict.Instance.materialBase;

        public PickUp(string target) : base(action + target)
        {
            this.target = target;
        }

        public static void TryAddAction(WorldModelDST worldModel, WorldResource resource)
        {
            Dictionary<string, int> resourceRecipes = resource.Recipes;
            Dictionary<string, Buildable> buildableBase = BuildablesDict.Instance.buildableBase;

            for (int i = 0; i < resourceRecipes.Count; i++)
            {
                string recipeName = resourceRecipes.ElementAt(i).Key;

                // TODO - Add all recipe names in resource.Recipes to the BuildableBase dictionary.

                if (!buildableBase.ContainsKey(recipeName))
                {
                    continue;
                }

                Buildable buildable = buildableBase[recipeName];
                Dictionary<string, int> requiredMaterials = buildable.RequiredMaterials;

                bool nowCanDo = true;
                for (int j = 0; j < requiredMaterials.Count; j++)
                {
                    string requiredMaterialName = requiredMaterials.ElementAt(j).Key;
                    int requiredMaterialQuantity = requiredMaterials.ElementAt(j).Value;

                    if (!worldModel.Possesses(requiredMaterialName, requiredMaterialQuantity))
                    {
                        nowCanDo = false;
                        break;
                    }
                }

                if (nowCanDo)
                {
                    worldModel.AddAction(new Construct(recipeName));
                }
            }
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            worldState.Cycle += duration;
            // worldState.UpdateSatiation(-1.0f);
            // worldState.Walter.Position = worldState.GetNextPosition(this.Target, "world");
            worldState.RemoveFromWorld(this.target, 1);
            
            if (this.materialBase.ContainsKey(this.target))
            {
                MaterialBehavior(worldState);
            }
            else if (this.foodBase.ContainsKey(this.target))
            {
                FoodBehavior(worldState);
            }

            // TODO - Add new PickUp Actions if picked item is tool.
        }

        private void MaterialBehavior(WorldModelDST worldState)
        {
            WorldResource material = this.materialBase[this.target];
            material.GetBonuses(worldState);

            if (material.IsPrimitive)
            { // PrimitiveMaterial behaviour.
                BasicWorldResource targetMaterial = (BasicWorldResource)material;
                worldState.AddToPossessedItems(targetMaterial.MaterialName, targetMaterial.Quantity);
                if (targetMaterial.IsFuel)
                {
                    worldState.AddToFuel(targetMaterial.MaterialName, targetMaterial.Quantity);
                }
                TryAddAction(worldState, targetMaterial);
            }
            else if (material is GatherableCompoundWorldResource)
            {
                GatherableCompoundWorldResource gatherableMaterial = (GatherableCompoundWorldResource)material;
                Object basicMaterial = gatherableMaterial.ResourceWhenPicked;
                if (basicMaterial is Food)
                {
                    Food food = (Food)basicMaterial;
                    worldState.AddToPossessedItems(food.FoodName, 1);
                    worldState.AddAction(new Eat(food.FoodName));
                }
                else
                {
                    BasicWorldResource bMaterial = (BasicWorldResource)basicMaterial;
                    worldState.AddToPossessedItems(bMaterial.MaterialName, bMaterial.Quantity);
                    TryAddAction(worldState, bMaterial);
                }
            }

            /*
            If the resource is compound, the agent mines / chops it, and the composing 
            basic materials (rocks / flint / nitre / logs) remain on the ground.
            There is no need for additional behaviour here, as the basic materials
            aren't put in the inventory.
            */
        }

        private void FoodBehavior(WorldModelDST worldState)
        {
            Food food = this.foodBase[this.target];
            worldState.AddToPossessedItems(food.FoodName, 1);
            ActionDST eatAction = new Eat(food.FoodName);
            worldState.AddAction(eatAction);
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            if (this.foodBase.ContainsKey(this.target))
            {
                Food food = this.foodBase[this.target];
                return new List<Pair<string, string>>(1)
                {
                    new Pair<string, string>("Action(PICKUP, -, -, -, -)", preWorldState.GetEntitiesGUID(this.target).ToString())
                };
            }
            else if (this.materialBase.ContainsKey(this.target))
            {
                WorldResource material = this.materialBase[this.target];

                if (material.IsPickable)
                { // Material can be picked up by hand, using the PICKUP action.
                    return new List<Pair<string, string>>(1)
                    {
                        new Pair<string, string>("Action(PICKUP, -, -, -, -)", preWorldState.GetEntitiesGUID(this.target).ToString())
                    };
                }
                // Material needs tools to be gathered / can only use PICK action.
                CompoundWorldResource compoundMaterial = (CompoundWorldResource)material;
                // Gets tool needed to gather target.
                Tool tool = compoundMaterial.RequiredTool;

                if (tool is null)
                { // If tool can be hands, but the action needs to be PICK.
                    return new List<Pair<string, string>>(1)
                    {
                        new Pair<string, string>("Action(PICK, -, -, -, -)", preWorldState.GetEntitiesGUID(this.target).ToString())
                    };
                }

                string toolName = tool.MaterialName;
                string harvestingActionName = compoundMaterial.RequiredToolAction;

                if (preWorldState.IsEquipped(toolName))
                { // If the necessary tool is already equiped, harvests.
                    return new List<Pair<string, string>>(1)
                    {
                        new Pair<string, string>("Action(" + harvestingActionName + ", -, -, -, -)", preWorldState.GetEntitiesGUID(this.target).ToString())
                    };
                }

                return new List<Pair<string, string>>(2)
                { // Constructs the compound action of equipping the tool and harvesting.
                    new Pair<string, string>("Action(EQUIP, " + preWorldState.GetInventoryGUID(toolName).ToString() + ", -, -, -)", "-"),
                    new Pair<string, string>("Action(" + harvestingActionName + ", -, -, -, -)", preWorldState.GetEntitiesGUID(this.target).ToString())
                };
            }
            return base.Decompose(preWorldState);
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
