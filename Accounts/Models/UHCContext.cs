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

        public virtual DbSet<Agents> Agents { get; set; }

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agents>(entity =>
            {
                entity.Property(e => e.DateRegistered).HasColumnType("datetime");

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.IdNumber).HasMaxLength(50);

                entity.Property(e => e.LastModified).HasColumnType("datetime");

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.MiddleName).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.TerminalId).HasMaxLength(50);

                entity.Property(e => e.UserName).HasMaxLength(20);
            });
        }
    }
}
