namespace Trestle.Utils
{
    public struct Endpoints
    {
        public struct Mojang
        {
            private const string BASE = "https://api.mojang.com/";

            public static string GetUuid(string username)
                => $"{BASE}users/profiles/minecraft/{username}";
        }
    }
}