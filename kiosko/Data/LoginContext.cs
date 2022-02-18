using kiosko.Models;
using Microsoft.EntityFrameworkCore;

namespace kiosko.Data
{
    public class LoginContext : DbContext
    {
        public LoginContext(DbContextOptions<LoginContext> options) : base(options)
        {

        }
        public DbSet<LoginModel> LoginItems { get; set; }
    }
}
