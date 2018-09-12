using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Accounts.Models
{
    public partial class UHCContext : DbContext
    {
        public UHCContext()
        {
        }

        public UHCContext(DbContextOptions<UHCContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppUsers> AppUsers{ get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer("Server=.;Database=UHC;Trusted_Connection=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUsers > ()
                .HasIndex(p => new { p.IdNumber, p.TerminalId,p.PhoneNumber})
                .IsUnique(true);
        }
    }
}
