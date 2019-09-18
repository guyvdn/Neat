using System;

namespace GuyVdN.Neat.Extensions
{
    public static class IntExtensions
    {
        public static void Times(this int count, Action<int> action)
        {
            for (var i = 0; i < count; i++)
            {
                action(i);
            }
        }
    }
}