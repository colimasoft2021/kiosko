using kiosko.Models;
using Microsoft.EntityFrameworkCore;

namespace kiosko.Data
{
    public class KColSoftContext : DbContext
    {
        public KColSoftContext(DbContextOptions<KColSoftContext> options) : base(options)
        {

        }
        public DbSet<KColSoftModel> KColSoftsItem { get; set; }
    }
}
