using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Routing;
using System.Web.Http.OData.Routing.Conventions;

using Microsoft.Data.Edm;

namespace ODataVersioning.Helpers
{
    public static class ODataRouteExtensions
    {

        public static void MapVersionedODataRoute(this HttpRouteCollection routes, string routeName, string routePrefix, IEdmModel model)
        {
            routes.MapVersionedODataRoute(routeName, routePrefix, model, new VersionAwareODataPathHandler(), ODataRoutingConventions.CreateDefault());
        }

        public static void MapVersionedODataRoute(this HttpRouteCollection routes, string routeName, string routePrefix, IEdmModel model, VersionAwareODataPathHandler pathHandler, IEnumerable<IODataRoutingConvention> routingConvetions)
        {
            var constraint = new VersionAwareODataPathRouteConstraint(pathHandler, model, routeName, routingConvetions);
            routes.Add(routeName, new ODataRoute(routePrefix, constraint));
        }

    }
}
