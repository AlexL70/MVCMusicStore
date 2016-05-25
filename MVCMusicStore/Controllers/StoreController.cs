using System.Web.Mvc;
using MVCMusicStore.Models;
using System.Linq;
using System.Data.Entity;

namespace MVCMusicStore.Controllers
{
    public class StoreController : Controller
    {
        private MusicStoreEntities db;

        public StoreController() : base()
        {
            db = new MusicStoreEntities();
        }

        public StoreController(MusicStoreEntities context) : base()
        {
            db = context;
        }

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
            var album = db.Albums.Where(a => a.AlbumId == id).Single();
            //var album = db.Albums.Find(id);
            return View(album);
        }

        //  GET: /Store/GenreMenu
        [ChildActionOnly]
        public ActionResult GenreMenu()
        {
            var genres = db.Genres.ToList();
            return PartialView(genres);
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