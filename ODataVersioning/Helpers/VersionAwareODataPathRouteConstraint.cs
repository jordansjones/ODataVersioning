using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData.Routing;
using System.Web.Http.OData.Routing.Conventions;
using System.Web.Http.Routing;

using Microsoft.Data.Edm;
using Microsoft.Data.OData;

namespace ODataVersioning.Helpers
{
    public class VersionAwareODataPathRouteConstraint : ODataPathRouteConstraint
    {

        public VersionAwareODataPathRouteConstraint(VersionAwareODataPathHandler pathHandler, IEdmModel model, string routeName, IEnumerable<IODataRoutingConvention> routingConventions) : base(pathHandler, model, routeName, routingConventions)
        {
            VersionAwarePathHandler = pathHandler;
        }

        public VersionAwareODataPathHandler VersionAwarePathHandler { get; private set; }

        public override bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (routeDirection != HttpRouteDirection.UriResolution)
            {
                return true;
            }

            object odataPathValue;
            if (!values.TryGetValue(ODataRouteConstants.ODataPath, out odataPathValue))
            {
                return false;
            }

            var pathString = odataPathValue as string;
            if (pathString == null) pathString = string.Empty;

            ODataPath path = null;
            try
            {
                path = VersionAwarePathHandler.Parse(request, EdmModel, pathString);
            }
            catch (ODataException e)
            {
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid OData path", e));
            }

            if (path != null)
            {
                values[ODataRouteConstants.ODataPath] = path.Segments.First();
                // Set all the properties we need for routing, querying, formatting
                request.SetEdmModel(EdmModel);
                request.SetODataPathHandler(VersionAwarePathHandler);
                request.SetODataPath(path);
                request.SetODataRouteName(RouteName);
                request.SetODataRoutingConventions(RoutingConventions);

                if (!values.ContainsKey(ODataRouteConstants.Controller))
                {
                    // Select controller name using the routing conventions
                    string controllerName = SelectControllerName(path, request);
                    if (controllerName != null)
                    {
                        values[ODataRouteConstants.Controller] = controllerName;
                    }
                }
            }

            return path != null;
        }

    }
}