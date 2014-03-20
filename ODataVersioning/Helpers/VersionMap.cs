using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.OData;
using System.Web.Http.OData.Builder;

namespace ODataVersioning.Helpers
{
    public static class VersionMap
    {
        public const string VersionHeaderName = "x-api-version";

        private static readonly Dictionary<KeyValuePair<int, string>, string> Map = new Dictionary<KeyValuePair<int, string>, string>();

        private static int _currentVersion = -1;

        public static int CurrentVersion
        {
            get
            {
                if (_currentVersion < 0 && Map.Any())
                {
                    _currentVersion = Map.Keys.Select(x => x.Key).Max();
                }
                return _currentVersion;
            }
            set { _currentVersion = value; }
        }

        private static int _minVersion = -1;

        public static int MinVersion
        {
            get
            {
                if (_minVersion < 0 && Map.Any())
                {
                    _minVersion = Map.Keys.Select(x => x.Key).Min();
                }
                return _minVersion;
            }
        }

        public static string GetEndpointName(int version, string endpoint)
        {
            string targetEndpoint = null;
            Map.TryGetValue(new KeyValuePair<int, string>(version, endpoint), out targetEndpoint);
            return targetEndpoint;
        }

        public static ODataEntityVersionMapBuilder ForEndpoint(this ODataConventionModelBuilder modelBuilder, string endpointName)
        {
            return new ODataEntityVersionMapBuilder(modelBuilder, endpointName);
        }

        private static void MapVersionEntitySetConfig(int version, string endpoint, string entitySetName)
        {
            Map.Add(new KeyValuePair<int, string>(version, endpoint), entitySetName);
        }

        public class ODataEntityVersionMapBuilder
        {

            private readonly ODataConventionModelBuilder _modelBuilder;

            private readonly string _endpointName;

            private List<Tuple<int, string, Type>> _entityVersionTypeMap = new List<Tuple<int, string, Type>>();

            public ODataEntityVersionMapBuilder(ODataConventionModelBuilder modelBuilder, string endpointName)
            {
                _modelBuilder = modelBuilder;
                _endpointName = endpointName;
            }

            public EntityVersionSetter OnVersion(int version)
            {
                return new EntityVersionSetter(this, version);
            }

            public EntityVersionSetter OnVersionRange(int minVersion, int maxVersion)
            {
                return new EntityVersionSetter(this, minVersion, maxVersion);
            }

            public ODataConventionModelBuilder Build()
            {
                foreach (var tpl in _entityVersionTypeMap.OrderBy(tpl => tpl.Item1))
                {
                    var version = tpl.Item1;
                    var entitySetName = tpl.Item2;
                    var entityType = tpl.Item3;

                    _modelBuilder.AddEntitySet(entitySetName, _modelBuilder.AddEntity(entityType));
                    MapVersionEntitySetConfig(version, _endpointName, entitySetName);
                }
                return _modelBuilder;
            }

            public class EntityVersionSetter
            {

                private readonly ODataEntityVersionMapBuilder _builder;

                private readonly int _minVersion;

                private readonly int? _maxVersion;

                private Type _entityType;

                public EntityVersionSetter(ODataEntityVersionMapBuilder builder, int minVersion, int? maxVersion = null)
                {
                    _builder = builder;
                    _minVersion = minVersion;
                    _maxVersion = maxVersion;
                }

                public EntityVersionSetter MapEntitySet<TEntityType>()
                    where TEntityType : class
                {
                    _entityType = typeof (TEntityType);
                    return this;
                }

                public ODataEntityVersionMapBuilder ToController<TControllerType>()
                    where TControllerType : ODataController
                {
                    const string controllerSuffix = "Controller";
                    var controllerName = typeof (TControllerType).Name;
                    if (controllerName.EndsWith(controllerSuffix))
                    {
                        controllerName = controllerName.Replace(controllerSuffix, string.Empty);
                    }

                    var minVersion = _minVersion;
                    var maxVersion = _maxVersion ?? _minVersion;
                    for (var version = minVersion; version < (maxVersion + 1); version++)
                    {
                        _builder._entityVersionTypeMap.Add(Tuple.Create(version, controllerName, _entityType));
                    }
                    return _builder;
                }

            }
        }
        
    }
}