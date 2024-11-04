using QRCoder;
using System.Drawing;
using System.Net.Http;

namespace FrontEndWeb.Configurations
{

    public interface IQRCodeService
    {
        Bitmap QRCodeGenerateImage(string url);

        byte[] QRCodeImageToByteArray(Bitmap img);
    }



    public class QRCodeService : IQRCodeService
    {
        public QRCodeService()
        { }



        /// <summary>
        /// Creates a BitMap (image of the QRCode) with the url. 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Bitmap QRCodeGenerateImage(string url)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(10);
            return qrCodeImage;
        }

        /// <summary>
        /// Transforms a BitMap (image) into Array of Bytes
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public byte[] QRCodeImageToByteArray(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}
