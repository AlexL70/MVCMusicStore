using System;
using System.Data.Entity;
using MVCMusicStore.Models;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace ExtractData
{
    public class MStoreEntities : DbContext
    {
        public static string BaseDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));

        private static string ConnectionString()
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = "(LocalDb)\\MSSQLLocalDB";
            sqlBuilder.AttachDBFilename = $"{BaseDir}MVCMusicStore\\App_Data\\MVCMusicStore.mdf";
            sqlBuilder.InitialCatalog = "MVCMusicStore";
            //sqlBuilder.PersistSecurityInfo = true;
            sqlBuilder.IntegratedSecurity = true;
            //sqlBuilder.MultipleActiveResultSets = true;

            return sqlBuilder.ToString();
        }

        public MStoreEntities() : 
            base(ConnectionString()) { }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Artist> Artists { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MStoreEntities db = new MStoreEntities();

            string TDDir = $"{MStoreEntities.BaseDir}MVCMusicStore\\App_TestData\\";
            var genres = db.Genres.Select(g => new { Name = g.Name}).ToList();
            var str = JsonConvert.SerializeObject(genres);
            File.WriteAllText($"{TDDir}Genres.json", str);
            var artists = db.Artists.Select(a => new { Name = a.Name }).ToList();
            str = JsonConvert.SerializeObject(artists);
            File.WriteAllText($"{TDDir}Artists.json", str);
            var albums = db.Albums.Select(al => new {
                Title = al.Title,
                Genre = al.Genre.Name,
                Price = al.Price,
                Artist = al.Artist.Name,
                AlbumArtUrl = al.AlbumArtUrl
                }).ToList();
            str = JsonConvert.SerializeObject(albums);
            File.WriteAllText($"{TDDir}Albums.json", str);
            //Console.ReadKey();
        }
    }
}
