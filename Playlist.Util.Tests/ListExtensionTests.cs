using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlayList.Util;

namespace Playlist.Util.Tests
{
    [TestClass]
    public class ListExtensionTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void LastItemShuffled()
        {
            var count = 0;

            var rnd = new Random();

            for (var i = 0; i < 10; i++)
            {
                var list = Enumerable.Range(1, 500).ToList();
                list.Shuffle(rnd);
                
                Debug.WriteLine(string.Join(", ", list.Take(5)));

                if (list.Last() == 500)
                    count++;
            }

            Assert.IsTrue(count != 10, "Last item is not shuffled after 10 attempts");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FirstItemShuffled()
        {
           var count = 0;

           var rnd = new Random();

           for (var i = 0; i < 10; i++)
           {
              var list = Enumerable.Range(1, 500).ToList();
              list.Shuffle(rnd);

              Debug.WriteLine(string.Join(", ", list.Take(5)));

              if (list.First() == 1)
                 count++;
           }

           Assert.IsTrue(count != 10, "First item is not shuffled after 10 attempts");
        }

    }
}
