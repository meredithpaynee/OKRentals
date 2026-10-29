using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using OklahomaRentals.Server.Services.Interfaces;

namespace OklahomaRentals.Server.Services.Implementations
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<IEnumerable<CloudinaryImageResult>> GetImagesByFolderAsync(string folder)
        {
            // Search for all image resources inside the folder (use wildcard)
            // Quote the folder since it may contain spaces or special chars
            var expression = $"folder:\"{folder}/*\" AND resource_type:image";

            var results = new List<CloudinaryImageResult>();
            string nextCursor = null;

            do
            {
                var searchBuilder = _cloudinary.Search()
                    .Expression(expression)
                    .MaxResults(100);

                if (!string.IsNullOrEmpty(nextCursor))
                    searchBuilder = searchBuilder.NextCursor(nextCursor);

                var result = await searchBuilder.ExecuteAsync();

                if (result.Error != null)
                    throw new Exception(result.Error.Message);

                if (result.Resources != null)
                {
                    results.AddRange(result.Resources.Select(r => new CloudinaryImageResult
                    {
                        PublicId = r.PublicId,
                        SecureUrl = r.SecureUrl?.ToString() ?? string.Empty
                    }));
                }

                nextCursor = result.NextCursor;
            } while (!string.IsNullOrEmpty(nextCursor));

            return results;
        }
    }
}