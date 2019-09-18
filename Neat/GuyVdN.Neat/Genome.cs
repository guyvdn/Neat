using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GuyVdN.Neat.Extensions;

namespace GuyVdN.Neat
{
    public class Genome
    {
        private const int firstLayer = 1;
        public int Layers { get; private set; }

        public int NumberOfBiasNodes { get; set; }

        public int NumberOfInputNodes { get; }

        public int NumberOfOutputNodes { get; }

        private readonly InnovationHistory innovationHistory;
        private readonly List<Node> nodes = new List<Node>();
        private readonly List<Connection> connections = new List<Connection>();
        public IEnumerable<Node> Nodes => nodes;

        public IEnumerable<Connection> Connections => connections;

        public Node GetNode(int nodeNumber) => Nodes.SingleOrDefault(n => n.Number == nodeNumber);

        public Genome(int numberOfInputNodes, int numberOfOutputNodes, InnovationHistory innovationHistory, bool isBaby = false)
        {
            NumberOfBiasNodes = 1;
            NumberOfInputNodes = numberOfInputNodes;
            NumberOfOutputNodes = numberOfOutputNodes;
            this.innovationHistory = innovationHistory;

            Guard.GreaterThanZero(() => numberOfInputNodes);
            Guard.GreaterThanZero(() => numberOfOutputNodes);

            Layers = 2;

            if (isBaby)
                return;

            NumberOfBiasNodes.Times(_ => AddNode(firstLayer));
            numberOfInputNodes.Times(_ => AddNode(firstLayer));
            numberOfOutputNodes.Times(_ => AddNode(firstLayer + 1));
        }

        private Node AddNode(int layer)
        {
            var newNode = new Node(NewNodeNumber, layer);
            nodes.Add(newNode);
            return newNode;
        }

        private int NewNodeNumber => nodes.Count;

        public void MutateAddConnection()
        {
            if (IsFullyConnected())
                return;

            var startNodes = nodes.Where(n => n.Layer < Layers).ToArray();

            while (true)
            {
                var from = GetRandom.Instance().Item(startNodes);
                var endNodes = nodes.Where(n => n.Layer > from.Layer).ToArray();
                var to = GetRandom.Instance().Item(endNodes);

                if (from.IsConnectedTo(to))
                    continue;

                AddConnection(from, to);
                return;
            }
        }

        private bool IsFullyConnected()
        {
            var startNodes = nodes.Where(n => n.Layer < Layers);
            return startNodes.All(startNode => startNode.Connections.Count() >= nodes.Count(n => n.Layer > startNode.Layer));
        }

        public void Mutate()
        {
            const double ChanceOfMutatingConnectionWeights = 0.8;
            if (GetRandom.Instance().Bool(ChanceOfMutatingConnectionWeights))
            {
                foreach (var connection in Connections)
                {
                    connection.MutateWeight();
                }
            }

            const double ChanceOfMutatingANewConnection = 0.05;
            if (GetRandom.Instance().Bool(ChanceOfMutatingANewConnection))
            {
                MutateAddConnection();
            }

            const double ChanceOfMutatingANewNode = 0.01;
            if (GetRandom.Instance().Bool(ChanceOfMutatingANewNode))
            {
                MutateAddNode();
            }
        }

        public double[] FeedForward(double[] input)
        {
            if (input.Length != NumberOfInputNodes)
                throw new InvalidOperationException($"Number of inputs expected to be {NumberOfInputNodes} but was {input.Length}");

            var startNodes = nodes.Where(n => n.Layer == firstLayer).ToArray();

            // Bias Node is always 1
            NumberOfBiasNodes.Times(i => startNodes[i].OutputValue = 1);
            NumberOfInputNodes.Times(i => startNodes[i + NumberOfBiasNodes].OutputValue = input[i]);

            EngageNodes();
            var outputs = GetOutputs().ToArray();
            ResetNodes();

            return outputs;
        }

        private void EngageNodes()
        {
            for (var layer = firstLayer; layer <= Layers; layer++)
            {
                foreach (var node in nodes.Where(n => n.Layer == layer))
                {
                    node.Engage();
                }
            }
        }

        private IEnumerable<double> GetOutputs()
        {
            return nodes.Where(n => n.Layer == Layers).Select(n => n.OutputValue);
        }

        private void ResetNodes()
        {
            nodes.ForEach(n => n.Reset());
        }

