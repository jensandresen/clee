using System;

namespace Clee.Tests
{
    public static class ExceptionHelper
    {
        public static T Grab<T>(Action action) where T : Exception
        {
            T exceptionThrown = null;

            try
            {
                action();
            }
            catch (T err)
            {
                exceptionThrown = err;
            }

            return exceptionThrown;
        }
    }
}