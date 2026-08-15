using System.Security.Cryptography;

namespace Axlon.Services.Basic.Helper
{
    public static class SceneHelper
    {
        private const string Chars =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string Generate()
        {
            Span<char> result = stackalloc char[6];

            for (var i = 0; i < result.Length; i++)
            {
                result[i] =
                    Chars[RandomNumberGenerator.GetInt32(Chars.Length)];
            }

            return new string(result);
        }
    }
}
