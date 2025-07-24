namespace ReconocimientoFacialBlazor.Services
{
    public interface IFaceRecognitionService
    {
        Task<float> CompareAsync(Stream image1, Stream image2);
    }
}
