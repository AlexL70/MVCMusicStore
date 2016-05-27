using MVCMusicStore.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSubstitute;
using MockEfDbSet.Test.TestUtils;

namespace MVCMusicStore.Tests.Extensions
{
    public class GenreMock : Genre
    {
        public static IQueryable<Album> AllAlbums { get; set; }

        public override List<Album> Albums
        {
            get
            {
                return AllAlbums.Where(a => a.GenreId == GenreId).ToList();
            }
        }
    }


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
            var context = Substitute.For<MusicStoreEntities>();
            //  Load Genres data
            var str = File.ReadAllText($"{dataDir}Genres.json");
            var genres = JsonConvert.DeserializeObject<List<GenreMock>>(str);
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
            GenreMock.AllAlbums = albums.AsQueryable();
            var mGenres = NSubstituteUtils.CreateMockDbSet<Genre>(genres);
            var mArtists = NSubstituteUtils.CreateMockDbSet(artists);
            var mAlbums = NSubstituteUtils.CreateMockDbSet(albums);
            var mCarts = NSubstituteUtils.CreateMockDbSet<Cart>();
            var mOrders = NSubstituteUtils.CreateMockDbSet<Order>();
            var mOrderDetails = NSubstituteUtils.CreateMockDbSet<OrderDetail>();
            context.Genres.Returns(mGenres);
            context.Artists.Returns(mArtists);
            context.Albums.Returns(mAlbums);
            context.Carts.Returns(mCarts);
            context.Orders.Returns(mOrders);
            context.OrderDetails.Returns(mOrderDetails);
            foreach (var album in albums)
            {
                mAlbums.Add(album);
            }
            return context;
        }
    }
}
