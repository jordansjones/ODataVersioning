using System;
using System.Data.Entity;
using System.Linq;

using ODataVersioning.Migrations;

namespace ODataVersioning.Models
{
    public abstract class BaseProductsServiceContext<T> : DbContext
        where T : class, IEntity, new()
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public BaseProductsServiceContext() : base("name=ProductsServiceContext")
        {
        }

        public DbSet<T> Items { get; set; }
    
    }

    public class ProductsServiceContext : BaseProductsServiceContext<SimpleProduct>
    {

        static ProductsServiceContext()
        {
            Database.SetInitializer<ProductsServiceContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SimpleProduct>().ToTable("dbo.CategorizedProducts");
            base.OnModelCreating(modelBuilder);
        }

    }

    public class CategorizedProductsServiceContext : BaseProductsServiceContext<CategorizedProduct>
    {

        static CategorizedProductsServiceContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<CategorizedProductsServiceContext, Configuration>());
        }
    }
}
