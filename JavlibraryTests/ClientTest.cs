using NUnit.Framework;
using System.Threading.Tasks;
using System.Linq;

namespace Tests
{
    public class Tests
    {
        private Javlibrary.Client client;

        [SetUp]
        public void Setup()
        {
            client = new Javlibrary.Client();
        }

        [Test]
        public async Task TestSearchMany()
        {
            var results = await client.Search("abp");

            Assert.AreEqual(results.Count(), 20);
            Assert.AreEqual(results.ElementAt(5).code, "ABP-006");
            Assert.AreEqual(results.ElementAt(5).url, "https://www.javlibrary.com/en/?v=javlijaqye");
        }

        [Test]
        public async Task TestSearchSingle()
        {
            var results = await client.Search("HND-723");

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.ElementAt(0).code, "HND-723");
            Assert.AreEqual(results.ElementAt(0).url, "https://www.javlibrary.com/en/?v=javli6laqy");
        }

        [Test]
        public async Task TestLoadNormalizeTitle()
        {
            var result = await client.Load("javli6lg24");

            var correct = new Javlibrary.Video(
                id: "javli6lg24",
                code: "STARS-126",
                title: "A Big Ass Pantyhose Woman Who Is Exposed So Much That There Is No Plump",
                actresses: new[] { "Koizumi Hinata" },
                genres: new[] { "Cosplay", "Solowork", "Beautiful Girl", "Huge Butt" },
                studio: "SOD Create"
            );

            Assert.AreEqual(result, correct);
        }

        [Test]
        public async Task TestLoadOneActress()
        {
            var result = await client.Load("javlio354u");

            var correct = new Javlibrary.Video(
                id: "javlio354u",
                code: "ABP-002",
                title: "NEW TOKYO Style 01 Aika Phosphorus",
                actresses: new[] { "Aikarin" },
                genres: new[] { "Handjob", "Solowork", "Facials" },
                studio: "Prestige"
            );

            Assert.AreEqual(result, correct);
        }

        [Test]
        public async Task TestLoadManyActresses()
        {
            var result = await client.Load("javli6bm5q");

            var correct = new Javlibrary.Video(
                id: "javli6bm5q",
                code: "SDDE-592",
                title: "Room Boundaries-If It Were In This Way, I Would Like It!To",
                actresses: new[] { "Kurata Mao", "Mihara Honoka", "Kururigi Aoi" },
                genres: new[] { "Cosplay", "Planning", "Cum", "Hypnosis" },
                studio: "SOD Create"
            );

            Assert.AreEqual(result, correct);
        }

        [Test]
        public async Task TestLoadNoActresses()
        {
            var result = await client.Load("javliarg3u");

            var correct = new Javlibrary.Video(
                id: "javliarg3u",
                code: "IPTD-041",
                title: "Goddesses Of The Speed Of Sound 01 RQ'S Cafe",
                actresses: new string[] { },
                genres: new[] { "Mini Skirt", "Big Tits", "Slender", "Race Queen", "Digital Mosaic" },
                studio: "IDEA POCKET"
            );

            Assert.AreEqual(result, correct);
        }
    }
}