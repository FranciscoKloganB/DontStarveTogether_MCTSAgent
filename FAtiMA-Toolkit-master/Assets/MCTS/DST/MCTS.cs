using System;
using MCTS.DST.Actions;
using MCTS.DST.WorldModels;
using System.Collections.Generic;

namespace MCTS.DST
{

    public class MCTSAlgorithm
    {
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
            MCTSNode currentNode = nodeToDoSelection;
            while (!currentNode.State.IsTerminal())
            {
                ActionDST nextAction = currentNode.State.GetNextAction();
                if (nextAction != null)
                {
                    return Expand(currentNode, nextAction);
                }

                MCTSNode newNode = BestUCTChild(currentNode);
                if (newNode == null)
                {
                    return currentNode;
                }
                currentNode = newNode;
            }
            return currentNode;
        }

        protected MCTSNode Expand(MCTSNode parent, ActionDST action)
        {
            
            
            
            //TO DO
            
            return new MCTSNode(new WorldModelDST());
        }

        protected float Playout(WorldModelDST initialPlayoutState)
        {

            //TO DO


            return 0.0f;
        }

        protected virtual void Backpropagate(MCTSNode node, float reward)
        {
            //TO DO
        }

        protected virtual MCTSNode BestUCTChild(MCTSNode node)
        {
            float UCTValue;
            float bestUCT = float.MinValue;
            MCTSNode bestNode = null;

            int i = 0;

            while (i < node.ChildNodes.Count)
            {
                UCTValue = (float)((node.ChildNodes[i].Q / node.ChildNodes[i].N) + 1.4f * Math.Sqrt(Math.Log(node.N) / node.ChildNodes[i].N));
                if (UCTValue > bestUCT)
                {
                    bestUCT = UCTValue;
                    bestNode = node.ChildNodes[i];
                }
                i++;
            }
            return bestNode;
        }

        protected ActionDST BestFinalAction(MCTSNode node)
        {
            float averageQ;
            float bestAverageQ = float.MinValue;
            MCTSNode bestNode = null;

            int i = 0;

            while (i < node.ChildNodes.Count)
            {
                averageQ = node.ChildNodes[i].Q / node.ChildNodes[i].N;
                if (averageQ > bestAverageQ)
                {
                    bestAverageQ = averageQ;
                    bestNode = node.ChildNodes[i];
                }
                i++;
            }
            return bestNode.Action;
        }

    
    }
}
