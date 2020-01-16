﻿using System;
using KnowledgeBase;
using SerializationUtilities;
using System.Collections.Generic;
using System.IO;
using WellFormedNames;
using Utilities;
using MCTS.DST;
using MCTS.DST.Actions;
using MCTS.DST.WorldModels;
using System.Linq;

namespace MCTS
{
    public class MCTSAsset
    {
        private static readonly Name MCTS_DYNAMIC_PROPERTY_NAME = Name.BuildName("MCTS");
        private KB m_kb;
        private MCTSAlgorithm Mcts;
        private List<Pair<string, string>> ToDoActionsList;
        private Pair<string,int> NextActionInfo;
      
        public MCTSAsset()
        {
            m_kb = null;
            ToDoActionsList = new List<Pair<string, string>>();
            NextActionInfo = new Pair<string, int>("",0);
       
        }

        /// <summary>
        /// Binds a KB to this asset.
        /// </summary>
        /// <param name="kb">The Knowledge Base to be binded to this asset.</param>
        public void RegisterKnowledgeBase(KB kB)
        {
            if (m_kb != null)
            {
                //Unregist bindings
                UnbindToRegistry(m_kb);
                m_kb = null;
            }

            m_kb = kB;
            BindToRegistry(kB);
        }

        private void BindToRegistry(IDynamicPropertiesRegistry registry)
        {
            registry.RegistDynamicProperty(MCTS_DYNAMIC_PROPERTY_NAME, MCTSSearch);
        }

        public void UnbindToRegistry(IDynamicPropertiesRegistry registry)
        {
            registry.UnregistDynamicProperty(MCTS_DYNAMIC_PROPERTY_NAME);
        }

        //This is where the main body of the MCTS Search must be implemented
        private IEnumerable<DynamicPropertyResult> MCTSSearch(IQueryContext context, Name actionVar, Name targetVar)
        {
            Substitution actionSub = null;
            Substitution targetSub = null;

            try
            {
                //How to clone the KB with our JSON serializer
                var jsonSerializer = new JSONSerializer();
                var memStream = new MemoryStream();
                var json = jsonSerializer.SerializeToJson(this.m_kb);
                var kbCloned = jsonSerializer.DeserializeFromJson<KB>(json);

                //Escrever comentário

                if (this.NextActionInfo.Item1 != "")
                {
                    NextAction PriorityAction = new NextAction(NextActionInfo.Item1, kbCloned);

                    if (this.NextActionInfo.Item2 <= 1)
                    {
                        this.NextActionInfo = new Pair<string, int>("", 0);
                    }
                    else
                    {
                        this.NextActionInfo.Item2 -= 1;
                    }

                    Pair<string, string> pairOfPriorityAction = PriorityAction.ConstructNextAction();
                    this.ToDoActionsList.Add(pairOfPriorityAction);
                }

                if (this.ToDoActionsList.Count == 0)
                {
                    PreWorldState preWorldState = new PreWorldState(kbCloned);
                    WorldModelDST worldModel = new WorldModelDST(preWorldState);

                    Console.WriteLine("Available Actions: ");

                    foreach (var action in worldModel.AvailableActions)
                    {
                        Console.WriteLine("     " + action.Name);
                    }
                    Console.WriteLine("");

                    this.Mcts = new MCTSAlgorithm(worldModel);
                    this.Mcts.InitializeMCTSearch();

                    ActionDST MacroAction = this.Mcts.Run();

                    this.ToDoActionsList = MacroAction.Decompose(preWorldState);

                    this.NextActionInfo = MacroAction.NextActionInfo();
                    this.NextActionInfo.Item1 = preWorldState.CompleteNextActionInfo(this.NextActionInfo.Item1);
                }

                Pair<string, string> CurrentAction = this.ToDoActionsList[0];
                this.ToDoActionsList.Remove(CurrentAction);

                Console.WriteLine("Next Action:");
                Console.WriteLine(CurrentAction.Item1 + " " + CurrentAction.Item2);
                Console.WriteLine("");

                actionSub = new Substitution(actionVar, new ComplexValue(Name.BuildName(CurrentAction.Item1)));
                targetSub = new Substitution(targetVar, new ComplexValue(Name.BuildName(CurrentAction.Item2)));
            }
            catch(Exception ex)
            {
                Console.WriteLine("MCTSAsset.cs: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            if (actionSub is null || targetSub is null)
            {
                Console.WriteLine("MCTSAsset.cs - actionSub || targetSub is null");
            }

            foreach (var subSet in context.Constraints)
            {
                subSet.AddSubstitution(actionSub);
                subSet.AddSubstitution(targetSub);

                yield return new DynamicPropertyResult(new ComplexValue(Name.BuildName(true), 1.0f), subSet);
            }

        }

    }
}
