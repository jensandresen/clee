using System.Collections.Generic;

namespace Clee.Tests
{
    public class RouteEngine
    {
        private readonly ISet<Route> _routes = new HashSet<Route>();

        public IEnumerable<Route> Routes
        {
            get { return _routes; }
        }

        public void RegisterRoute(Route route)
        {
            _routes.Remove(route);
            _routes.Add(route);
        }

        public void RegisterRouteFrom<T>() where T : Command
        {
            var commandType = typeof (T);
            RegisterRoute(new Route(commandType));
        }
    }
}