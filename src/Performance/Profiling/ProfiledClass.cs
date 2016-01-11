using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Framework.Integration;

namespace Profiling
{
    public class ProfiledClass
    {
        private object _instance;

        public Type Type { get; set; }

        public IEnumerable<ProfiledMethod> Methods { get; set; }

        public ProfiledClass(Type type, IEnumerable<ProfiledMethod> methods)
        {
            Type = type;
            Methods = methods;
        }

        public object CreateInstance(ClusterBase cluster)
        {
            if (_instance == null)
            {
                var constructors = Type.GetConstructors();

                var clusterConstructor = constructors.FirstOrDefault(c =>
                {
                    var parameters = c.GetParameters();
                    return parameters.Length == 1 &&
                           typeof(IIntegrationCluster).IsAssignableFrom(parameters[0].ParameterType);
                });

                _instance = clusterConstructor != null
                    ? Activator.CreateInstance(Type, cluster)
                    : Activator.CreateInstance(Type);
            }

            return _instance;
        }
    }
}