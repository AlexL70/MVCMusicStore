using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NUnit.Framework;
using MVCMusicStore;
using MVCMusicStore.Controllers;
using MVCMusicStore.Models;
using MVCMusicStore.Tests.Extensions;

namespace MVCMusicStore.Tests.Controllers
{
    [TestFixture, Category(nameof(HomeController))]
    public class HomeControllerTest
    {
        private MusicStoreEntities context;
        private HomeController controller;

        public HomeControllerTest()
        {
            context = ContextFactory.GetMusicStoreContext();
        }

        [SetUp]
        public void SetUpTest()
        {
            controller = new HomeController(context);
        }

        [TearDown]
        public void TearDownTest()
        {
            controller.Dispose();
        }

        [Test]
        public void Index()
        {
            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            //Assert.Equals("Index", result.ViewName);
        }

        [Test]
        public void About()
        {
            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [Test]
        public void Contact()
        {
            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
