using Moq;
using MVCMusicStore.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Entity;

namespace MVCMusicStore.Tests.Extensions
{
    public static class MVCMusicStoreContextFactory
    {
        private static string dataDir;

        static MVCMusicStoreContextFactory()
        {
            var wDir = TestContext.CurrentContext.TestDirectory;
            var rDir = Path.GetFullPath(Path.Combine(wDir, @"..\..\..\"));
            dataDir = $"{rDir}MVCMusicStore\\App_TestData\\";
        }

        public static MusicStoreEntities GetContext()
        {
            var context = new Mock<MusicStoreEntities>();
            //  Load Genres data
            var str = File.ReadAllText($"{dataDir}Genres.json");
            var genres = JsonConvert.DeserializeObject<List<Genre>>(str);
            int i = 0;
            genres.ForEach(g => g.GenreId = ++i);
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
                var artist0 = artists.Single(a => a.Name == albumTmp.Artist);
                var genre0 = genres.Single(g => g.Name == albumTmp.Genre);
                albums.Add(new Album
                {
                    AlbumId = ++albumId,
                    Title = albumTmp.Title,
                    GenreId = genre0.GenreId,
                    Genre = genre0,
                    Price = albumTmp.Price,
                    ArtistId = artist0.ArtistId,
                    Artist = artist0,
                    AlbumArtUrl = albumTmp.AlbumArtUrl
                });
            }
            var mGenres = genres.GetQueryableMockDbSet();
            var mArtists = artists.GetQueryableMockDbSet();
            var mAlbums = albums.GetQueryableMockDbSet();
            var mCarts = new Mock<DbSet<Cart>>();
            var mOrders = new Mock<DbSet<Order>>();
            var mOrderDetails = new Mock<DbSet<OrderDetail>>();
            context.Setup(c => c.Genres).Returns(mGenres.Object);
            context.Setup(c => c.Artists).Returns(mArtists.Object);
            context.Setup(c => c.Albums).Returns(mAlbums.Object);
            context.Setup(c => c.Carts).Returns(mCarts.Object);
            context.Setup(c => c.Orders).Returns(mOrders.Object);
            context.Setup(c => c.OrderDetails).Returns(mOrderDetails.Object);
            foreach (var album in albums)
            {
                mAlbums.Object.Add(album);
            }
            return context.Object;
        }
    }
}
