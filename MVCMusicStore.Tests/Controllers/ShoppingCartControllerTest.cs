using System;
using NUnit.Framework;
using MVCMusicStore.Models;
using MVCMusicStore.Controllers;
using MVCMusicStore.Tests.Extensions;
using System.Linq;
using System.Web;
using NSubstitute;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using System.Collections.Generic;

namespace MVCMusicStore.Tests.Controllers
{
    [TestFixture]
    public class ShoppingCartControllerTest
    {
        private MusicStoreEntities dbContect;
        private ApplicationDbContext dbSecContext;
        private ShoppingCartController controller;
        private const string adminEmail = "admin@store.com";
        private const string adminPwd = "admin";
        private const string userEmail = "user@store.com";
        private const string userPwd = "user";
        private Dictionary<string, string> strSessionParams;

        private HttpContextBase context;
        private HttpRequestBase request;

        public ShoppingCartControllerTest()
        {
            dbContect = ContextFactory.GetMusicStoreContext();
            dbSecContext = ContextFactory.GetAppContext(
                adminUserEmail: adminEmail,
                adminPwd: adminPwd,
                customUserEmail: userEmail,
                customPwd: userPwd);
            strSessionParams = new Dictionary<string, string>();
        }

        private string getSessionParam(string key)
        {
            if (!strSessionParams.Keys.Contains(key))
            {
                return null;
            }
            else
            {
                return strSessionParams[key];
            }
        }

        [SetUp]
        public void SetUp()
        {
            controller = new ShoppingCartController(dbContect);
            context = Substitute.For<HttpContextBase>();
            request = Substitute.For<HttpRequestBase>();
            context.Request.Returns(request);
            context.Session[Arg.Any<string>()].Returns(x => getSessionParam((string)x[0]));
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller); 
        }

        [TearDown]
        public void TearDown()
        {
            controller.Dispose();
        }

        [Test]
        public void AddToCart()
        {
            const int albumId = 5;
            var cart = ShoppingCart.GetCart(controller, dbContect);
            int oldCount = cart.GetCount();
            int oldAlbumCount = cart.GetCartItems().Where(ci => ci.AlbumId == albumId).Count();
            controller.AddToCart(albumId);
            Assert.AreEqual(oldCount + 1, cart.GetCount());
            int newAlbumCount = cart.GetCartItems().Where(ci => ci.AlbumId == albumId).Count();
            Assert.AreEqual(oldAlbumCount + 1, newAlbumCount);
        }

    }
}
