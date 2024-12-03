using System.Text;
using System.Text.Json;

namespace OS.API.Utilities
{
    public static class ResponseUtilities
    {
        public static Dictionary<string, string> BuildError(string error)
        {
            return new Dictionary<string, string>
            {
                { "error", error }
            };
        }
        public static Dictionary<string, string> BuildWarning(string warning)
        {
            return new Dictionary<string, string>
            {
                { "warning", warning }
            };
        }
        public static Dictionary<string, string> BuildInfo(string message)
        {
            return new Dictionary<string, string>
            {
                { "info", message }
            };
        }
    }
}
