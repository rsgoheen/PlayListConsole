using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PlayList.Util;
using PlayList.Util.Zune;

namespace PlaylistConsole
{
   class PlaylistConsole
   {
      public void DoCommand(string[] args)
      {
         var songFile = System.Configuration.ConfigurationManager.AppSettings["SongFile"];
         var musicDirectory = System.Configuration.ConfigurationManager.AppSettings["MusicLibraryPath"];

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
         return args.Any( x => x.StartsWithCaseInsensitive("-r"));
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

          var addAllsongs = Playlist.SongsInPlaylist(@"F:\Music\Zune\Playlists\Always Add.zpl");

          var songs = new ShuffleFirstPlaylistGenerator().GeneratePlayList(
              songProvider,
              2400,
              addAllsongs);

         //var songs = new PlaylistGenerator().GeneratePlayList(
         //     songProvider, 
         //     2400,
         //     addAllsongs);

         //var plg = new PlaylistGenerator(Path.Combine(musicDirectory, songFile)).GeneratePlaylist();
         Playlist.SaveShellPlaylist(songs);
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
