using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Concepts.Crud.Infrastructure.Postgres;

public class Program
{
    public static int Main()
    {
        return 0;
    }
}

public class CrudContextFactory : IDesignTimeDbContextFactory<CrudContext>
{
    public CrudContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CrudContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=cruddb;Username=crud;Password=password");
        return new CrudContext(optionsBuilder.Options);
    }
}