using System;
using System.Collections.Generic;
using System.Linq;

namespace GuyVdN.Neat
{
    public class Node
    {
        private double inputSum;

        //public static readonly Node Bias = new Node(0, 1);

        public double OutputValue { get; set; }

        private readonly IList<Connection> connections = new List<Connection>();

        public IEnumerable<Connection> Connections => connections;

        public int Layer { get; private set; }
        public int Number { get; }
        public int InnovationNumber { get; private set; }

        public Node(int number, int layer)
        {
            Number = number;
            Layer = layer;
        }

        public void AddConnection(Connection connection)
        {
            connections.Add(connection);
        }

        public bool IsConnectedTo(Node to)
        {
            return connections.Any(c => c.To == to);
        }

        public void Engage()
        {
            if (Layer > 1)
            {
                OutputValue = Sigmoid(inputSum);
            }

            foreach (var connection in connections)
            {
                if (connection.IsEnabled)
                {
                    connection.To.AddInput(OutputValue * connection.Weight);
                }
            }
        }

        public static double Sigmoid(double value)
        {
            return 1.0 / (1.0 + Math.Pow(Math.E, -4.9 * value));
        }

        public void AddInput(double value)
        {
            inputSum += value;
        }

        public void Reset()
        {
            inputSum = 0;
        }

        public Node Clone() => new Node(Number, Layer);

        public void MoveToNextLayer()
        {
            Layer += 1;
        }
    }
}