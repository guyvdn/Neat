using System;
using System.Collections.Generic;
using System.Linq;

namespace GuyVdN.Neat
{
    public interface IGetRandom
    {
        double Double(double maxValue);
        double Double(double minValue, double maxValue);
        bool Bool(double pctChanceOfTrue);
        double Gaussian(int mean = 0, int stdDev = 1);
        int Int(int maxValue);
        T Item<T>(IEnumerable<T> items);
    }

    public class GetRandom : IGetRandom
    {
        private static readonly Random random = new Random();

        private static readonly GetRandom instance = new GetRandom();

        public static Func<IGetRandom> Instance = () => instance;

        public double Double(double maxValue)
        {
            return Double(0, maxValue);
        }

        public double Double(double minValue, double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }

        public bool Bool(double pctChanceOfTrue)
        {
            return random.NextDouble() < pctChanceOfTrue;
        }

        public double Gaussian(int mean = 0, int stdDev = 1)
        {
            // https://stackoverflow.com/questions/218060/random-gaussian-variables
            var u1 = 1.0 - random.NextDouble();
            var u2 = 1.0 - random.NextDouble();
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }

        public int Int(int maxValue)
        {
            return random.Next(maxValue);
        }

        public T Item<T>(IEnumerable<T> items)
        {
            var itemList = items.ToList();
            var index = random.Next(itemList.Count);
            return itemList[index];
        }
    }
}