using System.Web.Mvc;
using NUnit.Framework;
using MVCMusicStore.Controllers;
using MVCMusicStore.Models;
using System.Collections.Generic;
using MVCMusicStore.Tests.Extensions;

namespace MVCMusicStore.Tests.Controllers
{
    [TestFixture]
    public class StoreControllerTest
    {
        private MusicStoreEntities context;
        private StoreController controller;

        public StoreControllerTest()
        {
            context = ContextFactory.GetMusicStoreContext();
        }

        [SetUp]
        public void SetupTest()
        {
            controller = new StoreController(context);
        }

        [TearDown]
        public void TearDownTest()
        {
            controller.Dispose();
        }

        [Test]
        public void TestIndexView()
        {
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(10, (result.Model as List<Genre>).Count);
        }

        [Test]
        public void TestBrowseView()
        {
            var result = controller.Browse("Disco") as ViewResult;
            Assert.IsNotNull(result);
            var genre = result.Model as Genre;
            Assert.AreEqual(3, genre.Albums.Count);
        }

        [Test]
        public void TestDetailsView()
        {
            var result = controller.Details(6) as ViewResult;
            Assert.IsNotNull(result);
            Album album = (result.Model as Album);
            Assert.AreEqual("Carmina Burana", album.Title);
            Assert.AreEqual("Classical", album.Genre.Name);
            Assert.AreEqual("Boston Symphony Orchestra & Seiji Ozawa", album.Artist.Name);
        }

        [Test]
        public void TestGenreMenu()
        {
            var result = controller.GenreMenu() as PartialViewResult;
            Assert.IsNotNull(result);
            var genres = result.Model as List<Genre>;
            Assert.AreEqual(10, genres.Count);
        }
    }
}
