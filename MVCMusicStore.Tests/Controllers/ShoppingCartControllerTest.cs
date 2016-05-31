using NUnit.Framework;
using MVCMusicStore.Models;
using MVCMusicStore.Controllers;
using MVCMusicStore.Tests.Extensions;
using System.Linq;
using System.Web;
using NSubstitute;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;
using System;
using MVCMusicStore.ViewModels;

namespace MVCMusicStore.Tests.Controllers
{
    [TestFixture, Category(nameof(ShoppingCart))]
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
        private ShoppingCart cart;
        private Func<int, Cart> cartExpr;

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
            cart = ShoppingCart.GetCart(controller, dbContect);
            cartExpr = (albId) => cart.GetCartItems().Where(ci => ci.AlbumId == albId).SingleOrDefault();
        }

        [TearDown]
        public void TearDown()
        {
            controller.Dispose();
        }

        [Test]
        public void AddToCartTest()
        {
            const int albumId = 5;
            int oldCount = cart.GetCount();
            int oldAlbumCount = cartExpr(albumId)?.Count ?? 0;
            controller.AddToCart(albumId);
            Assert.AreEqual(oldCount + 1, cart.GetCount());
            int newAlbumCount = cartExpr(albumId)?.Count ?? 0;
            Assert.AreEqual(oldAlbumCount + 1, newAlbumCount);
            var v = controller.Index();
            Assert.AreEqual(oldAlbumCount + 1, ((v as ViewResult).Model as ShoppingCartViewModel).CartItems.Count);
        }

        [Test]
        public void RemoveFromCartTest()
        {
            int albumId = 10;
            var cart = ShoppingCart.GetCart(controller, dbContect);
            int oldAlbumCount = cartExpr(albumId)?.Count ?? 0;
            controller.AddToCart(albumId);
            controller.AddToCart(albumId);
            controller.AddToCart(albumId + 10);  // Add extra album to check filters work
            Assert.AreEqual(oldAlbumCount + 2, cartExpr(albumId)?.Count ?? 0);
            var v = controller.CartSummary() as PartialViewResult;
            Assert.AreEqual(oldAlbumCount + 3, (int) v.ViewData[$"{nameof(Cart)}{nameof(Cart.Count)}"]);
            int recordId = cartExpr(albumId)?.RecordId ?? 0;
            controller.RemoveFromCart(recordId);
            Assert.AreEqual(oldAlbumCount + 1, cartExpr(albumId)?.Count ?? 0);
            while(cartExpr(albumId)?.Count > 0)
            {
                controller.RemoveFromCart(recordId);
            }
            Assert.AreEqual(0, cartExpr(albumId)?.Count ?? 0);
            //  Remove extra album to keep DB clear
            controller.RemoveFromCart(cartExpr(albumId + 10)?.RecordId ?? 0);
        }

    }
}
