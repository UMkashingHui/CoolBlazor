using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoolWebApi.Models.Identity;
using CoolWebApi.Services.AppTime;
using CoolWebApi.Services.Identity;
using CoolWebApi.Utils.Entities.Catalog;
using CoolWebApi.Utils.Entities.Contracts;
using CoolWebApi.Utils.Entities.ExtendedAttributes;
using CoolWebApi.Utils.Entities.Misc;

namespace CoolWebApi.Utils.Contexts
{
    public class CoolBlazorDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public CoolBlazorDbContext(DbContextOptions options, ICurrentUserService currentUserService, IDateTimeService dateTimeService)
            : base(options)
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        public virtual DbSet<Product> Products { get; init; }
        public virtual DbSet<Brand> Brands { get; init; }
        public virtual DbSet<Document> Documents { get; init; }
        public virtual DbSet<DocumentType> DocumentTypes { get; init; }
        public virtual DbSet<DocumentExtendedAttribute> DocumentExtendedAttributes { get; init; }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = _dateTimeService.NowUtc;
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = _dateTimeService.NowUtc;
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        break;
                }
            }
            if (_currentUserService.UserId == null)
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            else
            {
                return await base.SaveChangesAsync(_currentUserService.UserId.Any(), cancellationToken);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.Name is "LastModifiedBy" or "CreatedBy"))
            {
                property.SetColumnType("nvarchar(128)");
            }

            base.OnModelCreating(builder);
            builder.Entity<CoolBlazorUser>(entity =>
            {
                entity.ToTable(name: "Users", "Identity");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            builder.Entity<CoolBlazorRole>(entity =>
            {
                entity.ToTable(name: "Roles", "Identity").HasKey(x => x.Id);
            });
            builder.Entity<CoolBlazorUserRole>(entity =>
            {
                entity.ToTable("UserRoles", "Identity").HasKey(x => x.Id);
            });

            builder.Entity<IdentityUserClaim<ObjectId>>(entity =>
            {
                entity.ToTable("UserClaims", "Identity");
            });

            builder.Entity<CoolBlazorUserLogin>(entity =>
            {
                entity.ToTable("UserLogins", "Identity").HasKey(x => x.Id);
            });

            builder.Entity<CoolBlazorRoleClaim>(entity =>
            {
                entity.ToTable(name: "RoleClaims", "Identity");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaims)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<CoolBlazorUserToken>(entity =>
            {
                entity.ToTable("UserTokens", "Identity").HasKey(x => x.Id);
            });
        }
    }
}
