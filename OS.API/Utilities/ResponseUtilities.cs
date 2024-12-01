using System.Text;
using System.Text.Json;

namespace OS.API.Utilities
{
    public static class ResponseUtilities
    {
        public static Dictionary<string, string> BuildErrorBody(string message)
        {
            Dictionary<string, string> error = new Dictionary<string, string> {
                    { "error", message }
                };
            return error;
        }
    }
}
