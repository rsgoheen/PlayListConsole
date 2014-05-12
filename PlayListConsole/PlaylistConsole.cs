using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PlayList.Util;
using PlayList.Util.Zune;

namespace PlaylistConsole
{
   class PlaylistConsole
   {
      private int _totalSongCount;

      public void DoCommand(string[] args)
      {
         var songFile = System.Configuration.ConfigurationManager.AppSettings["SongFile"];
         var musicDirectory = System.Configuration.ConfigurationManager.AppSettings["MusicLibraryPath"];
         if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TotalSongCount"], out _totalSongCount))
            _totalSongCount = 2400;

         if (ShouldGenerateSongCatalog(args))
            GenerateSongListFromFilesystem(musicDirectory, songFile);

         if (ShouldGeneratePlaylist(args))
            GeneratePlaylist(musicDirectory, songFile);

         if (ShowHelp(args))
            PrintHelp();
      }

      private bool ShowHelp(string[] args)
      {
         return args.Any(x => x.StartsWithCaseInsensitive("-h"));
      }

      private bool ShouldGeneratePlaylist(string[] args)
      {
         return args.Any(x => x.StartsWithCaseInsensitive("-g")) || (args.Length == 0);
      }

      private bool ShouldGenerateSongCatalog(string[] args)
      {
         return args.Any(x => x.StartsWithCaseInsensitive("-r"));
      }

      private void GenerateSongListFromFilesystem(string musicDirectory, string songFile)
      {
         var songs = MusicFileReader.GetSongs(musicDirectory);
         File.WriteAllLines(
             Path.Combine(musicDirectory, songFile),
             songs.Select(JsonConvert.SerializeObject));
      }

      private void GeneratePlaylist(string musicDirectory, string songFile)
      {
         var songProvider = File
             .ReadAllLines(Path.Combine(musicDirectory, songFile))
             .Select(JsonConvert.DeserializeObject<Song>);

         songProvider = FilterSongs(songProvider);

         var addAllsongs = Playlist.SongsInPlaylist(@"F:\Music\Zune\Playlists\Always Add.zpl");

         var songs = new PlaylistGenerator().GeneratePlayList(
             songProvider,
             _totalSongCount,
             addAllsongs)
             .ToArray();

         Func<Song, bool> isJazz = x => string.Equals(x.PrimaryGenre, "Jazz", StringComparison.OrdinalIgnoreCase);
         Func<Song, bool> isClassical = x => string.Equals(x.PrimaryGenre, "Classical", StringComparison.OrdinalIgnoreCase);

         Playlist.SavePlaylist(
            songs.Where(x => ! isJazz(x) && ! isClassical(x)),
            "My Everything Else");
         Playlist.SavePlaylist(songs.Where(isClassical), 
            "My Classical");
         Playlist.SavePlaylist(songs.Where(isJazz),
            "My Jazz");
      }

      private IEnumerable<Song> FilterSongs(IEnumerable<Song> songProvider)
      {
         return songProvider
            .Where(x => ! string.Equals(x.PrimaryGenre, "Holiday", StringComparison.OrdinalIgnoreCase));
      }

      private static void PrintHelp()
      {
         Console.WriteLine(
@"Generate a Zune playlist from the songs in the library.

   -g : Generate a playlist.  Add number of songs following flag (e.g. -g 2000)
   -r : Refresh the song data for generating the playlist (from the songs in the  music directory)
   -h : Show help
"
            );
      }

   }
}
