using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PlayList.Util;
using PlayList.Util.Zune;

namespace PlaylistConsole
{
    class Program
    {
        /* TODO:
         * - Move files to util 
         * - SpecFlow
         * - Different playlists for different genres
         * - UI
         *  - Visualize playlist better
         *  - Display music library
         * 
         * Questions:
         * - Can the Windows file system be "queried" to get only those files that have changed?
         * 
         */

        static void Main(string[] args)
        {
            var songProvider = File
              .ReadAllLines(@"C:\Users\rgoheen\Documents\Visual Studio 2012\Projects\Projects\PlayListConsole\Songs.txt")
              .Select(JsonConvert.DeserializeObject<Song>);

            var genres = new Dictionary<string, int>();

            foreach (var song in songProvider)
            {
                string genre = song.Genre ?? "undefined";

                if (!genres.ContainsKey(genre))
                    genres.Add(genre, 0);

                genres[genre]++;
            }

            foreach (var key in genres.Keys.OrderBy(x => x))
            {
                Console.WriteLine("\"{0}\",",key);
                //Console.WriteLine("{0:d5} - {1}", genres[key], key);
            }

            
            
            //new PlaylistConsole().DoCommand(args);
        }
    }
}
