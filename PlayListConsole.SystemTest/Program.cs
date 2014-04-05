using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PlayList.Util;
using PlaylistConsole;

namespace PlayListConsole.SystemTest
{
   class Program
   {
      static void Main()
      {
          var songProvider = File
            .ReadAllLines(@"C:\Users\rgoheen\Documents\Visual Studio 2012\Projects\Projects\PlayListConsole\Songs.txt")
            .Select(JsonConvert.DeserializeObject<Song>);

          var categories = new Dictionary<string, int>();

          foreach (var song in songProvider)
          {
              if (!categories.ContainsKey(song.Category))
                  categories.Add(song.Category,0);

              categories[song.Category]++;
          }

          foreach (var key in categories.Keys.OrderBy(x => x))
          {
              Console.WriteLine("{0:d5} - {1}", categories[key], key);
          }

          //log4net.Config.XmlConfigurator.Configure();
          //TimedTest();
          //Console.ReadLine();
      }

      private static void TimedTest()
      {

         // var songs = File
         //     .ReadAllLines(@"F:\Music\Songs.txt")
         //     .Select(JsonConvert.DeserializeObject<Song>).ToArray();

         //for (int i = 0; i < 20; i++)
         //{
         //   var gen = new PlaylistGenerator(songs, 2400);
         //   gen.GeneratePlaylist();
         //}
      }
   }
}
