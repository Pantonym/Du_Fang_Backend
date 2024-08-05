using Microsoft.EntityFrameworkCore;

namespace Du_Fang;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

    // TODO: Specify endpoint controllers
    // public DbSet<ToDoItem> ToDoItems { get; set; }

}