using System;
using System.Collections.Generic;
using System.Linq;

namespace Clee.Routing
{
    public interface IRouteFinder
    {
        Route FindRoute(Path input);
    }

    public class RouteEngine : IRouteFinder
    {
        private readonly ISet<Route> _routes = new HashSet<Route>();
        private readonly ICommandPathStrategy _commandPathStrategy;

        public RouteEngine(ICommandPathStrategy commandPathStrategy)
        {
            _commandPathStrategy = commandPathStrategy;
        }

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
            RegisterRouteFrom(commandType);
        }

        public void RegisterRouteFrom(Type commandType)
        {
            var commandMetaData = new CommandMetaData(commandType);
            var path = _commandPathStrategy.GeneratePathFor(commandMetaData);

            RegisterRoute(new Route(
                commandMetaData: commandMetaData,
                path: path
                ));
        }

        public Route FindRoute(Path input)
        {
            return _routes
                .Where(x => x.Path == input)
                .SingleOrDefault();
        }
    }
}