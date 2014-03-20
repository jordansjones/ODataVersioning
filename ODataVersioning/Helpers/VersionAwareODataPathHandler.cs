using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData.Routing;

using Microsoft.Data.Edm;

namespace ODataVersioning.Helpers
{
    public class VersionAwareODataPathHandler : DefaultODataPathHandler
    {

        public ODataPath Parse(HttpRequestMessage request, IEdmModel model, string odataPath)
        {
            var oldEntityName = ParseSegments(odataPath).First();
            var targetEndpoint = ProcessVersionHeader(request, oldEntityName);
            odataPath = odataPath.Replace(oldEntityName, targetEndpoint);
            return base.Parse(model, odataPath);
        }

        protected virtual string ProcessVersionHeader(HttpRequestMessage request, string odataPath)
        {
            var apiVersion = request.Headers.Where(kv => kv.Key.Equals(VersionMap.VersionHeaderName, StringComparison.InvariantCultureIgnoreCase)).SelectMany(kv => kv.Value).FirstOrDefault();

            int version = -1;
            if (string.IsNullOrWhiteSpace(apiVersion))
            {
                version = VersionMap.CurrentVersion;
            }
            else if (!int.TryParse(apiVersion, out version))
            {
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Api Version specified: " + apiVersion));
            }

            if (version < VersionMap.MinVersion || version > VersionMap.CurrentVersion) goto UnsupportedVersion;

            var endpoint = odataPath;

            var targetEndpoint = VersionMap.GetEndpointName(version, endpoint);
            if (targetEndpoint == null) goto UnsupportedVersion;

            return targetEndpoint;
        UnsupportedVersion:
            throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotAcceptable, string.Format("Unsupported Api Version specified: {0}. Current version: {1}",  version, VersionMap.CurrentVersion)));

        }

    }
}
