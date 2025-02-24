using Microsoft.EntityFrameworkCore;

namespace Coordinator.Models.EntityFramework;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options){}

    public DbSet<Node> Nodes { get; set; }
    public DbSet<NodeState> NodeStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Node>().HasData(
            new Node { Id = Guid.Parse("3b60112a-4f07-41cb-8dbd-45b501571a13"), Name = "Order.Api" },
            new Node { Id = Guid.Parse("ac65630b-59ca-43c3-837a-98991edc98ae"), Name = "Stock.Api" },
            new Node { Id = Guid.Parse("b3cb5183-ec29-469d-938f-af2f286c041d"), Name = "Payment.Api" });
    }
}
