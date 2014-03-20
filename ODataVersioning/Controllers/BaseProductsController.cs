using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.OData;

using ODataVersioning.Models;

namespace ODataVersioning.Controllers
{

    public abstract class BaseProductsController<T> : ODataController
        where T : class, IEntity, new()
    {

        private readonly BaseProductsServiceContext<T> db;

        protected BaseProductsController(BaseProductsServiceContext<T> dbCtx) : base()
        {
            db = dbCtx;
        }

        // GET odata/Products
        [Queryable]
        public IQueryable<T> Get()
        {
            return db.Items.AsQueryable();
        }

        // GET odata/Products(5)
        [Queryable]
        public SingleResult<T> Get([FromODataUri] int key)
        {
            return SingleResult.Create(db.Items.Where(product => product.Id == key));
        }

        // PUT odata/Products(5)
        public IHttpActionResult Put([FromODataUri] int key, T product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != product.Id)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(product);
        }

        // POST odata/Products
        public IHttpActionResult Post(T product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Items.Add(product);
            db.SaveChanges();

            return Created(product);
        }

        // DELETE odata/Products(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            var product = db.Items.Find(key);
            if (product == null)
            {
                return NotFound();
            }

            db.Items.Remove(product);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int key)
        {
            return db.Items.Count(e => e.Id == key) > 0;
        }

    }

    
    public class SimpleProductsController : BaseProductsController<SimpleProduct>
    {

        public SimpleProductsController() : base(new ProductsServiceContext()) {}

    }
    
    public class CategorizedProductsController : BaseProductsController<CategorizedProduct>
    {

        public CategorizedProductsController() : base(new CategorizedProductsServiceContext()) {}

    }
}
