using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayList.Util
{
   public class PlaylistGenerator : IPlaylistGenerator
   {
      private readonly List<ICriteriaCounter<Song>> _criteria;
      public int MaximumSongCount { get; private set; }

      private List<Song> _songList = new List<Song>();
      private int Pivot = 0;

      public PlaylistGenerator()
         : this(GetCriteriaCounters())
      {
      }

      public PlaylistGenerator(IEnumerable<ICriteriaCounter<Song>> criteria)
      {
         _criteria = new List<ICriteriaCounter<Song>>(criteria);
      }

      public IEnumerable<Song> GeneratePlayList(IEnumerable<Song> songProvider, int maximumSongCount, IEnumerable<Song> alwaysAdd)
      {
         MaximumSongCount = maximumSongCount;

         _songList = new List<Song>(songProvider);
         _songList.Shuffle();

         AddSongsBasedOnCriteria(_criteria);
         AddAlwaysAddSongs(alwaysAdd);
         FillInRemainingSongs();

         return _songList.SubsetTo(Pivot - 1);
      }

      private void FillInRemainingSongs()
      {
         while (Pivot < MaximumSongCount)
            Pivot++;
      }

      private void AddSongsBasedOnCriteria(IEnumerable<ICriteriaCounter<Song>> criteria)
      {
         var activeCriteria = criteria.Where(x => x.IsActive).ToArray();

         for (var current = Pivot; current < _songList.Count; current++)
         {
            var matchingFilter = activeCriteria
               .Where(x => x.IsActive)
               .FirstOrDefault(x => x.Criteria(_songList[current]));

            if (matchingFilter != null)
            {
               _songList.Swap(current, ++Pivot);
               matchingFilter.Decrement();
            }

            if (!activeCriteria.Any(x => x.IsActive))
               break;
         }

      }

      private void AddAlwaysAddSongs(IEnumerable<Song> alwaysAdd)
      {
         var songHash = new HashSet<Song>(alwaysAdd);
         for (var i = Pivot; i < _songList.Count; i++)
         {
            if (songHash.Contains(_songList[i]))
               _songList.Swap(i, ++Pivot);

            if (Pivot >= MaximumSongCount)
               break;
         }
      }

      private static IEnumerable<ICriteriaCounter<Song>> GetCriteriaCounters()
      {
         return new List<ICriteriaCounter<Song>>()
            {
                new CriteriaCounter(x => x.Rating.StartsWith("4"), 300),
                new CriteriaCounter(x => x.PrimaryGenre.ContainsCaseInsensitive("Classical"), 250),
                new CriteriaCounter(x => x.PrimaryGenre.ContainsCaseInsensitive("Jazz"), 250),
                new CriteriaCounter(x => x.PrimaryGenre.ContainsCaseInsensitive("World"), 200),
                new CriteriaCounter(x => x.PrimaryGenre.ContainsCaseInsensitive("Country"), 100),
                new CriteriaCounter(x => x.PrimaryGenre.ContainsCaseInsensitive("Hip Hop"), 100),
            };
      }
   }
}
