using Newtonsoft.Json;

namespace GuyVdN.Neat
{
    /// <summary>
    /// A connection between two Nodes
    /// </summary>
    public class Connection
    {
        public Node From { get; }
        public Node To { get; }
        public double Weight { get; private set; }
        public bool IsEnabled { get; private set; }
        public bool IsDisabled => !IsEnabled;
        public int InnovationNr { get; }

        public Connection(Node from, Node to, double weight, int innovationNr, bool isEnabled = true)
        {
            From = from;
            To = to;
            Weight = weight;
            InnovationNr = innovationNr;
            IsEnabled = isEnabled;
        }

        public void MutateWeight()
        {
            const double ChanceOfAssigningNewValue = 0.1;

            if (GetRandom.Instance().Bool(ChanceOfAssigningNewValue))
            {
                Weight = GetRandom.Instance().Double(-1, 1);
                return;
            }

            var newValue = Weight + GetRandom.Instance().Double(-0.01, 0.01);

            if (newValue > 1)
            {
                Weight = 1;
            }
            else if (newValue < -1)
            {
                Weight = -1;
            }
            else
            {
                Weight = newValue;
            }
        }

        public void Disable()
        {
            IsEnabled = false;
        }
    }
}