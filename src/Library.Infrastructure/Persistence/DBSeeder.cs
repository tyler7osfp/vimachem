using Library.Domain;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(LibraryDbContext ctx)
        {
            if (await ctx.Categories.AnyAsync()) return;

            var epic = Category.Create("Poetry");
            var philosophy = Category.Create("Philosophy");
            var fiction = Category.Create("Fiction");
            var mystery = Category.Create("Mystery");
            ctx.Categories.AddRange(epic, philosophy, fiction, mystery);

            var homer = Party.Create("Homer", "homer@library.gr", RoleType.Author);
            var nikos = Party.Create("Nikos Papadopoulos", "nikos@library.gr", RoleType.Customer);
            var plato = Party.Create("Plato", "plato@library.gr", RoleType.Author);
            ctx.Parties.AddRange(homer, nikos, plato);

            ctx.PartyRoles.Add(PartyRole.Create(plato.Id, RoleType.Customer));

            ctx.Books.AddRange(
                Book.Create("The Iliad", homer, epic.Id, 4),
                Book.Create("The Odyssey", homer, epic.Id, 3),
                Book.Create("The Republic", plato, philosophy.Id, 5),
                Book.Create("The Symposium", plato, philosophy.Id, 2)
            );

            await ctx.SaveChangesAsync();
        }
    }
}
