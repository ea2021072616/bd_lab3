using Microsoft.EntityFrameworkCore;
using ClienteAPI.Models;

namespace ClienteAPI.Data
{
    public class BdClientesContext : DbContext
    {
        public BdClientesContext(DbContextOptions<BdClientesContext> options)
            : base(options)
        {
        }

        // Definición de la tabla Clientes
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<TiposDocumentos> TiposDocumentos { get; set; }
    }
}
