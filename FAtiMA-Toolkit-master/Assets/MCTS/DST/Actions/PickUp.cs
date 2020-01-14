﻿using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;
using MCTS.DST.Resources.Materials;


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


        private void ExecuteBasicWorldResourceBehaviour(WorldModelDST worldState, BasicWorldResource targetMaterial)
        {
            worldState.AddToPossessedItems(targetMaterial.MaterialName, targetMaterial.Quantity);
            if (targetMaterial.IsFuel)
            {
                worldState.AddToFuel(targetMaterial.MaterialName, targetMaterial.Quantity);
            }

            // TODO - Add Construct behavior.
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            worldState.Cycle += this.Duration;
            worldState.UpdateSatiation(-1.0f);
            worldState.Walter.Position = worldState.GetNextPosition(this.Target, "world");

            WorldResource material = this.MaterialBase[this.Target];
            worldState.RemoveFromWorld(this.Target, 1);
            
            if (!material.IsPrimitive)
            { // ComposedMaterial behaviour.
                CompoundWorldResource targetMaterial = (CompoundWorldResource) material;
                for (int i = 0; i < targetMaterial.ComposingItems.Count; i++)
                {
                    BasicWorldResource primitiveComponent = targetMaterial.ComposingItems[i];
                    ExecuteBasicWorldResourceBehaviour(worldState, primitiveComponent);
                }
            }
            else
            { // PrimitiveMaterial behaviour.
                BasicWorldResource targetMaterial = (BasicWorldResource) material;
                ExecuteBasicWorldResourceBehaviour(worldState, targetMaterial);
            }
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
            Tool tool = compoundMaterial.usableTool;

            if (tool == null)
            { // If tool can be hands, but the action needs to be PICK.
                return new List<Pair<string, string>>(1)
                {
                    new Pair<string, string>("Action(PICK, -, -, -, -)", preWorldState.GetEntitiesGUID(this.Target).ToString())
                };
            }

            string toolName = tool.MaterialName;
            string harvestingActionName = compoundMaterial.doableToolAction;

            if (preWorldState.IsEquipped(toolName))
            { // If the necessary tool is already equiped, harvests.
                return new List<Pair<string, string>>(1)
                {
                    new Pair<string, string>("Action(" + harvestingActionName + ", -, -, -, -)", preWorldState.GetEntitiesGUID(this.Target).ToString())
                };
            }

            // TODO - Check if the required tool is in the inventory. If not then what???????

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
