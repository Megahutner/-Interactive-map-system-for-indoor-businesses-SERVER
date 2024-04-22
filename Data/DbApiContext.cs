using Microsoft.EntityFrameworkCore;
using UWEServer.Entities;

namespace UWEServer.Data
{
    public class DbApiContext: DbContext
    {
        public DbApiContext(DbContextOptions options) : base(options) 
        {

        }
        public DbSet<Terminal> Terminals { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Zone> Zones { get; set; }
    }
}
