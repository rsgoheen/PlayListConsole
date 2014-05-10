using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayList.Util
{
    public class PlaylistGenerator : IPlaylistGenerator
    {
        private readonly List<ICriteriaCounter<Song>> _criteria;
        public int MaximumSongCount { get; private set; }

        public PlaylistGenerator() : this(GetCriteriaCounters())
        {
        }

        public PlaylistGenerator(IEnumerable<ICriteriaCounter<Song>> criteria)
        {
            _criteria = new List<ICriteriaCounter<Song>>(criteria);
        }

        public IEnumerable<Song> GeneratePlayList(IEnumerable<Song> songProvider, int maximumSongCount, IEnumerable<Song> alwaysAdd)
        {
            var _random = new Random();

            MaximumSongCount = maximumSongCount;

            var list = new List<Song>(songProvider);
            list.Shuffle(_random);
            var pivotList = new RanomizedSubset<Song>(list);

            AddAlwaysAddSongs(pivotList, alwaysAdd);
            AddSongsBasedOnCriteria(pivotList, _criteria);
            FillInRemainingSongs(pivotList);

            return pivotList.GetFilteredSet();
        }

        public void AddCriteria(ICriteriaCounter<Song> criteriaCounter)
        {
            _criteria.Add(criteriaCounter);
        }

        private void FillInRemainingSongs(RanomizedSubset<Song> ranomizedSubset)
        {
            while (ranomizedSubset.FilteredItemCount < MaximumSongCount)
            {
                ranomizedSubset.Move(ranomizedSubset.Pivot);
            }

        }

        private void AddSongsBasedOnCriteria(RanomizedSubset<Song> ranomizedSubset, IEnumerable<ICriteriaCounter<Song>> criteria)
        {
            var activeCriteria = criteria.Where(x => x.IsActive).ToArray();
            var current = ranomizedSubset.Pivot;

            while (current < ranomizedSubset.Count)
            {
                var matchingFilter =
                    activeCriteria
                    .Where(x => x.IsActive)
                    .Where(x => x.Criteria(ranomizedSubset[current]))
                    .FirstOrDefault();

                if (matchingFilter != null)
                {
                    ranomizedSubset.Move(current);
                    matchingFilter.Decrement();
                }

                current++;

                if (!activeCriteria.Any(x => x.IsActive))
                    break;
                if (ranomizedSubset.FilteredItemCount >= MaximumSongCount)
                    return;
            }
        }

        private void AddAlwaysAddSongs(RanomizedSubset<Song> ranomizedSubset, IEnumerable<Song> alwaysAdd)
        {
            var songHash = new HashSet<Song>(alwaysAdd);
            for (var i = ranomizedSubset.Pivot; i < ranomizedSubset.Count; i++)
            {
                if (songHash.Contains(ranomizedSubset[i]))
                    ranomizedSubset.Move(i);
                if (ranomizedSubset.FilteredItemCount >= MaximumSongCount)
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
