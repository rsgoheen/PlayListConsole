using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using CsvHelper;

namespace PlayList.Util
{
    /// <summary>
    /// Class for reading metadata on music files in a directory
    /// </summary>
    /// <remarks>
    /// Uses Shell32 for reading file information.  See top of class for constants 
    /// related to where specific fields (titles, artists, etc) are expected in the
    /// Shell32 details.
    /// </remarks>
    public class MusicFileReader
    {
        public const int FILENAME = 0;
        public const int AUTHOR = 20;
        public const int TITLE = 21;
        public const int ALBUM_ARTIST = 217;

        public static IEnumerable<Song> GetSongs(string directory)
        {
            var folders = new Stack<string>();

            folders.Push(directory);

            while (folders.Count > 0)
            {
                var current = folders.Pop();
                foreach (var child in Directory.EnumerateDirectories(current))
                    folders.Push(child);

                foreach (var song in GetSongsInFolder(current))
                    yield return song;
            }
        }

        private static IEnumerable<Song> GetSongsInFolder(string current)
        {
            var shellFolder = new Shell32.Shell().NameSpace(current);

            Func<string, bool> fileFilter =
                name => name.EndsWith("mp3", StringComparison.CurrentCultureIgnoreCase) || name.EndsWith("wma", StringComparison.CurrentCultureIgnoreCase);

            var albumArt = GetAlbumArtFile(current);

            return Directory.EnumerateFiles(current)
                .Where(fileFilter)
                .Select(Path.GetFileName)
                .Select(shellFolder.ParseName)
                .Select(shellFile => new Song
                                            {
                                                Path = Path.Combine(current, shellFolder.GetDetailsOf(shellFile, 0)),
                                                AlbumArtPath = albumArt,
                                                Artist = GetArtist(shellFolder, shellFile),
                                                Title = GetTitle(shellFolder, shellFile),
                                                Album = shellFolder.GetDetailsOf(shellFile, 14),
                                                Size = ExtensionMethods.ToFileSize(shellFolder.GetDetailsOf(shellFile, 1)),
                                                Rating = shellFolder.GetDetailsOf(shellFile, 19),
                                                Genre = shellFolder.GetDetailsOf(shellFile, 16),
                                                PrimaryGenre = GetPrimaryGenre(shellFolder.GetDetailsOf(shellFile, 16)),
                                                Duration = ExtensionMethods.ToTimeSpan(shellFolder.GetDetailsOf(shellFile, 27)),
                                                DateCreated = ExtensionMethods.ToDateTime(shellFolder.GetDetailsOf(shellFile, 4)),
                                            });

        }

        private static string GetArtist(Shell32.Folder shellFolder, Shell32.FolderItem shellFile)
        {
            if (!string.IsNullOrEmpty(shellFolder.GetDetailsOf(shellFile, AUTHOR)))
                return shellFolder.GetDetailsOf(shellFile, AUTHOR);

            if (!string.IsNullOrEmpty(shellFolder.GetDetailsOf(shellFile, ALBUM_ARTIST)))
                return shellFolder.GetDetailsOf(shellFile, ALBUM_ARTIST);

            return "Unknown Artist";
        }

        private static string GetTitle(Shell32.Folder shellFolder, Shell32.FolderItem shellFile)
        {
            if (!string.IsNullOrEmpty(shellFolder.GetDetailsOf(shellFile, TITLE)))
                return shellFolder.GetDetailsOf(shellFile, TITLE);

            // return the filename if there's no title
            return shellFolder.GetDetailsOf(shellFile, FILENAME);
        }

        private static string GetAlbumArtFile(string current)
        {
            return Directory.EnumerateFiles(current)
                .Where(name => System.Text.RegularExpressions.Regex.Match(name, "(AlbumArt_|Folder)").Success)
                .OrderByDescending(name => name) // Prefer "Folder.jpg" over "AlbumArt*.jpg"
                .FirstOrDefault();
        }

        private static Dictionary<string, string> _genreDictionary;
        private static Dictionary<string, string> GenreDictionary {
            get
            {
                if (_genreDictionary == null)
                    _genreDictionary = GetGenreDictionary();
                return _genreDictionary;
            }
        }

        private static string GetPrimaryGenre(string genre)
        {
            if (GenreDictionary.ContainsKey(genre.ToUpper()))
                return GenreDictionary[genre.ToUpper()];

            return genre;
        }

        private static Dictionary<string, string> GetGenreDictionary()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var file = assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains("GenreMapping.csv"));
            var stream = assembly.GetManifestResourceStream(file);
            var dict = new Dictionary<string, string>();

            using (var sr = new StreamReader(stream))
            {
                var parser = new CsvParser(sr);

                while (true)
                {
                    var row = parser.Read();
                    if (row == null)
                        break;

                    dict.Add(row[0].ToUpper(), row[1]);
                }
            }

            return dict;
        }
    }
}