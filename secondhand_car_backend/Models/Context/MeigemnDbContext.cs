using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarCheck.Backend.Models.Context
{
    // (creacion de usuario)Heredamos de IdentityDbContext<IdentityUser>. 
    
    public class MeigemnDbContext : IdentityDbContext<IdentityUser>
    {
        // (creacion de usuario) El constructor recibe las opciones (como la cadena de conexión) 
        // y las pasa a la clase base (base(options)).
        public MeigemnDbContext(DbContextOptions<MeigemnDbContext> options)
            : base(options)
        {
        }

        // 4. DbSet: Aquí es donde declararemos las tablas de los coches más adelante.
        // Por ahora, al heredar de IdentityDbContext, las tablas de usuarios son "invisibles" pero están ahí.
        // public DbSet<CarPart> CarParts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            base.OnModelCreating(modelBuilder);

            // 6. Aquí es donde en el futuro pondremos las reglas personalizadas para los coches.
            // Ejemplo: Indicar que el nombre de la pieza es obligatorio.
            /*
            modelBuilder.Entity<CarPart>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
            });
            */
        }
    }
}

