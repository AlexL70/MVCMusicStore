using System.Web.Mvc;
using NUnit.Framework;
using MVCMusicStore.Controllers;
using MVCMusicStore.Models;
using System.Collections.Generic;

namespace MVCMusicStore.Tests.Controllers
{
    [TestFixture]
    public class StoreControllerTest
    {
        [Test]
        public void TestIndexView()
        {
            var controller = new StoreController();
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            //Assert.AreEqual(10, (result.Model as List<Genre>).Count);
        }
    }
}
