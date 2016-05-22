using MVCMusicStore.Models;
using MVCMusicStore.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace MVCMusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        private MusicStoreEntities db = new MusicStoreEntities();

        // GET: /ShoppingCart/
        [HttpGet]
        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this);

            //  Set up ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };

            return View(viewModel);
        }

        // GET: /Store/AddToCart/5
        [HttpGet]
        public ActionResult AddToCart(int albumId)
        {
            //  Get album from db
            var album = db.Albums.Single(a => a.AlbumId == albumId);
            //  Add album to shopping cart
            var cart = ShoppingCart.GetCart(this);
            cart.AddToCart(album);
            //  Back to main page for more shopping
            return RedirectToAction(nameof(this.Index));
        }

        //  AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public ActionResult RemoveFromCart(int recordId)
        {   //  Remove the item from the shopping cart
            var cart = ShoppingCart.GetCart(this);
            //  Get the name of the album to display confirmation
            string albumTitle = db.Carts.Single(c => c.RecordId == recordId).Album.Title;
            //  Remove album from cart
            int itemCount = cart.RemoveFromCart(recordId);
            //  Display confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = $"{Server.HtmlEncode(albumTitle)} has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = recordId
            };

            return Json(results);
        }

        //  GET: /ShoppingCart/CartSummary/
        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this);
            ViewData[$"{nameof(Cart)}{nameof(Cart.Count)}"] = cart.GetCount();
            return PartialView(nameof(this.CartSummary));
        }
    }
}