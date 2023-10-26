using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShopCMS.Models;

namespace OnlineShopCMS.Data
{
    public class OnlineShopCMSContext : DbContext
    {
        public OnlineShopCMSContext (DbContextOptions<OnlineShopCMSContext> options)
            : base(options)
        {
        }

        public DbSet<OnlineShopCMS.Models.Product> Product { get; set; } = default!;
        public DbSet<OnlineShopCMS.Models.Category> Category { get; internal set; }
    }
}