        public override string ToString()
        {
            var writer = new StringWriter();
            writer.WriteLine($"Number of input nodes: {NumberOfInputNodes}");
            writer.WriteLine($"Number of output nodes: {NumberOfOutputNodes}");
            writer.WriteLine($"Number of Layers: {Layers}");
            writer.WriteLine($"Nodes: {string.Join(", ", nodes.Select(n => n.Number))}");
            writer.WriteLine($"Connections: {Connections.Count()}");

            foreach (var connection in Connections)
            {
                writer.WriteLine($"  Connection {connection.InnovationNr} is {(connection.IsEnabled ? "enabled" : "disabled")}");
                writer.WriteLine($"    from Node {connection.From.Number} to Node {connection.To.Number}");
                writer.WriteLine($"    from Layer {connection.From.Layer} to Layer {connection.To.Layer}");
                writer.WriteLine($"    with a Weight of {connection.Weight}");
            }

            return writer.ToString();
        }

        public Genome Mate(Genome partner)
        {
            var baby = new Genome(NumberOfInputNodes, NumberOfOutputNodes, innovationHistory, true) { Layers = Layers };

            // Copy all nodes (including disjoint and excess) from more fit parent (this)
            Nodes.ForEach(node => baby.AddNode(node.Clone()));

            // Copy all connections
            foreach (var connection in Connections)
            {
                var weight = connection.Weight;
                var isEnabled = connection.IsEnabled;

                // Check if partner also has this connection
                var partnerConnection = partner.GetMatchingConnection(connection);
                if (partnerConnection != null)
                {
                    if (GetRandom.Instance().Bool(0.5))
                    {
                        weight = partnerConnection.Weight;
                    }

                    // Disable when this or partner connection is disabled
                    isEnabled = !(connection.IsDisabled || partnerConnection.IsDisabled && !GetRandom.Instance().Bool(0.75));
                }

                baby.AddConnection(connection.From.Number, connection.To.Number, weight, isEnabled);
            }

            return baby;
        }

        public Genome Clone()
        {
            var baby = new Genome(NumberOfInputNodes, NumberOfOutputNodes, innovationHistory, true) { Layers = Layers };

            // Copy all nodes
            Nodes.ForEach(node => baby.AddNode(node.Clone()));

            // Copy all connections
            foreach (var connection in Connections)
            {
                baby.AddConnection(connection.From.Number, connection.To.Number, connection.Weight, connection.IsEnabled);
            }

            return baby;
        }

        private void AddNode(Node node)
        {
            nodes.Add(node);
        }

        private Connection AddConnection(int fromNumber, int toNumber, double weight, bool isEnabled)
        {
            return AddConnection(GetNode(fromNumber), GetNode(toNumber), weight, isEnabled);
        }

        /// <summary>
        /// Add a connection with a random weight
        /// </summary>
        /// <returns>The new Connection</returns>
        public Connection AddConnection(Node from, Node to, bool isEnabled = true)
        {
            return AddConnection(from, to, GetRandom.Instance().Double(-1, 1), isEnabled);
        }

        public Connection AddConnection(Node from, Node to, double weight, bool isEnabled = true)
        {
            var innovationNr = innovationHistory.GetInnovationNumber(from, to);
            var newConnection = new Connection(@from, to, weight, innovationNr, isEnabled);

            connections.Add(newConnection);
            from.AddConnection(newConnection);

            return newConnection;
        }

        public void MutateAddNode()
        {
            var connectionToMutate = GetRandom.Instance().Item(connections);
            MutateAddNode(connectionToMutate.From, connectionToMutate.To);
        }

        public Node MutateAddNode(Node from, Node to)
        {
            if (!from.IsConnectedTo(to))
                throw new ArgumentException("Cannot add a node between two unconnected nodes");

            if (from.Layer >= to.Layer)
                throw new ArgumentException("From layer should be lower than to layer");

            if (from.Layer + 1 == to.Layer)
                IncreaseLayerOfNodes(to.Layer);

            var newNode = AddNode(from.Layer + 1);

            var oldConnection = connections.Single(c => c.From == from && c.To == to);
            oldConnection.Disable();

            AddConnection(from, newNode, 1);
            AddConnection(newNode, to, oldConnection.Weight);

            return newNode;
        }

        private void IncreaseLayerOfNodes(int layer)
        {
            Layers++;
            nodes.Where(n => n.Layer >= layer).ForEach(n => n.MoveToNextLayer());
        }

        private Connection GetMatchingConnection(Connection connection)
        {
            return Connections.SingleOrDefault(c => c.InnovationNr == connection.InnovationNr);
        }

        public void FullyConnect()
        {
            var startNodes = nodes.Where(n => n.Layer < Layers);
            ////return startNodes.All(startNode => startNode.Connections.Count() >= nodes.Count(n => n.Layer > startNode.Layer));

            foreach (var startNode in startNodes)
            {
                var endNodes = nodes.Where(n => n.Layer > startNode.Layer);
                foreach (var endNode in endNodes)
                {
                    if (!startNode.IsConnectedTo(endNode))
                    {
                        AddConnection(startNode, endNode);
                    }
                }
            }
        }
    }
}
