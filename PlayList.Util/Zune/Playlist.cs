using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace PlayList.Util.Zune
{
   /// <summary>
   /// Utility class for managing Zune format playlists (.zpl files)
   /// </summary>
   public sealed class Playlist
   {
      private readonly string _playlistTitle;
      private readonly XDocument _playlistXDocument;

      public Playlist(string playlistTitle)
      {
         _playlistTitle = playlistTitle;

         var assembly = Assembly.GetExecutingAssembly();
         var playlistFileName = assembly.GetManifestResourceNames()
             .First(x => x.EndsWith("playlist.zpl"));

         using (var stream = assembly.GetManifestResourceStream(playlistFileName))
            _playlistXDocument = XDocument.Load(stream);

         var titleTag = _playlistXDocument.Descendants("title").First();
         titleTag.Value = _playlistTitle;
         FileName = playlistTitle + ".zpl";
      }

      public Playlist(XDocument xDocument, string fileName)
      {
         var titleTag = xDocument.Descendants("title").FirstOrDefault();

         Debug.Assert(titleTag != null, "titleTag != null");

         _playlistTitle = titleTag.Value;
         _playlistXDocument = xDocument;
         FileName = fileName;
      }

      private string FileName { get; set; }

      public static IEnumerable<Song> SongsInPlaylist(string playlist)
      {
         var xDocument = XDocument.Load(playlist);
         var playlistTitle = xDocument.Descendants("title").First().Value;

         foreach (var songTag in xDocument.Descendants("media"))
         {
            Song s;
            try
            {
               s = new Song
                   {
                      Title = (string)songTag.Attribute("trackTitle"),
                      Album = (string)songTag.Attribute("albumTitle"),
                      Artist = (string)songTag.Attribute("trackArtist"),
                      Path = (string)songTag.Attribute("src"),
                      Playlists = new List<string>(1) { playlistTitle },
                   };

               if (s.Path == null)
                  throw new Exception("Song " + s.Title + " has a null file path");
            }
            catch
            {
               // Log the failure
               continue;
            }

            yield return s;
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="newSongs"></param>
      /// <returns>A new playlist (not an updatded version of the current playlist) with the new songs.</returns>
      public Playlist UpdatePlaylistXDoc(IEnumerable<Song> newSongs)
      {
         var newXDoc = new XDocument(XDocument);
         newXDoc.Descendants("media").Remove();
         foreach (var song in newSongs)
            newXDoc.Descendants("seq").First().Add(GetSongXML(song));
         return new Playlist(newXDoc, FileName);
      }

      private XElement GetSongXML(Song song)
      {
         return new XElement("media",
            new XAttribute("src", song.Path),
            new XAttribute("serviceId", ""),
            new XAttribute("albumTitle", song.Album),
            new XAttribute("albumArtist", song.Artist),
            new XAttribute("trackTitle", song.Title),
            new XAttribute("trackArtist", song.Artist),
            new XAttribute("duration", song.Duration)
            );
      }

      public XDocument XDocument { get { return _playlistXDocument; } }

      public static void SaveShellPlaylist(IEnumerable<Song> newSongs, string filePath = @"F:\Music\Zune\Playlists")
      {
         new Playlist("Shell One").UpdatePlaylistXDoc(newSongs).Save(filePath);
      }

      public static void SavePlaylist(IEnumerable<Song> songs, string playlistName,
                                      string filePath = @"F:\Music\Zune\Playlists")
      {
         new Playlist(playlistName).UpdatePlaylistXDoc(songs).Save(filePath);
      }

      public Playlist Save(string filePath)
      {
         if (!Directory.Exists(filePath))
            filePath = "";

         XDocument.Save(Path.Combine(filePath, FileName));
         return this;
      }

      public override int GetHashCode()
      {
         return FileName.ToLower().GetHashCode();
      }

      public override bool Equals(object right)
      {
         if (ReferenceEquals(right, null))
            return false;
         if (ReferenceEquals(this, right))
            return true;

         if (GetType() != right.GetType())
            return false;

         return Equals(right as Playlist);
      }

      public bool Equals(Playlist other)
      {
         return (other.GetHashCode() == GetHashCode());
      }

      public override string ToString()
      {
         return _playlistTitle;
      }
   }
}
