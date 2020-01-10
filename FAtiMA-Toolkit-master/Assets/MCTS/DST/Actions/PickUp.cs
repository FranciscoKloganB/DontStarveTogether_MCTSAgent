using System;
using System.Collections.Generic;
using Utilities;
using MCTS.DST.WorldModels;
using MCTS.DST;
using MCTS.DST.Resources.Materials;


namespace MCTS.DST.Actions
{

    public class PickUp : ActionDST
    {
        private Dictionary<string, Material> MaterialBase { get; } = MaterialDict.Instance.materialBase;
        public string Target;
        public float Duration;

        public PickUp(string name) : base(name)
        {
            this.Target = name;
            this.Duration = 0.33f;
        }


        private void ExecutePrimitiveBehaviour(WorldModelDST worldState, PrimitiveMaterial targetMaterial)
        {
            worldState.AddToPossessedItems(targetMaterial.Name, targetMaterial.Quantity);
            if (targetMaterial.IsFuel)
            {
                worldState.AddToFuel(targetMaterial.Name, targetMaterial.Quantity);
            }

            // TODO - Add Construct behavior.
        }

        public override void ApplyActionEffects(WorldModelDST worldState)
        {
            worldState.Cycle += this.Duration;
            worldState.UpdateSatiation(-1.0f);
            worldState.Walter.Position = worldState.GetNextPosition(this.Target, "world");

            Material material = this.MaterialBase[this.Target];
            worldState.RemoveFromWorld(this.Target, 1);
            
            if (!material.IsPrimitive)
            { // ComposedMaterial behaviour.
                ComposedMaterial targetMaterial = (ComposedMaterial) material;
                for (int i = 0; i < targetMaterial.ComposingItems.Count; i++)
                {
                    PrimitiveMaterial primitiveComponent = targetMaterial.ComposingItems[i];
                    ExecutePrimitiveBehaviour(worldState, primitiveComponent);
                }
            }
            else
            { // PrimitiveMaterial behaviour.
                PrimitiveMaterial targetMaterial = (PrimitiveMaterial) material;
                ExecutePrimitiveBehaviour(worldState, targetMaterial);
            }


            /*
            worldState.Cycle += this.Duration;
            worldState.UpdateSatiation(-1.0f);
            worldState.Walter.Position = worldState.GetNextPosition(this.Target, "world");
            if (this.Target == "tree")
            {
                worldState.RemoveFromWorld("tree", 1);
                worldState.AddToPossessedItems("log", 1);
                worldState.AddToFuel("log", 1);
                if (worldState.Possesses("rocks", 12))
                {
                    ActionDST action = (ActionDST)new Construct("firepit");
                    worldState.AddAction(action);
                }
                if (!worldState.Possesses("cutgrass", 3))
                    return;
                ActionDST action1 = (ActionDST)new Construct("campfire");
                worldState.AddAction(action1);
            }
            else if (this.Target == "boulder")
            {
                worldState.RemoveFromWorld("boulder", 1);
                worldState.AddToPossessedItems("rocks", 2);
                worldState.AddToPossessedItems("flint", 1);
                if (worldState.Possesses("log", 2) && worldState.Possesses("rocks", 12))
                {
                    ActionDST action = (ActionDST)new Construct("firepit");
                    worldState.AddAction(action);
                }
                if (worldState.Possesses("twigs", 1))
                {
                    ActionDST action = (ActionDST)new Construct("axe");
                    worldState.AddAction(action);
                }
                if (!worldState.Possesses("twigs", 2) || !worldState.Possesses("flint", 2))
                    return;
                ActionDST action1 = (ActionDST)new Construct("pickaxe");
                worldState.AddAction(action1);
            }
            else if (this.Target == "log")
            {
                worldState.RemoveFromWorld("log", 1);
                worldState.AddToPossessedItems("log", 1);
                worldState.AddToFuel("log", 1);
                if (worldState.Possesses("log", 2) && worldState.Possesses("cutgrass", 2))
                {
                    ActionDST action = (ActionDST)new Construct("campfire");
                    worldState.AddAction(action);
                }
                if (!worldState.Possesses("log", 2) || !worldState.Possesses("rocks", 12))
                    return;
                ActionDST action1 = (ActionDST)new Construct("firepit");
                worldState.AddAction(action1);
            }
            else if (this.Target == "twigs")
            {
                worldState.RemoveFromWorld("twigs", 1);
                worldState.AddToPossessedItems("twigs", 1);
                worldState.AddToFuel("twigs", 1);
                if (worldState.Possesses("flint", 1))
                {
                    ActionDST action = (ActionDST)new Construct("axe");
                    worldState.AddAction(action);
                }
                if (!worldState.Possesses("flint", 2) || !worldState.Possesses("twigs", 2))
                    return;
                ActionDST action1 = (ActionDST)new Construct("pickaxe");
                worldState.AddAction(action1);
            }
            else if (this.Target == "sapling")
            {
                worldState.RemoveFromWorld("sapling", 1);
                worldState.AddToPossessedItems("twigs", 1);
                worldState.AddToFuel("twigs", 1);
                if (worldState.Possesses("flint", 1))
                {
                    ActionDST action = (ActionDST)new Construct("axe");
                    worldState.AddAction(action);
                }
                if (!worldState.Possesses("flint", 2) || !worldState.Possesses("twigs", 2))
                    return;
                ActionDST action1 = (ActionDST)new Construct("pickaxe");
                worldState.AddAction(action1);
            }
            else if (this.Target == "flint")
            {
                worldState.RemoveFromWorld("flint", 1);
                worldState.AddToPossessedItems("flint", 1);
                if (worldState.Possesses("twigs", 1))
                {
                    ActionDST action = (ActionDST)new Construct("axe");
                    worldState.AddAction(action);
                }
                if (!worldState.Possesses("twigs", 2) || !worldState.Possesses("flint", 2))
                    return;
                ActionDST action1 = (ActionDST)new Construct("pickaxe");
                worldState.AddAction(action1);
            }
            else if (this.Target == "cutgrass")
            {
                worldState.RemoveFromWorld("cutgrass", 1);
                worldState.AddToPossessedItems("cutgrass", 1);
                worldState.AddToFuel("cutgrass", 1);
                if (worldState.Possesses("cutgrass", 3) && worldState.Possesses("log", 2))
                {
                    ActionDST action = (ActionDST)new Construct("campfire");
                    worldState.AddAction(action);
                }
                if (!worldState.Possesses("cutgrass", 2) || !worldState.Possesses("twigs", 2))
                    return;
                ActionDST action1 = (ActionDST)new Construct("torch");
                worldState.AddAction(action1);
            }
            else if (this.Target == "grass")
            {
                worldState.RemoveFromWorld("grass", 1);
                worldState.AddToPossessedItems("cutgrass", 1);
                worldState.AddToFuel("cutgrass", 1);
                if (worldState.Possesses("cutgrass", 3) && worldState.Possesses("log", 2))
                {
                    ActionDST action = (ActionDST)new Construct("campfire");
                    worldState.AddAction(action);
                }
                if (!worldState.Possesses("cutgrass", 2) || !worldState.Possesses("twigs", 2))
                    return;
                ActionDST action1 = (ActionDST)new Construct("torch");
                worldState.AddAction(action1);
            }
            else if (this.Target == "rocks")
            {
                worldState.RemoveFromWorld("rocks", 1);
                worldState.AddToPossessedItems("rocks", 1);
                if (!worldState.Possesses("log", 2) || !worldState.Possesses("rocks", 12))
                    return;
                ActionDST action = (ActionDST)new Construct("firepit");
                worldState.AddAction(action);
            }
            else if (this.Target == "berrybush")
            {
                worldState.RemoveFromWorld("berrybush", 1);
                worldState.AddToPossessedItems("berries", 2);
                ActionDST action = (ActionDST)new Eat("berries");
                worldState.AddAction(action);
            }
            else if (this.Target == "carrot" || this.Target == "carrot_planted")
            {
                worldState.RemoveFromWorld("carrot", 1);
                worldState.AddToPossessedItems("carrot", 1);
                ActionDST action = (ActionDST)new Eat("carrot");
                worldState.AddAction(action);
            }
            else
            {
                if (!(this.Target == "berries"))
                    return;
                worldState.RemoveFromWorld("berries", 1);
                worldState.AddToPossessedItems("berries", 1);
                ActionDST action = (ActionDST)new Eat("berries");
                worldState.AddAction(action);
            }
            */
        }

        public override List<Pair<string, string>> Decompose(PreWorldState preWorldState)
        {
            return new List<Pair<string, string>>(1)
            {
                new Pair<string, string>("Action(PICKUP, -, -, -, -)", preWorldState.GetInventoryGUID(this.Target).ToString())
            };
        }

        public override Pair<string, int> NextActionInfo()
        {
            return base.NextActionInfo();
        }
    }
}
