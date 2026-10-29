namespace OklahomaRentals.Server.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<IEnumerable<CloudinaryImageResult>> GetImagesByFolderAsync(string folder);
    }

    public class CloudinaryImageResult
    {
        public string PublicId { get; set; } = string.Empty;
        public string SecureUrl { get; set; } = string.Empty;
    }
}