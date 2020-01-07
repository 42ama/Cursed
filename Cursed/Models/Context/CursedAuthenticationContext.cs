using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Cursed.Models.Entities.Authentication;

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

        public virtual DbSet<Policy> Policy { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RoleHavePolicy> RoleHavePolicy { get; set; }
        public virtual DbSet<UserAuth> UserAuth { get; set; }
        public virtual DbSet<UserData> UserData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=pharmaceuticsAuth;Trusted_Connection=True;MultipleActiveResultSets=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Policy>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<RoleHavePolicy>(entity =>
            {
                entity.HasNoKey();

                entity.HasOne(d => d.Policy)
                    .WithMany(p => p.RoleHavePolicy)
                    .HasForeignKey(d => d.PolicyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleHavePolicy_Policy_Id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleHavePolicy)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleHavePolicy_Role_Id");
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
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
