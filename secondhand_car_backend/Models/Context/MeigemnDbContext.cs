using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using secondhand_car_backend.Models.Entities;

namespace secondhand_car_backend.Models.Context
{
   
    public class MeigemnDbContext : IdentityDbContext<IdentityUser>
    {
        // (creacion de usuario) El constructor recibe las opciones (como la cadena de conexión) 
        // y las pasa a la clase base (base(options)).
        public MeigemnDbContext(DbContextOptions<MeigemnDbContext> options)
            : base(options)
        {
        }

        #region dbSets
        public DbSet<CarPart> CarParts { get; set; }
        public DbSet<PartCriterionDto> PartCriteria { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            base.OnModelCreating(modelBuilder);

            #region tables

            modelBuilder.Entity<CarPart>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PartName).IsRequired();
                entity.Property(e => e.Category).IsRequired();
                entity.HasMany(e => e.Criteria)
                      .WithOne(c => c.CarPart)
                      .HasForeignKey(c => c.CarPartId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<PartCriterion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.IsPositive).IsRequired();
                entity.Property(e => e.CarPartId).IsRequired();
            });

            #endregion
        }
    }
}

