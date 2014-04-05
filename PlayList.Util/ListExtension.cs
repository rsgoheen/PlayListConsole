using System;
using System.Collections.Generic;

namespace PlayList.Util
{
    public static class ListExtension
    {
        public static void Shuffle<T>(this IList<T> list, Random rand = null)
        {
            if (rand == null)
                rand = new Random();

            for (var i = 1; i < list.Count; i++)
            {
                var swap = rand.Next(0, i + 1);

                var temp = list[swap];
                list[swap] = list[i];
                list[i] = temp;
            }
        }
    }
}