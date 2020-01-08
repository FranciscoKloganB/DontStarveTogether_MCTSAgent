using System;
using MCTS.DST.Actions;
using MCTS.DST.WorldModels;
using System.Collections.Generic;

namespace MCTS.DST
{

    public class MCTSAlgorithm
    {
        private const int MAX_SELECTION_DEPTH = 2;
        private const int MAX_PLAYOUTS_PER_SEARCH = 5;
        private const int MAX_PLAYOUT_DEPTH = 4;
        private const int MAX_ITERATIONS_PER_FRAME = 100;
        public const float C = 1.4f;

        public bool InProgress { get; private set; }
        public int MaxPlayoutDepthReached { get; private set; }
        public int MaxSelectionDepthReached { get; private set; }
        public MCTSNode BestFirstChild { get; set; }

        protected int CurrentIterations { get; set; }
        protected int CurrentDepth { get; set; }

        protected WorldModelDST CurrentState { get; set; }
        public MCTSNode InitialNode { get; set; }
        protected System.Random RandomGenerator { get; set; }

        public MCTSAlgorithm(WorldModelDST currentState)
        {
            this.InProgress = false;
            this.CurrentState = currentState;
            this.RandomGenerator = new System.Random();
        }

        public void InitializeMCTSearch()
        {
            this.MaxPlayoutDepthReached = 0;
            this.MaxSelectionDepthReached = 0;
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

                for (int i = 0; i < MAX_PLAYOUTS_PER_SEARCH; i++)
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
            Console.WriteLine(nodeToDoSelection.State.GetExecutableActions().Count);
            MCTSNode currentNode = nodeToDoSelection;
            int currentDepth = -1;
            while (++currentDepth < MAX_SELECTION_DEPTH) // !currentNode.State.IsTerminal())
            {
                List<ActionDST> executableActions = nodeToDoSelection.State.GetExecutableActions();
                int len = executableActions.Count;
                if (len == nodeToDoSelection.ChildNodes.Count)
                {
                    nodeToDoSelection = BestUCTChild(nodeToDoSelection);
                }
                else
                {
                    List<string> executedActions = new List<string>();
                    for (int i = 0; i < nodeToDoSelection.ChildNodes.Count; i++)
                    {
                        MCTSNode childNode = nodeToDoSelection.ChildNodes[i];
                        executedActions.Add(childNode.Action.Name);
                    }

                    List<ActionDST> availableActions = new List<ActionDST>();
                    for (int i = 0; i < executableActions.Count; i++)
                    {
                        ActionDST action = executableActions[i];
                        if (!executedActions.Contains(action.Name))
                        {
                            availableActions.Add(action);
                        }
                    }

                    if (availableActions.Count != 0)
                    {
                        int randomActionIndex = this.RandomGenerator.Next(availableActions.Count);
                        currentNode = Expand(nodeToDoSelection, availableActions[randomActionIndex]);
                    }
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
                List<ActionDST> availableActions = futureWorld.GetExecutableActions();
                int len = availableActions.Count;
                if (len == 0) 
                {
                    break;
                }
                randomChosenAction = availableActions[RandomGenerator.Next(len)];
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
                Console.WriteLine("utcvaule = " + UCTValue);
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
            if (state.Walter.HP == 0)
            {
                return 0.0f;
            }
            // TODO - Find better heuristic.
            return 1.0f;
        }
    }
}
