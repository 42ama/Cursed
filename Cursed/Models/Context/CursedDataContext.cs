using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Cursed.Models.Entities;

namespace Cursed.Models.Context
{
    public partial class CursedDataContext : DbContext
    {
        public CursedDataContext()
        {
        }

        public CursedDataContext(DbContextOptions<CursedDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Facility> Facility { get; set; }
        public virtual DbSet<License> License { get; set; }
        public virtual DbSet<Operation> Operation { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductCatalog> ProductCatalog { get; set; }
        public virtual DbSet<Recipe> Recipe { get; set; }
        public virtual DbSet<RecipeInheritance> RecipeInheritance { get; set; }
        public virtual DbSet<RecipeProductChanges> RecipeProductChanges { get; set; }
        public virtual DbSet<Storage> Storage { get; set; }
        public virtual DbSet<TechProcess> TechProcess { get; set; }
        public virtual DbSet<TransactionBatch> TransactionBatch { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Facility>(entity =>
            {
                entity.Property(e => e.Latitude).HasColumnType("decimal(8, 6)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(8, 6)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<License>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("date");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.License)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_License_Product_Id");
            });

            modelBuilder.Entity<Operation>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.Quantity).HasColumnType("decimal(9, 2)");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Operation)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Operation_Product_Id");

                entity.HasOne(d => d.StorageFrom)
                    .WithMany(p => p.OperationStorageFrom)
                    .HasForeignKey(d => d.StorageFromId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Operation_StorageFrom_Id");

                entity.HasOne(d => d.StorageTo)
                    .WithMany(p => p.OperationStorageTo)
                    .HasForeignKey(d => d.StorageToId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Operation_StorageTo_Id");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.Operation)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Operation_Transaction_Id");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.Quantity).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.QuantityUnit)
                    .IsRequired()
                    .HasMaxLength(3);

                entity.Property(e => e.Uid).HasColumnName("UId");

                entity.HasOne(d => d.Storage)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.StorageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Storage_Id");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_ProductCatalog_Id");
            });

            modelBuilder.Entity<ProductCatalog>(entity =>
            {
                entity.Property(e => e.Cas)
                    .HasColumnName("CAS")
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.Property(e => e.Content).IsRequired();
            });

            modelBuilder.Entity<RecipeInheritance>(entity =>
            {
                //entity.HasNoKey();
                entity.HasKey(e => new { e.ChildId, e.ParentId })
                    .HasName("CK_RecipeInheritance_ChildId_ParentId");

                entity.HasOne(d => d.Child)
                    .WithMany(p => p.RecipeInheritanceChild)
                    .HasForeignKey(d => d.ChildId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipeInheritance_Recipe_CId");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.RecipeInheritanceParent)
                    .HasForeignKey(d => d.ParentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipeInheritance_Recipe_PId");
            });

            modelBuilder.Entity<RecipeProductChanges>(entity =>
            {
                entity.HasKey(e => new { e.RecipeId, e.ProductId, e.Type })
                    .HasName("CK_RecipeProductChanges_RecipeId_ProductId_Type");

                entity.Property(e => e.Type)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.Quantity).HasColumnType("decimal(9, 2)");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.RecipeProductChanges)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipeProductChanges_ProductCatalog_Id");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeProductChanges)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipeProductChanges_Recipe_Id");
            });
            
            modelBuilder.Entity<Storage>(entity =>
            {
                entity.Property(e => e.Latitude).HasColumnType("decimal(8, 6)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(8, 6)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Storage)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Storage_Company_Id");
            });

            modelBuilder.Entity<TechProcess>(entity =>
            {
                //entity.HasNoKey();
                entity.HasKey(e => new { e.FacilityId, e.RecipeId })
                     .HasName("CK_TechProcess_FacilityId_RecipeId");

                entity.Property(e => e.DayEfficiency).HasColumnType("decimal(9, 2)");

                entity.HasOne(d => d.Facility)
                    .WithMany(p => p.TechProcess)
                    .HasForeignKey(d => d.FacilityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TechProcess_Facility_Id");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.TechProcess)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TechProcess_Recipe_Id");
            });

            modelBuilder.Entity<TransactionBatch>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.IsOpen)
                    .IsRequired();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TransactionBatch)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Operation_Company_Id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
