using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayList.Util
{
    public class ShuffleFirstPlaylistGenerator : IPlaylistGenerator
    {
        private readonly List<ICriteriaCounter<Song>> _criteria;
        public int MaximumSongCount { get; private set; }

        public ShuffleFirstPlaylistGenerator() : this(GetCriteriaCounters())
        {
        }

        public ShuffleFirstPlaylistGenerator(IEnumerable<ICriteriaCounter<Song>> criteria)
        {
            _criteria = new List<ICriteriaCounter<Song>>(criteria);
        }

        public IEnumerable<Song> GeneratePlayList(IEnumerable<Song> songProvider, int maximumSongCount, IEnumerable<Song> alwaysAdd)
        {
            var _random = new Random();

            MaximumSongCount = maximumSongCount;

            var list = new List<Song>(songProvider);
            list.Shuffle(_random);
            var pivotList = new PivotList<Song>(list);

            AddAlwaysAddSongs(pivotList, alwaysAdd);
            AddSongsBasedOnCriteria(pivotList, _criteria);
            FillInRemainingSongs(pivotList);

            return pivotList.GetFilteredSet();
        }

        public void AddCriteria(ICriteriaCounter<Song> criteriaCounter)
        {
            _criteria.Add(criteriaCounter);
        }

        private void FillInRemainingSongs(PivotList<Song> pivotList)
        {
            while (pivotList.FilteredItemCount < MaximumSongCount)
            {
                pivotList.Move(pivotList.Pivot);
            }

        }

        private void AddSongsBasedOnCriteria(PivotList<Song> pivotList, IEnumerable<ICriteriaCounter<Song>> criteria)
        {
            var activeCriteria = criteria.Where(x => x.IsActive).ToArray();
            var current = pivotList.Pivot;

            while (current < pivotList.Count)
            {
                var matchingFilter =
                    activeCriteria
                    .Where(x => x.IsActive)
                    .Where(x => x.Criteria(pivotList[current]))
                    .FirstOrDefault();

                if (matchingFilter != null)
                {
                    pivotList.Move(current);
                    matchingFilter.Decrement();
                }

                current++;

                if (!activeCriteria.Any(x => x.IsActive))
                    break;
                if (pivotList.FilteredItemCount >= MaximumSongCount)
                    return;
            }
        }

        private void AddAlwaysAddSongs(PivotList<Song> pivotList, IEnumerable<Song> alwaysAdd)
        {
            var songHash = new HashSet<Song>(alwaysAdd);
            for (var i = pivotList.Pivot; i < pivotList.Count; i++)
            {
                if (songHash.Contains(pivotList[i]))
                    pivotList.Move(i);
                if (pivotList.FilteredItemCount >= MaximumSongCount)
                    return;
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
