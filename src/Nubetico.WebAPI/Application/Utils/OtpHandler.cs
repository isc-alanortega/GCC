using OtpNet;

namespace Nubetico.WebAPI.Application.Utils
{
    public static class OtpHandler
    {
        public static string GenerarSecret()
        {
            byte[] secret = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(secret);
        }

        public static bool TokenEsValido(string token, string secretKey)
        {
            var secret = Base32Encoding.ToBytes(secretKey);
            var totp = new Totp(secret);
            return totp.VerifyTotp(token, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
        }

        public static string GenerarUrl(string appName, string username, string secret)
        {
            // RFC 5234
            string encodedLabel = Uri.EscapeDataString($"{appName}:{username}");

            // RFC 3986
            string encodedIssuer = Uri.EscapeDataString(appName);

            return $"otpauth://totp/{encodedLabel}?secret={secret}&issuer={encodedIssuer}";
        }
    }
}
