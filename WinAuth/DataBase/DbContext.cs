using Microsoft.EntityFrameworkCore;

namespace WinAuth.DataBase
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {

        }

        public DbSet<DbItem> CatalogItems { get; set; }
    }
}