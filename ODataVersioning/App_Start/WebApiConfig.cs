using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;

using ODataVersioning.Controllers;
using ODataVersioning.Helpers;
using ODataVersioning.Models;

namespace ODataVersioning
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            var odataBuilder = new ODataConventionModelBuilder();

            odataBuilder.ForEndpoint("Products")
                        .OnVersion(1).MapEntitySet<SimpleProduct>().ToController<SimpleProductsController>()
                        .OnVersionRange(2, 5).MapEntitySet<CategorizedProduct>().ToController<CategorizedProductsController>()
                        .Build();

            config.Routes.MapVersionedODataRoute("odata", "svc", odataBuilder.GetEdmModel());
        }
    }
}
