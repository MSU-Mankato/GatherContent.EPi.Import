using System;
using System.Net.Http;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Encoder = System.Drawing.Imaging.Encoder;

namespace GcEpiPluginV2._0.modules.GatherContentImport.GcEpiUtilities
{
    public class GcEpiImageExtractor
    {
        public static async Task<Stream> GetImageStreamAsync(string url)
        {
            var stream = new MemoryStream();

            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseStream = await response.Content.ReadAsStreamAsync();

                var img = Image.FromStream(responseStream);

                var encoder = GetEncoder(img.RawFormat);
                var encoderParameters = GetEncoderParameters();

                img.Save(stream, encoder, encoderParameters);

                stream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception)
            {
                stream = null;
            }

            return stream;
        }
        private static EncoderParameters GetEncoderParameters()
        {
            var qualityEncoder = Encoder.Quality;
            var encoderParameters = new EncoderParameters(1);
            var qualityEncoderParameter = new EncoderParameter(qualityEncoder, 100L);
            encoderParameters.Param[0] = qualityEncoderParameter;
            return encoderParameters;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo
                .GetImageDecoders()
                .FirstOrDefault(i => i.FormatID == format.Guid);
        }
    }
}