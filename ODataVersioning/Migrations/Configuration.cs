using ODataVersioning.Models;

namespace ODataVersioning.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ODataVersioning.Models.CategorizedProductsServiceContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }



        protected override void Seed(ODataVersioning.Models.CategorizedProductsServiceContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.Items.AddOrUpdate(
                new[]
                {
                    new CategorizedProduct { Id = 1, Name = "Product #1", Price = 1.0, Category = "Main Category"},
                    new CategorizedProduct { Id = 1, Name = "Product #2", Price = 2.0, Category = "Main Category"},
                    new CategorizedProduct { Id = 1, Name = "Product #3", Price = 3.0, Category = "Main Category"},
                    new CategorizedProduct { Id = 1, Name = "Product #4", Price = 4.0, Category = "Main Category"},
                    new CategorizedProduct { Id = 1, Name = "Product #5", Price = 5.0, Category = "Not the Main Category"},
                });
        }
    }
}
