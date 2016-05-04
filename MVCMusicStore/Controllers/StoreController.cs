using System.Web;
using System.Web.Mvc;
using MVCMusicStore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace MVCMusicStore.Controllers
{
    public class StoreController : Controller
    {
        MusicStoreEntities db = new MusicStoreEntities();

        // GET: Store
        public ActionResult Index()
        {
            var genres = db.Genres.ToList();
            return View(genres);
        }

        // GET: Store/Browse?genre=Blues
        public ActionResult Browse(string genre)
        {
            var genreModel = db.Genres.Where(g => g.Name == genre)
                .Include(g => g.Albums).Single();
            return View(genreModel);
        }

        // GET: Store/Details/5
        public ActionResult Details(int id)
        {
            var album = db.Albums.Find(id);
            return View(album);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}