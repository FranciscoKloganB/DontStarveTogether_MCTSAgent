using System;
using MCTS.DST.Actions;
using MCTS.DST.WorldModels;
using System.Collections.Generic;
using System.Linq;

namespace MCTS.DST
{

    public class MCTSAlgorithm
    {
        private const int MAX_SELECTION_DEPTH = 2;
        private const int MAX_PLAYOUT_DEPTH = 2;
        private const int MAX_ITERATIONS_PER_FRAME = 100;
        public const float C = 1.4f;

        public bool InProgress { get; private set; }
        public MCTSNode BestFirstChild { get; set; }

        protected int CurrentIterations { get; set; }
        protected int CurrentDepth { get; set; }

        protected WorldModelDST CurrentState { get; set; }
        public MCTSNode InitialNode { get; set; }
        protected Random RandomGenerator { get; set; }

        public MCTSAlgorithm(WorldModelDST currentState)
        {
            this.InProgress = false;
            this.CurrentState = currentState;
            this.RandomGenerator = new Random();
        }

        public void InitializeMCTSearch()
        {
            this.CurrentIterations = 0;
            this.InitialNode = new MCTSNode(this.CurrentState)
            {
                Action = null,
                Parent = null,
            };
            this.InProgress = true;
            this.BestFirstChild = null;
        }

        public ActionDST Run()
        {
            MCTSNode selectedNode;
            float reward;

            Console.WriteLine("Running MCTS Search");

            while (this.CurrentIterations++ < MAX_ITERATIONS_PER_FRAME)
            {
                selectedNode = Selection(this.InitialNode);
                if (selectedNode == this.InitialNode)
                { // Initial node does not have any children.
                    Console.WriteLine("Initial node does not have any children.");
                    break;
                }
                for (short j = 0; j < MAX_PLAYOUT_DEPTH; j++)
                {
                    reward = Playout(selectedNode.State);
                    Backpropagate(selectedNode, reward);
                }
            }
            
            this.InProgress = false;
            return BestFinalAction(this.InitialNode);
        }

        protected MCTSNode Selection(MCTSNode nodeToDoSelection)
        {
            MCTSNode currentNode = nodeToDoSelection;
            int currentDepth = -1;
            while (++currentDepth < MAX_SELECTION_DEPTH) // !currentNode.State.IsTerminal())
            {
                HashSet<ActionDST> allActions = currentNode.State.GetExecutableActions();
                // If all executable actions have been used to generate child worlds, 
                // the current node cannot be further expanded.
                if (allActions.Count == currentNode.ChildNodes.Count)
                {
                    currentNode = this.BestUCTChild(nodeToDoSelection);
                }
                else
                {
                    // List of all actions that already generated child worlds.
                    List<string> executedActions = new List<string>();
                    for (int i = 0; i < currentNode.ChildNodes.Count; i++)
                    {
                        MCTSNode childNode = currentNode.ChildNodes[i];
                        executedActions.Add(childNode.Action.Name);
                    }

                    // List of available actions = AllActions - ExecutedActions.
                    List<ActionDST> availableActions = new List<ActionDST>();
                    for (int i = 0; i < allActions.Count; i++)
                    {
                        ActionDST action = allActions.ElementAt(i);
                        if (!executedActions.Contains(action.Name))
                        {
                            availableActions.Add(action);
                        }
                    }

                    // There are still available actions to generate child worlds.
                    if (availableActions.Count != 0)
                    {
                        int randomActionIndex = this.RandomGenerator.Next(availableActions.Count);
                        currentNode = Expand(currentNode, availableActions[randomActionIndex]);
                    }
                    return currentNode;
                }
            }
            return currentNode;
        }

        protected MCTSNode Expand(MCTSNode parent, ActionDST action)
        {
            WorldModelDST futureWorld = parent.State.GenerateChildWorldModel();
            action.ApplyActionEffects(futureWorld);
            MCTSNode child = new MCTSNode(futureWorld)
            {
                Action = action,
                Parent = parent,
            };
            parent.ChildNodes.Add(child);
            return child;
        }

        protected float Playout(WorldModelDST initialPlayoutState)
        {
            WorldModelDST futureWorld = initialPlayoutState;
            ActionDST randomChosenAction;
            CurrentDepth = -1;
            while (++this.CurrentDepth < MAX_PLAYOUT_DEPTH)
            {
                HashSet<ActionDST> availableActions = futureWorld.GetExecutableActions();
                int len = availableActions.Count;
                if (len == 0) 
                {
                    break;
                }
                randomChosenAction = availableActions.ElementAt(RandomGenerator.Next(len));
                futureWorld = futureWorld.GenerateChildWorldModel();
                randomChosenAction.ApplyActionEffects(futureWorld);
            }
            return WorldStateHeuristic(futureWorld);
        }

        protected virtual void Backpropagate(MCTSNode node, float reward)
        {
            while (node != null)
            {
                node.N++;
                node.Q += reward;
                node = node.Parent;
            }
        }

        protected virtual MCTSNode BestUCTChild(MCTSNode node)
        {
            MCTSNode bestNode = null;
            if (node.ChildNodes.Count == 0)
            {
                return bestNode;
            }
            bestNode = node.ChildNodes[0];
            MCTSNode child;
            float UCTValue;
            float bestUCT = float.MinValue;
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                child = node.ChildNodes[i];
                UCTValue = (float) ((child.Q / child.N) + 1.4f * Math.Sqrt(Math.Log(node.N) / child.N));
                if (UCTValue > bestUCT)
                {
                    bestUCT = UCTValue;
                    bestNode = node.ChildNodes[i];
                }
            }
            return bestNode;
        }

        protected ActionDST BestFinalAction(MCTSNode node)
        {
            float averageQ;
            float bestAverageQ = float.MinValue;
            MCTSNode bestNode = null;

            int i = -1;
            while (++i < node.ChildNodes.Count)
            {
                averageQ = node.ChildNodes[i].Q / node.ChildNodes[i].N;
                if (averageQ > bestAverageQ)
                {
                    bestAverageQ = averageQ;
                    bestNode = node.ChildNodes[i];
                }
            }
            return bestNode.Action;
        }

        protected float WorldStateHeuristic(WorldModelDST state)
        {
            if (state.Walter.HP <= 0 || state.Walter.Sanity <= 0 || (state.Walter.Satiation <= 0 && state.Walter.HP <= 30))
            {
                return 0.0f;
            }

            float cycleProgress = state.Cycle;
            float duskLength = state.CycleInfo[1];
            float nightLength = state.CycleInfo[2];
            float cycleLength = state.CycleInfo.Sum();

            if (cycleProgress >= cycleLength - nightLength)
            {   // It's night time
                return (float)(state.LightValueNight() * 0.9f + state.FoodValue() * 0.1f) / 10.0f;
            }
            else if (cycleProgress >= cycleLength - nightLength - duskLength)
            {   // It's dusk time
                return (float)(state.LightValueDay() * 7.0f + state.FoodValue() * 3.0f) / 10.0f;
            }
            else
            {   // It's day time
                return (float)(state.LightValueDay() * 2.0f + state.FoodValue() * 8.0f) / 10.0f;
            }
        }
    }
}
