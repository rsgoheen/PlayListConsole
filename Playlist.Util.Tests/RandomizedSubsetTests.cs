using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlayList.Util;

namespace Playlist.Util.Tests
{
    [TestClass]
    public class RandomizedSubsetTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void TestMoveItem()
        {
            var list = new RanomizedSubset<int>(Enumerable.Range(1, 10));
            Assert.AreEqual(list.GetFilteredSet().Count(), 0);

            var item = list[4];
            list.Move(4);

            CollectionAssert.Contains(list.GetFilteredSet().ToArray(), item);
            Assert.AreEqual(list.GetFilteredSet().Count(), 1);
        }
    }
}
