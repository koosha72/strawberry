namespace Strawberry.Core
{
    public static class ExceptionHelper
    {
        public static bool ShowExceptions = true;

        public static event EventHandler<Exception> ExceptionCaught = null;

        public static void Throw(object thrower, Exception e)
        {
            if (ShowExceptions)
                throw e;
            else
            {
                if (ExceptionCaught != null)
                {
                    ExceptionCaught(thrower, e);
                }
            }
        }
    }
}
