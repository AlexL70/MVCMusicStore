using NUnit.Framework;
using MVCMusicStore.Controllers;
using System.Web.Mvc;

namespace MVCMusicStore.Tests.Controllers
{
    [TestFixture, Category(nameof(AccountController))]
    public class AccountControllerTest
    {
        //private ApplicationDbContext context;
        private AccountController controller;

        public AccountControllerTest()
        {
        }

        [SetUp]
        public void SetUp()
        {
            controller = new AccountController();
        }

        [Test]
        public void LoginGet()
        {
            var result = controller.Login("") as ActionResult;
            Assert.IsNotNull(result);
        }
    }
}
