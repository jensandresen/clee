using System;

namespace Clee.Tests.Helpers
{
    public static class ExceptionHelper
    {
        public static ExceptionGrabber From(Action action)
        {
            return new ExceptionGrabber(action);
        }

        public class ExceptionGrabber
        {
            private readonly Action _action;

            public ExceptionGrabber(Action action)
            {
                _action = action;
            }

            public T Grab<T>() where T : Exception
            {
                T exceptionThrown = null;

                try
                {
                    _action();
                }
                catch (T err)
                {
                    exceptionThrown = err;
                }

                return exceptionThrown;
            }
        }
    }
}