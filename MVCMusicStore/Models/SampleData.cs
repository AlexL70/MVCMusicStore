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
        public void pubSeed<TUser, TRole, TUserRole>(ApplicationDbContext context,
            string adminUserEmail, string adminPwd, string customUserEmail, string customPwd)
            where TUser : ApplicationUser, new()
            where TRole : IdentityRole, new()
            where TUserRole : IdentityUserRole, new()
        {
            var adminUser = new TUser
            {
                UserName = adminUserEmail,
                PasswordHash = adminPwd,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                Email = adminUserEmail
            };
            context.Users.AddOrUpdate(u => u.UserName, adminUser);
            var custUser = new TUser
            {
                UserName = customUserEmail,
                PasswordHash = customPwd,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                Email = customUserEmail
            };
            context.Users.AddOrUpdate(u => u.UserName, custUser);
            var adminRole = new TRole { Name = nameof(StdRoles.Administrators) };
            context.Roles.AddOrUpdate(r => r.Name, adminRole);
            var manRole = new TRole { Name = nameof(StdRoles.Managers) };
            context.Roles.AddOrUpdate(r => r.Name, manRole);
            var custRole = new TRole { Name = nameof(StdRoles.Customers) };
            context.Roles.AddOrUpdate(r => r.Name, custRole);
            context.SaveChanges();
            adminRole.Users.Add(new TUserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
            manRole.Users.Add(new TUserRole { UserId = adminUser.Id, RoleId = manRole.Id });
            custRole.Users.Add(new TUserRole { UserId = custUser.Id, RoleId = custRole.Id });
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var passwordHasher = new PasswordHasher();
            var adminUserEmail = WebConfigurationManager.AppSettings["AdminEmail"];
            var customUserEmail = WebConfigurationManager.AppSettings["CustomerEmail"];
            var adminPwd = passwordHasher.HashPassword(
                WebConfigurationManager.AppSettings["AdminPwd"]
                );
            var customPwd = passwordHasher.HashPassword(
                WebConfigurationManager.AppSettings["CustomerPwd"]
                );
            pubSeed<ApplicationUser, IdentityRole, IdentityUserRole>(context: context,
                adminUserEmail: adminUserEmail,
                adminPwd: adminPwd,
                customUserEmail: customUserEmail,
                customPwd: customPwd);
        }
    }
}