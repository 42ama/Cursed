using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Cursed.Models.Entities.Authentication;
using System.Configuration;

namespace Cursed.Models.Context
{
    public partial class CursedAuthenticationContext : DbContext
    {
        public CursedAuthenticationContext()
        {
        }

        public CursedAuthenticationContext(DbContextOptions<CursedAuthenticationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<UserAuth> UserAuth { get; set; }
        public virtual DbSet<UserData> UserData { get; set; }
        public virtual DbSet<LogRecord> LogRecord { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["AuthenticationDatabaseConnection"].ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK_Role_Name_Clustered");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserAuth>(entity =>
            {
                entity.HasKey(e => e.Login)
                    .HasName("PK_UserAuth_Login_Clustered");

                entity.Property(e => e.Login)
                    .HasMaxLength(39)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(48)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LogRecord>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_LogRecord_Id_Clustered");

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnType("datetime");

                entity.Property(e => e.UserLogin)
                    .IsRequired()
                    .HasMaxLength(39)
                    .IsUnicode(false);

                entity.Property(e => e.UserIP)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.HasOne(d => d.UserAuth)
                    .WithOne(p => p.LogRecord)
                    .HasForeignKey<LogRecord>(d => d.UserLogin)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<UserData>(entity =>
            {
                entity.HasKey(e => e.Login)
                    .HasName("PK_UserData_Login_Clustered");

                entity.Property(e => e.Login)
                    .HasMaxLength(39)
                    .IsUnicode(false);

                entity.HasOne(d => d.LoginNavigation)
                    .WithOne(p => p.UserData)
                    .HasForeignKey<UserData>(d => d.Login)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserData)
                    .HasForeignKey(d => d.RoleName)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
