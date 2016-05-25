using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCMusicStore.Controllers;
using MVCMusicStore.Models;

namespace MVCMusicStore.Tests.Controllers
{
    [TestClass]
    public class StoreControllerTest
    {
        [TestMethod]
        public void TestIndexView()
        {
            var controller = new StoreController();
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}
