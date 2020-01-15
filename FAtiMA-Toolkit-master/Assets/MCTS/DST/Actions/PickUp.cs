using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;
using MCTS.DST.Resources.Materials;
using MCTS.DST.Resources.Buildables;
using System.Linq;

namespace MCTS.DST.Actions
{

    public class PickUp : ActionDST
    {
        private Dictionary<string, WorldResource> MaterialBase { get; } = MaterialDict.Instance.materialBase;
        public string Target;
        public float Duration;
        private static readonly string ActionName = "Pickup_";

        public PickUp(string target) : base(ActionName + target)
        {
            this.Target = target;
            this.Duration = 0.33f;
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

                    // Commenting this as it isn't easily debugable.
                    /*
                    if (!(worldModel.Possesses(requiredMaterialName) && worldModel.PossessedItems[requiredMaterialName] >= requiredMaterialQuantity))
                    {
                        nowCanDo = false;
                        break;
                    }
                    */
                    if (!worldModel.Possesses(requiredMaterialName))
                    {
                        nowCanDo = false;
                        break;
                    }
                    else if (worldModel.PossessedItems[requiredMaterialName] < requiredMaterialQuantity)
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
            worldState.Cycle += this.Duration;
            worldState.UpdateSatiation(-1.0f);
            worldState.Walter.Position = worldState.GetNextPosition(this.Target, "world");
            worldState.RemoveFromWorld(this.Target, 1);
            
            WorldResource material = this.MaterialBase[this.Target];
            if (material.IsPrimitive)
            { // PrimitiveMaterial behaviour.
                BasicWorldResource targetMaterial = (BasicWorldResource) material;
                worldState.AddToPossessedItems(targetMaterial.MaterialName, targetMaterial.Quantity);
                if (targetMaterial.IsFuel)
                {
                    worldState.AddToFuel(targetMaterial.MaterialName, targetMaterial.Quantity);
                }

                // TryAddAction(worldState, targetMaterial);
            }
            /*
            else if (material is GatherableCompoundWorldResource)
            {
                GatherableCompoundWorldResource gatherableMaterial = (GatherableCompoundWorldResource)material;
                BasicWorldResource basicMaterial = gatherableMaterial.ResourceWhenPicked;
                // TryAddAction(worldState, basicMaterial);
            }
            */

            /*
            If the resource is compound, the agent mines / chops it, and the composing 
            basic materials (rocks / flint / nitre / logs) remain on the ground.
            There is no need for additional behaviour here, as the basic materials
            aren't put in the inventory.
            */


            // TODO - Add new PickUp Actions if picked item is tool.
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            WorldResource material = this.MaterialBase[this.Target];

            if (material.IsPickable)
            { // Material can be picked up by hand, using the PICKUP action.
                Console.WriteLine("decompose, material is pickable.");
                return new List<Pair<string, string>>(1)
                {
                    new Pair<string, string>("Action(PICKUP, -, -, -, -)", preWorldState.GetEntitiesGUID(this.Target).ToString())
                };
            }
            // Material needs tools to be gathered / can only use PICK action.
            CompoundWorldResource compoundMaterial = (CompoundWorldResource) material;
            // Gets tool needed to gather target.
            Tool tool = compoundMaterial.RequiredTool;

            if (tool is null)
            { // If tool can be hands, but the action needs to be PICK.
                return new List<Pair<string, string>>(1)
                {
                    new Pair<string, string>("Action(PICK, -, -, -, -)", preWorldState.GetEntitiesGUID(this.Target).ToString())
                };
            }

            string toolName = tool.MaterialName;
            string harvestingActionName = compoundMaterial.RequiredToolAction;

            if (preWorldState.IsEquipped(toolName))
            { // If the necessary tool is already equiped, harvests.
                return new List<Pair<string, string>>(1)
                {
                    new Pair<string, string>("Action(" + harvestingActionName + ", -, -, -, -)", preWorldState.GetEntitiesGUID(this.Target).ToString())
                };
            }

            return new List<Pair<string, string>>(2)
            { // Constructs the compound action of equipping the tool and harvesting.
                new Pair<string, string>("Action(EQUIP, " + preWorldState.GetInventoryGUID(toolName).ToString() + ", -, -, -)", "-"),
                new Pair<string, string>("Action(" + harvestingActionName + ", -, -, -, -)", preWorldState.GetEntitiesGUID(this.Target).ToString())
            };
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
