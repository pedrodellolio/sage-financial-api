using System.Text.RegularExpressions;

namespace SageFinancialAPI.Validators
{
    public static partial class ExpoPushTokenValidator
    {
        private static readonly Regex ExpoPushTokenRegex = ExpoToken();

        public static bool IsValidExpoPushToken(string token)
        {
            return ExpoPushTokenRegex.IsMatch(token);
        }

        [GeneratedRegex(@"^ExponentPushToken\[[a-zA-Z0-9]+\]$", RegexOptions.Compiled)]
        private static partial Regex ExpoToken();
    }
}