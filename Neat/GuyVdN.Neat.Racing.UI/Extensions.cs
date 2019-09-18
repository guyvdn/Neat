using System;
using System.Windows;

namespace Neat
{
    public static class Extensions
    {
        public static Visibility ToVisibility(this bool value)
        {
            return value ? Visibility.Visible : Visibility.Hidden;
        }

        public static double NextDouble(this Random random, double minValue, double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }

        public static void Times(this int count, Action<int> action)
        {
            for (var i = 0; i < count; i++)
            {
                action(i);
            }
        }

        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            Array.ForEach(array, action);
        }

    }
}