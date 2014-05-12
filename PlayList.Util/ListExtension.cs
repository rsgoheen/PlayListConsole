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
            list.Swap(i, rand.Next(0, i + 1));
      }

      public static void Swap<T>(this IList<T> list, int first, int second)
      {
         if (second < 0 || second > list.Count - 1)
            throw new ArgumentOutOfRangeException("second");
         if (first < 0 || first > list.Count - 1)
            throw new ArgumentOutOfRangeException("first");

         var temp = list[first];
         list[first] = list[second];
         list[second] = temp;
      }
   }
}