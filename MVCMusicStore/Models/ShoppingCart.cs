using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCMusicStore.Models
{
    public class ShoppingCart
    {
        MusicStoreEntities db = new MusicStoreEntities();

        private string ShoppingCartId { get; set; }

        public const string CartSessionKey = nameof(Cart.CartId);

        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        //  Helper method to simplify calling shopping cart from controller
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        public void AddToCart(Album album)
        {
            //  Get the matching cart and album instances from db
            var cartItem = db.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId &&
                c.AlbumId == album.AlbumId);

            if (cartItem == null)
            {   //  if album is not on cart, then create a new one and add to db context
                cartItem = new Cart
                {
                    AlbumId = album.AlbumId,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                db.Carts.Add(cartItem);
            }
            else
            {   //  if album is already on cart, then just increase count
                cartItem.Count++;
            }
            //  Save changes to database
            db.SaveChanges();
        }

        //  Removes item from cart and returns count of remainint items
        public int RemoveFromCart(int recordId)
        {
            int itemCount = 0;

            var cartItem = db.Carts.Single(
                c => c.CartId == ShoppingCartId &&
                c.RecordId == recordId);

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    db.Carts.Remove(cartItem);
                }
                db.SaveChanges();
            }

            return itemCount;
        }

        //  Removes all items from the cart
        public void EmptyCart()
        {
            var cartItems = db.Carts.Where(c => c.CartId == ShoppingCartId);
            foreach (var item in cartItems)
            {
                db.Carts.Remove(item);
            }
            db.SaveChanges();
        }

        //  Returns all items from the cart
        public List<Cart> GetCartItems()
        {
            return db.Carts.Where(c => c.CartId == ShoppingCartId).ToList();
        }

        //  Returns count of all items on shopping cart
        public int GetCount()
        {
            int? count = db.Carts.Where(c => c.CartId == ShoppingCartId).Select(c => (int?)c.Count).Sum();
            return count ?? 0;
        }

        //  Returns the cart's total (value of all albums on the cart)
        public decimal GetTotal()
        {
            decimal? total = db.Carts.Where(c => c.CartId == ShoppingCartId)
                .Select(c => (int?)c.Count * c.Album.Price).Sum();
            return total ?? decimal.Zero;
        }

        //  Creates new order out of shopping cart and returns OrderId
        public int CreateOrder(Order order)
        {
            decimal OrderTotal = 0;

            var cartItems = GetCartItems();

            //  add each cart item to OrderDetails
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    AlbumId = item.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Album.Price,
                    Quantity = item.Count
                };

                //  Increment total
                OrderTotal += orderDetail.UnitPrice * orderDetail.Quantity;
                //  Add new detail to context
                db.OrderDetails.Add(orderDetail);
            }

            //  Set total
            order.Total = OrderTotal;
            //  Save order changes
            db.SaveChanges();
            //  Empty the shopping cart
            EmptyCart();

            return order.OrderId;
        }

        //  Method returns cart id for current session
        //  HttpContextBase is used to allow access to cookies
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                //  Send cart id back to client as cookie
                if (!String.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    //  If user is authenticated, then use user name as cart id
                    context.Session[CartSessionKey] = context.User.Identity.Name;
                }
                else
                {
                    //  If user is not authenticated, then generate random cart id
                    context.Session[CartSessionKey] = Guid.NewGuid().ToString();
                }
            }
            return context.Session[CartSessionKey].ToString();
        }

        //  Migrate shopping cart created by anonymous user to his/her
        //  real user name after user logged in
        public void MigrateCart(string UserName)
        {
            var shoppingCart = db.Carts.Where(c => c.CartId == ShoppingCartId);
            foreach (var item in shoppingCart)
            {
                item.CartId = UserName;
            }
            db.SaveChanges();
            ShoppingCartId = UserName;
        }
    }
}
