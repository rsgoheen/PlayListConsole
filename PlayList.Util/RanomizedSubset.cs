using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayList.Util
{
   /// <summary>
   /// Generates a random subset from a given set of items
   /// </summary>
   public class RanomizedSubset<T> where T : IEquatable<T>
   {
      private readonly IList<T> _list;
      private readonly Random _rnd;

      public RanomizedSubset(IEnumerable<T> set, Random random = null)
      {
         _list = new List<T>(set);
         Pivot = 0;
         _rnd = random ?? new Random();
         _list.Shuffle(random ?? new Random());
      }

      public int Pivot { get; private set; }

      /// <summary>
      /// Returns a new set that represents the portion of the list that has been 
      /// moved before the pivot point
      /// </summary>
      /// <returns>An enumerable set of items, or an empty set if there are no items before
      /// the pivot</returns>
      public IEnumerable<T> GetFilteredSet()
      {
         var subList = new T[Pivot];

         Array.Copy(_list.ToArray(),
             0,
             subList,
             0,
             Pivot);

         return subList;
      }

      public int FilteredItemCount
      {
         get { return Pivot; }
      }

      public T this[int index] { get { return _list[index]; } }

      public int Count { get { return _list.Count; } }

      /// <summary>
      /// Move a items that matches the given criteria
      /// </summary>
      /// <param name="criteria">A function that returns true for all items that
      /// should be considered for moving to the filtered section</param>
      /// <param name="count">the number of items to move</param>
      /// <returns></returns>
      public int MoveRandom(Func<T, bool> criteria, int count = 1)
      {
         var availableItemIds = _list
                                 .Where(criteria)
                                 .Select(item => _list.IndexOf(item))
                                 .Where(position => position >= Pivot)
                                 .Take(count)
                                 .ToList();

         foreach (var id in availableItemIds)
         {
            Swap(id, Pivot);
            Pivot++;
         }

         return availableItemIds.Count;
      }

      /// <summary>
      /// Move an item at a particular location into the filtered portion of the set
      /// </summary>
      /// <param name="index">The index of the item to move</param>
      /// <returns>Returns true if the item is moved, false if it is not 
      /// (for example, if the item is already within the filtered set)</returns>
      public bool Move(int index)
      {
         if (index < 0 || index >= _list.Count)
            throw new IndexOutOfRangeException();

         if (index < Pivot)
            return false;

         Swap(index, Pivot);
         Pivot++;

         return true;
      }

      public bool Move(T item)
      {
         var index = _list.IndexOf(item);
         return Move(index);
      }

      /// <summary>
      /// Move all items matching the given criteria into the filtered section
      /// </summary>
      /// <param name="criteria">A function that returns true for all items that
      /// should be considered for moving to the filtered section</param>
      /// <returns>True if one or more items are moved, false if zero items are moved</returns>
      public bool MoveAll(Func<T, bool> criteria)
      {
         var itemIds = _list
             .Where(criteria)
             .Select(item => _list.IndexOf(item))
             .Where(id => id >= Pivot)
             .ToArray();

         if (itemIds.Length == 0)
            return false;

         foreach (var id in itemIds)
         {
            Swap(id, Pivot);
            Pivot++;
         }

         return true;
      }

      /// <summary>
      /// Returns a random item from the non-filtered section that matches the criteria
      /// </summary>
      /// <param name="item">An item, if found, matching the criteria</param>
      /// <param name="criteria">The criteria to use when searching for an item</param>
      /// <returns>true if a matching items is found, false otherwise</returns>
      public bool TryGetRandomNonFiltered(out T item, Func<T, bool> criteria)
      {
         item = default(T);
         var subSet = _list.Skip(Pivot).Where(criteria).ToArray();

         var subSetLength = subSet.Count();
         if (subSetLength == 0)
            return false;

         item = subSet.Skip(_rnd.Next(subSetLength)).First();
         return true;
      }

      private void Swap(int first, int second)
      {
         if (first == second)
            return;

         T temp = _list[first];
         _list[first] = _list[second];
         _list[second] = temp;
      }
   }
}
