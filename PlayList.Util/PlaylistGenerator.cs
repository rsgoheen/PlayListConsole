/* TODO:
 * Additional logging (what albums have been added, etc).
 * Have songs more likely to make list
 * Excluded songs (eg Xmas songs)
 * Song genre synonyms
 * 
 * 
 * UI to show songs, albums
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PlayList.Util.Zune;

namespace PlayList.Util
{
    public class PlaylistGenerator : IPlaylistGenerator
    {
        public PlaylistGenerator(){}

        public PlaylistGenerator(string songFilePath, 
            string alwaysAddPlaylist = @"F:\Music\Zune\Playlists\Always Add.zpl", 
            int totalSongCount = 2400)
        {
            SongProvider = File
                .ReadAllLines(songFilePath)
                .Select(JsonConvert.DeserializeObject<Song>);

            TotalSongCount = totalSongCount;
            AlwaysAddPlaylist = alwaysAddPlaylist;
        }

        public PlaylistGenerator(IEnumerable<Song> songProvider, int totalSongCount)
        {
            SongProvider = songProvider;
            TotalSongCount = totalSongCount;
        }

        private IEnumerable<Song> SongProvider { get; set; }
        public string AlwaysAddPlaylist { get; set; }
        private IEnumerable<Song> AlwaysAddSongs { get; set; } 

        public int TotalSongCount { get; set; }
        public string PlaylistToInclude { get; set; }

        private readonly Func<Song, bool> FavoriteSong = (x => x.Rating.StartsWith("4"));
        private readonly Func<Song, bool> ClassicalSong = (x => x.Genre.ContainsCaseInsensitive("Classical"));
        private readonly Func<Song, bool> JazzSong = (x => ExtensionMethods.ContainsCaseInsensitive(x.Genre, "Jazz"));
        private readonly Func<Song, bool> WorldSong = (x => ExtensionMethods.ContainsCaseInsensitive(x.Genre, "World"));
        private readonly Func<Song, bool> CountrySong = (x => ExtensionMethods.ContainsCaseInsensitive(x.Genre, "Country"));
        private readonly Func<Song, bool> HipHopSong = (x => ExtensionMethods.ContainsCaseInsensitive(x.Genre, "Hip Hop"));

        public IEnumerable<Song> GeneratePlayList(IEnumerable<Song> songProvider, int maximumSongCount, IEnumerable<Song> alwaysAdd = null)
        {
            SongProvider = songProvider;
            TotalSongCount = maximumSongCount;
            AlwaysAddSongs = alwaysAdd;

            throw new NotImplementedException();

        }

        public IEnumerable<Song> GeneratePlaylist()
        {
            var sw = new System.Diagnostics.Stopwatch();
            var list = new PivotList<Song>(SongProvider);

            sw.Start();
            AddAlwaysAddSongs(list);
            Log(sw.ElapsedMilliseconds, "Always Add");

            AddAlbums(list, FavoriteSong, 7);
            //AddAlbums(list, ClassicalSong, 6);
            AddAlbums(list, JazzSong, 6);
            Log(sw.ElapsedMilliseconds, "Albums");

            AddSongs(list, FavoriteSong, 300);
            AddSongs(list, ClassicalSong, 250);
            AddSongs(list, JazzSong, 150);
            AddSongs(list, WorldSong, 25);
            AddSongs(list, CountrySong, 25);
            AddSongs(list, HipHopSong, 50);
            Log(sw.ElapsedMilliseconds, "Songs");

            FillRestOfList(list, TotalSongCount);
            Log(sw.ElapsedMilliseconds, "Rest");

            return list.GetFilteredSet();
        }

        private void Log(long ms, string msg)
        {
            //log.Debug(string.Format("{0:d8} -- {1}", ms, msg));
        }

        private void AddAlwaysAddSongs(PivotList<Song> list)
        {
            GetAlwaysAddSongList().ToList().ForEach(x => list.Move(x));
        }

        private static void FillRestOfList(PivotList<Song> pivotList, int totalSongs)
        {
            var count = totalSongs - pivotList.GetFilteredSet().Count();
            while (count > 0)
            {
                if (!pivotList.MoveRandom())
                    break;

                count--;
            }
        }

        private static void AddSongs(PivotList<Song> pivotList, Func<Song, bool> criteria, int count)
        {
            while (count > 0)
            {
                if (!pivotList.MoveRandom(criteria))
                    break;

                count--;
            }
        }

        private static void AddAlbums(PivotList<Song> pivotList, Func<Song, bool> criteria, int albumCount)
        {
            Song song;
            for (int i = 0; i <= albumCount; i++)
                if (pivotList.TryGetRandomNonFiltered(out song, criteria))
                    pivotList.MoveAll(s => s.Album == song.Album);
        }

        private IEnumerable<Song> GetAlwaysAddSongList()
        {
            return Playlist.SongsInPlaylist(AlwaysAddPlaylist);
        }

        //private static readonly ILog log = LogManager.GetLogger(typeof(PlaylistGenerator));
    }
}
