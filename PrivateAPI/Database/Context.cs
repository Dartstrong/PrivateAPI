using Microsoft.EntityFrameworkCore;

namespace PrivateAPI.Models
{
    public class Context: DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public virtual DbSet<Session> Sessions { get; set; }
    }
}
