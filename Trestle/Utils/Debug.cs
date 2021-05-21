namespace Trestle.Utils
{
    public static class Debug
    {
        /// <summary>
        /// Whether or not the current instance of Trestle is a debug build.
        /// </summary>
        public static bool IsDebugBuild()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}