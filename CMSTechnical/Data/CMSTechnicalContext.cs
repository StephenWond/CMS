using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CMSTechnical.Model;

namespace CMSTechnical.Data
{
    public class CMSTechnicalContext : DbContext
    {
        public CMSTechnicalContext (DbContextOptions<CMSTechnicalContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        }
    }
}
