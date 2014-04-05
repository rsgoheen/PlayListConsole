using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlayList.Util;

namespace Playlist.Util.Tests
{
    [TestClass]
    public class UnitTest1
    {
        public static string[] Categories = new[] {"one","two","three","four","five","six"};
        private readonly Random _random = new Random();

        [TestMethod]
        [TestCategory("Performance")]
        public void TestFilterTime()
        {
            var sw = new Stopwatch();

            for (var i = 1000; i < (10 ^ 5); i *= 10)
            {
                var all = new List<Song>(GetRandomSongs(i));
            }
        }

        private IEnumerable<Song> GetRandomSongs(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return GenerateSong(i);
            }
        }

        private Song GenerateSong(int id)
        {
            var idPrefix = (string.Format("({0:D8}) ", id));

            return new Song()
            {
                Path = idPrefix + "Path",
                Title = idPrefix + "Title",
                Artist = idPrefix + "Artist",
                Album = idPrefix + "Album",
                Category = Categories[id % Categories.Length],
            };
            
        }
    }
}
