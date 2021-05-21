namespace Trestle.Utils
{
    public static class Debug
    {
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