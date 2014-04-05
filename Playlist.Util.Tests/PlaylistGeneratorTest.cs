using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlayList.Util;

namespace Playlist.Util.Tests
{
    [TestClass]
    public class PlaylistGeneratorTest
    {

        [TestMethod]
        [TestCategory("Unit")]
        public void TestAlwaysAdd()
        {
            var alwaysAdd = new List<Song>(GetRandomSongs(250));
            var all = new List<Song>(GetRandomSongs(7500));

            all.AddRange(alwaysAdd);

            var songs = new ShuffleFirstPlaylistGenerator(new List<ICriteriaCounter<Song>>()).GeneratePlayList(all, alwaysAdd.Count, alwaysAdd);
            CollectionAssert.AreEquivalent(songs.ToArray(), alwaysAdd);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void TestCriteriaFilter()
        {
            var firstCat = "one";

            var firstCriteria = new List<Song>(GetRandomSongs(10));
            firstCriteria.ForEach(x => x.Category = firstCat);

            var all = new List<Song>(GetRandomSongs(75000));
            all.AddRange(firstCriteria);

            var criteria = new List<ICriteriaCounter<Song>>() { new CriteriaCounter(song => song.Category == firstCat, firstCriteria.Count() + 100) };

            var songs = new ShuffleFirstPlaylistGenerator(criteria).GeneratePlayList(all, firstCriteria.Count, new List<Song>());

            CollectionAssert.AreEquivalent(songs.ToArray(), firstCriteria);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void TestMultipleCriteriaFilter()
        {
            var firstCat = "one";
            var secondCat = "two";
            var thirdCat = "three";

            var firstCriteria = new List<Song>(GetRandomSongs(10));
            firstCriteria.ForEach(x => x.Category = firstCat);
            var secondCriteria = new List<Song>(GetRandomSongs(50));
            secondCriteria.ForEach(x => x.Category = secondCat);
            var thirdCriteria = new List<Song>(GetRandomSongs(150));
            thirdCriteria.ForEach(x => x.Category = thirdCat);

            var criteriaList = new List<Song>();
            criteriaList.AddRange(firstCriteria);
            criteriaList.AddRange(secondCriteria);
            criteriaList.AddRange(thirdCriteria);

            var all = new List<Song>(GetRandomSongs(75000));
            all.AddRange(criteriaList);

            var criteria = new List<ICriteriaCounter<Song>>()
            {
                new CriteriaCounter(song => song.Category == firstCat, firstCriteria.Count()),
                new CriteriaCounter(song => song.Category == secondCat, secondCriteria.Count()),
                new CriteriaCounter(song => song.Category == thirdCat, thirdCriteria.Count()),
            };

            var songs = new ShuffleFirstPlaylistGenerator(criteria).GeneratePlayList(all, criteriaList.Count, new List<Song>());

            CollectionAssert.AreEqual(
                songs.OrderBy(x => x.GetHashCode()).ToArray(), 
                criteriaList.OrderBy(x => x.GetHashCode()).ToArray());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FillsInWithSongsWhenNotEnoughSpecified()
        {
            var all = new List<Song>(GetRandomSongs(75000));
            var songs = new ShuffleFirstPlaylistGenerator(new List<ICriteriaCounter<Song>>())
                .GeneratePlayList(all, 100, all.Take(50));

            Assert.AreEqual(songs.Count(), 100);
        }

        private IEnumerable<Song> GetRandomSongs(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return GenerateSong(i);
            }
        }

        private static Random _rand = new Random();
        private Song GenerateSong(int id)
        {
            var words = RandomText.Split(new char[] {' ', '.', ','});
            var sb = new StringBuilder();
            sb.Append(string.Format("({0:D8})",id));
            var len = _rand.Next(2, 5);
            for (var i = 0; i < len; i++)
                sb.Append(words[_rand.Next(0, words.Length - 1)] + " ");

            return new Song()
            {
                Path = sb.ToString(),
                Title = sb.ToString(),
                Artist = sb.ToString(),
                Album = sb.ToString(),
                Category = "xxxx",
            };
        }

        private static string RandomText = @"Because labor markets in science and engineering differ greatly across fields, industries, and time periods, it is easy to cherry-pick specific specialties that really are in short supply, at least in specific years and locations. But generalizing from these cases to the whole of U.S. science and engineering is perilous. Employment in small but expanding areas of information technology such as social media may be booming, while other larger occupations languish or are increasingly moved offshore. It is true that high-skilled professional occupations almost always experience unemployment rates far lower than those for the rest of the U.S. workforce, but unemployment among scientists and engineers is higher than in other professions such as physicians, dentists, lawyers, and registered nurses, and surprisingly high unemployment rates prevail for recent graduates even in fields Labor markets for scientists and engineers also differ geographically. Employer demand is far higher in a few hothouse metropolitan areas than in the rest of the country, especially during boom periods. Moreover recruitment of domestic professionals to these regions may be more difficult than in others when would-be hires discover that the remuneration employers are offering does not come close to compensating for far higher housing and other costs. According to the most recent data from the National Association of Realtors, Silicon Valley  magical religious texts Egyptians all periods contain spells intended   be used against serpents, scorpions, noxious reptiles all kinds, their number, importance which  attached   them, suggest that Egypt must always have produced these pests abundance, that Egyptians were always horribly afraid them. text Unas, which  written towards close Vth Dynasty, contains many such spells, Theban Saite Books Dead several Chapters consist nothing but spells incantations, many which are based on archaic texts, against crocodiles, serpents, other deadly reptiles, insects all kinds. All such creatures were regarded as incarnations evil spirits, which attack dead as well as living, therefore  necessary for well-being former that copies spells against them should be written upon walls tombs, coffins, funerary amulets, etc. gods were just as open   attacks venomous reptiles as man, Ra, himself, king gods, nearly died from poison a snake-bite. Now gods were, as a rule, able   defend themselves against attacks Set his fiends, poisonous snakes insects which were their emissaries, by virtue fluid life, which  peculiar attribute divinity, efforts Egyptians were directed   acquisition a portion this magical power, which would protect their souls bodies their houses cattle, other property, each day each night throughout year. When a man cared for protection himself only he wore an amulet some kind, which fluid life  localized. When he wished   protect his house against invasion by venomous reptiles he placed statues containing fluid life niches walls various chambers, or some place outside but near house, or buried them earth with their faces turned direction from which he expected attack come.";
    }
}
