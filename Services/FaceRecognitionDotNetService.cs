using FaceRecognitionDotNet;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReconocimientoFacialBlazor.Services
{
    public class FaceRecognitionDotNetService : IFaceRecognitionService
    {
        private readonly FaceRecognition _fr;

        public FaceRecognitionDotNetService()
        {
            // Cambia la ruta si es necesario
            _fr = FaceRecognition.Create("models");
        }

        public async Task<float> CompareAsync(Stream image1, Stream image2)
        {
            var imgA = await LoadImageFromStream(image1);
            var imgB = await LoadImageFromStream(image2);
            // Carga imágenes
           // var imgA = FaceRecognition.LoadImage(await ToByteArray(image1));
           // var imgB = FaceRecognition.LoadImage(await ToByteArray(image2));

            // Obtén encodings
            var encA = _fr.FaceEncodings(imgA).FirstOrDefault();
            var encB = _fr.FaceEncodings(imgB).FirstOrDefault();

            if (encA == null || encB == null)
                return 0; // No se detectó rostro

            // Calcula distancia (menor = más parecido) 
            double distance = FaceRecognition.FaceDistance(encA, encB);

            // Escala a similitud (1=igual, 0=diferente)
            float similarity = 1.0f - (float)distance;
            return similarity;
        }

        private async Task<byte[]> ToByteArray(Stream input)
        {
            using var ms = new MemoryStream();
            await input.CopyToAsync(ms);
            return ms.ToArray();
        }

        private async Task<Image> LoadImageFromStream(Stream stream)
        {
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            using var bmp = new System.Drawing.Bitmap(new MemoryStream(ms.ToArray()));

            var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            var bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, rgbValues, 0, bytes);

            bmp.UnlockBits(bmpData);

            Mode mode = bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed ? Mode.Greyscale : Mode.Rgb;

            return FaceRecognition.LoadImage(rgbValues, bmp.Height, bmp.Width, Math.Abs(bmpData.Stride), mode);
        }
    }
}
