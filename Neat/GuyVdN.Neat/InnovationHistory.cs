using System;
using System.Collections.Generic;

namespace GuyVdN.Neat
{
    public class InnovationHistory
    {
        private int currentInnovationNumber = 1;
        private readonly Dictionary<Tuple<int, int>, int> history = new Dictionary<Tuple<int, int>, int>();

        public int GetInnovationNumber(Node from, Node to)
        {
            var key = new Tuple<int, int>(from.Number, to.Number);

            if (history.ContainsKey(key))
                return history[key];

            history.Add(key, currentInnovationNumber);
            return currentInnovationNumber++;
        }
    }
}