using Microsoft.EntityFrameworkCore;
using ndisforms.Data.Models;

#nullable disable

namespace ndisforms.Data
{

    public class NDISDbContext : DbContext
    {
        public NDISDbContext(DbContextOptions<NDISDbContext> options) : base(options)
        {
        }
        public DbSet<IR_Header> IR_Header { get; set; }
        public DbSet<TblEmail> TblEmail { get; set; }
    }
}