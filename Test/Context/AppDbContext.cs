using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace Test.Data {
    public class AppDbContext : DbContext {

       public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
        
        }

        public DbSet<Usuario> usuario { get; set; }
        public DbSet<Cuenta> cuenta{ get; set; }
        public DbSet<Cuenta_Transaccion> cuenta_transaccion { get; set; }

        }
}
