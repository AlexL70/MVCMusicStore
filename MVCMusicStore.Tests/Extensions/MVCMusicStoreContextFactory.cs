using System;
using MVCMusicStore.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSubstitute;
using MockEfDbSet.Test.TestUtils;
using Microsoft.AspNet.Identity.EntityFramework;


namespace MVCMusicStore.Tests.Extensions
{
    public static class ContextFactory
    {
        private static string dataDir;
        public const string adminUserEmail = "admin@store.com";
        public const string adminPwd = "admin";
        public const string customUserEmail = "user@store.com";
        public const string customPwd = "user";

        static ContextFactory()
        {
            var wDir = TestContext.CurrentContext.TestDirectory;
            var rDir = Path.GetFullPath(Path.Combine(wDir, @"..\..\..\"));
            dataDir = $"{rDir}MVCMusicStore\\App_TestData\\";
        }

        public static MusicStoreEntities GetMusicStoreContext()
        {
            var context = Substitute.For<MusicStoreEntities>();
            //  Load Genres data
            var str = File.ReadAllText($"{dataDir}Genres.json");
            var genresRough = JsonConvert.DeserializeObject<List<Genre>>(str);
            int i = 0;
            genresRough.ForEach(g => g.GenreId = ++i);
            //  Load Artists data
            str = File.ReadAllText($"{dataDir}Artists.json");
            var artists = JsonConvert.DeserializeObject<List<Artist>>(str);
            i = 0;
            artists.ForEach(a => a.ArtistId = ++i);
            //  Load Albums data
            str = File.ReadAllText($"{dataDir}Albums.json");
            //  Defines anonymous type as an array of anonymous objects
            //  Anonymous object stores genre name and artist name 
            //  instead of actual Genre and Artist object
            var tmpAlbums = new[] { new
            {  //  Defines anonymous object for temporary album
                Title = "",
                Genre = "",
                Price = decimal.Zero,
                Artist = "",
                AlbumArtUrl = ""
            } };
            var albumsTemp = JsonConvert.DeserializeAnonymousType(str, tmpAlbums);
            var albums = new List<Album>();
            int albumId = 0;
            foreach (var albumTmp in albumsTemp)
            {
                var newAlbum = NSubstituteUtils.CreateMockEntity (new Album
                {
                    AlbumId = ++albumId,
                    Title = albumTmp.Title,
                    GenreId = genresRough.Single(g => g.Name == albumTmp.Genre).GenreId,
                    Price = albumTmp.Price,
                    ArtistId = artists.Single(a => a.Name == albumTmp.Artist).ArtistId,
                    AlbumArtUrl = albumTmp.AlbumArtUrl
                });
                NSubstituteUtils.SetMockVirtualProp(newAlbum, nameof(Album.Artist), al => artists.Single(a => a.ArtistId == al.ArtistId));
                albums.Add(newAlbum);
            }
            var genres = new List<Genre>();
            genresRough.ForEach(g => genres.Add(NSubstituteUtils.CreateMockEntity(g)));
            var mGenres = NSubstituteUtils.CreateMockDbSet(genres);
            var mArtists = NSubstituteUtils.CreateMockDbSet(artists);
            var mAlbums = NSubstituteUtils.CreateMockDbSet(albums);
            var carts = new List<Cart>();
            var mCarts = NSubstituteUtils.CreateMockDbSet(carts);
            var orders = new List<Order>();
            var mOrders = NSubstituteUtils.CreateMockDbSet(orders);
            var orderDetails = new List<OrderDetail>();
            var mOrderDetails = NSubstituteUtils.CreateMockDbSet(orderDetails);
            context.Genres.Returns(mGenres);
            genres.ForEach(gnr => NSubstituteUtils.SetMockVirtualProp(gnr, nameof(Genre.Albums),
                g => albums.Where(a => a.GenreId == g.GenreId).ToList()));
            albums.ForEach(alb => NSubstituteUtils.SetMockVirtualProp(alb, nameof(Album.Genre), 
                al => genres.Single(g => g.GenreId == al.GenreId)));
            context.Artists.Returns(mArtists);
            context.Albums.Returns(mAlbums);
            context.Carts.Returns(mCarts);
            context.Carts.When(crt => crt.Add(Arg.Any<Cart>())).Do(x =>
            {
                var cart = (Cart)x[0];
                cart.RecordId = carts.Count > 0 ? (carts.Select(c => c.RecordId).Max()) + 1 : 1;
                var mCart = NSubstituteUtils.CreateMockEntity(cart);
                NSubstituteUtils.SetMockVirtualProp(mCart, nameof(Cart.Album),
                    crt => albums.Where(a => a.AlbumId == crt.AlbumId).SingleOrDefault());
                carts.Add(mCart);
            });
            context.Carts.When(crt => crt.Remove(Arg.Any<Cart>())).Do(x => {
                carts.Remove((Cart)x[0]);
            });
            context.Orders.Returns(mOrders);
            context.OrderDetails.Returns(mOrderDetails);
            albums.ForEach(alb => NSubstituteUtils.SetMockVirtualProp(alb, nameof(Album.OrderDetails),
                al => orderDetails.Where(od => od.AlbumId == al.AlbumId).ToList()));
            return context;
        }

        public static ApplicationDbContext GetAppContext()
        {
            var context = Substitute.For<ApplicationDbContext>();
            var mUsers = NSubstituteUtils.CreateMockDbSet<ApplicationUser>();
            var mRoles = NSubstituteUtils.CreateMockDbSet<IdentityRole>();
            var mUserRoles = NSubstituteUtils.CreateMockDbSet<IdentityUserRole>();
            context.Users.Returns(mUsers);
            context.Roles.Returns(mRoles);
            context.Users.When(usr => usr.Add(Arg.Any<ApplicationUser>())).Do(x => {
                var user = (ApplicationUser)x[0];
                var mUser = NSubstituteUtils.CreateMockEntity(user);
                mUsers.Add(mUser);
            });
            context.Users.When(usr => usr.Remove(Arg.Any<ApplicationUser>())).Do(x => {
                mUsers.Remove((ApplicationUser)x[0]);
            });
            context.Roles.When(rl => rl.Add(Arg.Any<IdentityRole>())).Do(x => {
                var role = (IdentityRole)x[0];
                var mRole = NSubstituteUtils.CreateMockEntity(role);
                mRoles.Add(mRole);
            });
            context.Roles.When(rl => rl.Add(Arg.Any<IdentityRole>())).Do(x => {
                mRoles.Remove((IdentityRole)x[0]);
            });
            var sd = new SampleUserData();
            sd.pubSeed<ApplicationUser, IdentityRole, IdentityUserRole>(context: context,
                adminUserEmail: adminUserEmail,
                adminPwd: adminPwd,
                customUserEmail: customUserEmail,
                customPwd: customPwd);

            return context;
        }
    }
}
