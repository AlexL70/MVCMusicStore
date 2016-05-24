using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Configuration;
using System.IO;
using Newtonsoft.Json;
using System.Web;

namespace MVCMusicStore.Models
{
    public class SampleData : DropCreateDatabaseIfModelChanges<MusicStoreEntities>
    {
        protected override void Seed(MusicStoreEntities context)
        {
            string appPath = HttpContext.Current.Request.ApplicationPath;
            string Dir = $"{HttpContext.Current.Request.MapPath(appPath)}App_TestData\\";
            string str = File.ReadAllText($"{Dir}Genres.json");
            var genres = JsonConvert.DeserializeObject<List<Genre>>(str);
            str = File.ReadAllText($"{Dir}Artists.json");
            var artists = JsonConvert.DeserializeObject<List<Artist>>(str);
            str = File.ReadAllText($"{Dir}Albums.json");
            //  Defines anonymous type as an array of anonymous objects
            var tmpAlbums = new[] { new
            {  //  Defines anonymous object for temporary album
                Title = "",
                Genre = "",
                Price = decimal.Zero,
                Artist = "",
                AlbumArtUrl = ""
            } };
            var albums = JsonConvert.DeserializeAnonymousType(str, tmpAlbums);
            foreach (var alb in albums)
            {
                context.Albums.Add(new Album
                {
                    Title = alb.Title,
                    Genre = genres.Single(g => g.Name == alb.Genre),
                    Price = alb.Price,
                    Artist = artists.Single(a => a.Name == alb.Artist),
                    AlbumArtUrl = alb.AlbumArtUrl
                });
            }
        }
    }

    public class SampleUserData : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var passwordHasher = new PasswordHasher();
            //var adminUserEmail = configuration("AdminEmail");
            var adminUserEmail = WebConfigurationManager.AppSettings["AdminEmail"];
            var customUserEmail = WebConfigurationManager.AppSettings["CustomerEmail"];
            var adminPwd = passwordHasher.HashPassword(
                WebConfigurationManager.AppSettings["AdminPwd"]
                );
            var customPwd = passwordHasher.HashPassword(
                WebConfigurationManager.AppSettings["CustomerPwd"]
                );
            var adminUser = new ApplicationUser
            {
                UserName = adminUserEmail,
                PasswordHash = adminPwd,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                Email = adminUserEmail
            };
            context.Users.AddOrUpdate(u => u.UserName, adminUser);
            var custUser = new ApplicationUser
            {
                UserName = customUserEmail,
                PasswordHash = customPwd,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                Email = customUserEmail
            };
            context.Users.AddOrUpdate(u => u.UserName, custUser);
            var adminRole = new IdentityRole { Name = nameof(StdRoles.Administrators) };
            context.Roles.AddOrUpdate(r => r.Name, adminRole);
            var manRole = new IdentityRole { Name = nameof(StdRoles.Managers) };
            context.Roles.AddOrUpdate(r => r.Name, manRole);
            var custRole = new IdentityRole { Name = nameof(StdRoles.Customers) };
            context.Roles.AddOrUpdate(r => r.Name, custRole);
            context.SaveChanges();
            context.Roles.Where(r => r.Name == nameof(StdRoles.Administrators)).Single().Users.Add(
                new IdentityUserRole { UserId = adminUser.Id }
                );
            context.Roles.Where(r => r.Name == nameof(StdRoles.Managers)).Single().Users.Add(
                new IdentityUserRole { UserId = adminUser.Id }
                );
            context.Roles.Where(r => r.Name == nameof(StdRoles.Customers)).Single().Users.Add(
                new IdentityUserRole { UserId = custUser.Id }
                );
        }
    }
}