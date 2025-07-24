using System;
using System.IO;
using System.Threading.Tasks;

namespace ReconocimientoFacialBlazor.Services
{
    public class MockFaceRecognitionService : IFaceRecognitionService
    {
        private static readonly Random _random = new();

        public Task<float> CompareAsync(Stream image1, Stream image2)
        {
            // Retorna un valor aleatorio entre 0.3 y 0.99 (simulación de similitud)
            float score = (float)(_random.NextDouble() * 0.69 + 0.3);
            return Task.FromResult(score);
        }
    }
}
