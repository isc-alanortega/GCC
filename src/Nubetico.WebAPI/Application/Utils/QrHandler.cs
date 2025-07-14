using QRCoder;
using System.Drawing;

namespace Nubetico.WebAPI.Application.Utils
{
    public static class QrHandler
    {
        public static string QrBase64(string content)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                byte[] qrCodeImage = qrCode.GetGraphic(10, Color.Black, Color.White);

                string img = Convert.ToBase64String(qrCodeImage, 0, qrCodeImage.Length);
                return string.Format("data:image/png;base64,{0}", img);
            }
        }
    }
}
