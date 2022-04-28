using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace NetcoreSaas.Application.Helpers
{
    public static class ImageHelper
    {
        public static byte[] ImageToByteArray(Image imageIn)
        {
            using var ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Gif);
            return ms.ToArray();
        }

        public static Bitmap ByteToImage(byte[] blob)
        {
            var stream = new MemoryStream();
            var pData = blob;
            stream.Write(pData, 0, Convert.ToInt32(pData.Length));
            var bm = new Bitmap(stream, false);
            stream.Dispose();
            return bm;
        }
        
        public static void CompressImage(string soucePath, string destPath, int quality)
        {
            using var bmp1 = new Bitmap(soucePath);
            var jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            var qualityEncoder = Encoder.Quality;

            var myEncoderParameters = new EncoderParameters(1);

            var myEncoderParameter = new EncoderParameter(qualityEncoder, quality);

            myEncoderParameters.Param[0] = myEncoderParameter;
            bmp1.Save(destPath, jpgEncoder, myEncoderParameters);
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
