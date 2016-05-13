using System.Web.Mvc;
using MVCMusicStore.Models;
using System;
using System.Linq;

namespace MVCMusicStore.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private MusicStoreEntities db = new MusicStoreEntities();
        private const string PromoCode = "FREE";

        //  GET: /Checkout/AddressAndPayment
        public ActionResult AddressAndPayment()
        {
            return View();
        }

        //  POST: /Checkout/AddressAndPayment
        [HttpPost]
        public ActionResult AddressAndPayment(FormCollection values)
        {
            var order = new Order();
            TryUpdateModel(order);

            try
            {
                if (!string.Equals(values[nameof(PromoCode)], PromoCode,
                    StringComparison.OrdinalIgnoreCase))
                {   //  Wrong promo code
                    return View(order);
                }
                else
                {
                    order.UserName = User.Identity.Name;
                    order.OrderDate = DateTime.Now;
                    //  Save order
                    db.Orders.Add(order);
                    db.SaveChanges();
                    //  Process order
                    var cart = ShoppingCart.GetCart(this);
                    cart.CreateOrder(order);
                    return RedirectToAction(nameof(Complete),
                        new { orderId = order.OrderId });
                }
            }
            catch
            {
                //  If any errors occur, then redisplay with error messages
                return View(order);
            }
        }

        public ActionResult Complete(int orderId)
        {
            //  Validate if customer really owns this order
            bool isOwned = db.Orders.Any(o => o.OrderId == orderId
                && o.UserName == User.Identity.Name);
            if (isOwned)
            {
                return View(orderId);
            }
            else
            {
                //  Redirect to "Error" shared view
                return View("Error");
            }
        }
    }
}