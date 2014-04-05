using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayList.Util;
using PlaylistConsole;

namespace PlayListConsole.SystemTest
{
   class AggregateListTest
   {
      private ConcurrentDictionary<Song,int> count = new ConcurrentDictionary<Song,int>();

      private readonly Song[] _songs = File
             .ReadAllLines(@"F:\Music\Songs.txt")
             .Select(JsonConvert.DeserializeObject<Song>).ToArray();

      public void GenerateLists()
      {
         var tasks = new List<Task>();

         for (int i = 0; i < 300; i++)
         {
            var task = new Task<HashSet<Song>>(() => DoGenerateList(new List<Song>(_songs)));
            task.ContinueWith(AddPlaylistStats);
            tasks.Add(task);
            task.Start();
         }

         Task.WaitAll(tasks.ToArray());
         Console.WriteLine("All Done -- Printing Stats");
         Console.WriteLine("");

         var sum = new ConcurrentDictionary<int, int>();

         using (var file = new System.IO.StreamWriter("F:\\SongCount.txt"))
         foreach (var key in count.OrderByDescending(x => x.Value).Select(x => x.Key))
         {
            Console.WriteLine("{0:d4}, {1}", count[key], key);
            file.WriteLine("{0:d4}, {1}", count[key], key);
            sum.AddOrUpdate(count[key], 1, (k, val) => ++val);
         }

         Console.WriteLine("Stats");
         Console.WriteLine("Total: " + count.Count());
         Console.WriteLine("Total count == 1: " + count.Where(x => x.Value == 1).Count());
         Console.WriteLine("Total count > 1: " + count.Where(x => x.Value > 1).Count());
         Console.WriteLine("Total count > 10: " + count.Where(x => x.Value > 10).Count());
         Console.WriteLine("");

         foreach (var key in sum.OrderByDescending( x => x.Value).Select(x => x.Key))
            Console.WriteLine("{0:d4}, {1}", sum[key], key);

      }

      private void AddPlaylistStats(Task<HashSet<Song>> task)
      {
         foreach (var key in task.Result)
            count.AddOrUpdate(key, 1, (k, val) => ++val);
      }

      public HashSet<Song> DoGenerateList(IEnumerable<Song> songs)
      {
         //var gen = new PlaylistGenerator(songs, 2400);
         //Console.Write(".");
         //return new HashSet<Song>(gen.GeneratePlaylist());
          throw new NotImplementedException();
      }
   }
}
