using Market.Identity.Application;
using Market.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Identity.Infrastructure.Persistence;

public static class RolesSeed
{
    public static void SeedRoles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = 1,
                Name = Roles.ShopOwner.ToString(),
                Title = "Владелец магазина",
                Description = "Владелец магазина имеет полный доступ ко всем магазинам, которые он создал"
            },
            new Role
            {
                Id = 2,
                Name = Roles.Customer.ToString(),
                Title = "Покупатель",
                Description = "Обычный покупатель, который может просматривать товары и оформлять заказы"
            }
        );
    }
}