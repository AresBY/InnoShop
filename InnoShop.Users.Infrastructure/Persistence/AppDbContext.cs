using InnoShop.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace InnoShop.Users.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    // Здесь можно настроить маппинг сущностей, если нужно
    //    base.OnModelCreating(modelBuilder);
    //}

}
