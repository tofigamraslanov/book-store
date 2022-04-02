using System;
using System.Linq;
using BulkyBook.DataAccess.Data;
using BulkyBook.Entities;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception e)
            {
                throw;
            }

            if (_roleManager.RoleExistsAsync(StaticDetails.RoleAdmin).GetAwaiter().GetResult()) return;

            _roleManager.CreateAsync(new IdentityRole(StaticDetails.RoleAdmin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.RoleEmployee)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.RoleUserCompany)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.RoleUserIndividual)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                PhoneNumber = "0505388389",
                StreetAddress = "Zarifa Aliyeva",
                State = "AZE",
                City = "Baku",
                PostalCode = "883838839",
                Name = "Tofig Amraslanov"
            }, "Admin01*").GetAwaiter().GetResult();
 
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@gmail.com");

            _userManager.AddToRoleAsync(user, StaticDetails.RoleAdmin).GetAwaiter().GetResult();
        }
    }
}