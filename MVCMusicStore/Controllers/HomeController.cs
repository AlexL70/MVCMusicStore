using System.Web.Mvc;
using MVCMusicStore.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVCMusicStore.Controllers
{
    public class HomeController : Controller
    {
        private MusicStoreEntities db;

        public HomeController() : base()
        {
            db = new MusicStoreEntities();
        }

        public HomeController(MusicStoreEntities context) : base()
        {
            db = context;
        }

        public ActionResult Index()
        {
            //  Get 5 most popular albums
            var albums = GetTopSellingAlbums(5);
            return View(albums);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private List<Album> GetTopSellingAlbums(int count)
        {
            //  Group OrderDetails table by AlbumId and
            //  take albums with the highest cound
            return db.Albums
                .OrderByDescending(a => a.OrderDetails.Count())
                .Take(count).ToList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}