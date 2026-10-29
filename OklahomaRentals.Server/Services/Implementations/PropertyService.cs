using OklahomaRentals.Server.API.Response;
using OklahomaRentals.Server.Services.Interfaces;
using OklahomaRentals.Server.Repositories.Interfaces;
using System;
using OklahomaRentals.Server.API.Requests;

namespace OklahomaRentals.Server.Services.Implementations
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _repository;

        public PropertyService(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PropertyResponse>> GetAllAsync()
        {
            var properties = await _repository.GetAllAsync();

            // Map entities to response DTOs
            return properties.Select(p => new PropertyResponse
            {
                Id = p.Id,
                Name = p.Name,
                Campus = p.Campus,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Price = p.Price,
                Description = p.Description,
                IsLeased = p.IsLeased,
                Address = p.Address,
                NextAvailable = p.NextAvailable
            });
        }

        public async Task<PropertyResponse?> GetByIdAsync(int id)
        {
            var property = await _repository.GetByIdAsync(id);

            if (property == null)
                return null;

            return new PropertyResponse
            {
                Id = property.Id,
                Name = property.Name,
                Campus = property.Campus,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                Price = property.Price,
                Description = property.Description,
                IsLeased = property.IsLeased,
                Address = property.Address,
                NextAvailable = property.NextAvailable
            };
        }

        public async Task<PropertyResponse> CreateAsync(PropertyRequest property)
        {
            var entity = new Data.Entities.Property
            {
                Name = property.Name,
                Campus = property.Campus,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                Price = property.Price,
                Description = property.Description,
                IsLeased = false,
                Address = property.Address,
                AdminId = property.AdminId,
                NextAvailable = property.NextAvailable
            };
            var created = await _repository.CreateAsync(entity);
            return new PropertyResponse
            {
                Id = created.Id,
                Name = created.Name,
                Campus = created.Campus,
                Bedrooms = created.Bedrooms,
                Bathrooms = created.Bathrooms,
                Price = created.Price,
                Description = created.Description,
                IsLeased = created.IsLeased,
                Address = created.Address,
                NextAvailable = created.NextAvailable
            };
        }

        public async Task<PropertyResponse?> UpdateAsync(int id, PropertyRequest property)
        {
            var p = await _repository.GetByIdAsync(id);
            if (p == null)
                return null;

            p.Name = property.Name;
            p.Campus = property.Campus;
            p.Bedrooms = property.Bedrooms;
            p.Bathrooms = property.Bathrooms;
            p.Price = property.Price;
            p.Description = property.Description;
            p.Address = property.Address;
            p.AdminId = property.AdminId;

            var updated = await _repository.UpdateAsync(p);

            return new PropertyResponse
            {
                Id = updated.Id,
                Name = updated.Name,
                Campus = updated.Campus,
                Bedrooms = updated.Bedrooms,
                Bathrooms = updated.Bathrooms,
                Price = updated.Price,
                Description = updated.Description,
                IsLeased = updated.IsLeased,
                Address = updated.Address,
                NextAvailable = updated.NextAvailable
            };
        }

        public async Task<PropertyResponse> CreateFillerTempProperty()
        {
            var entity = new Data.Entities.Property
            {
                Name = "Filler Property",
                Campus = "Main",
                Bedrooms = 3,
                Bathrooms = 2,
                Price = 1200.00m,
                Description = "A nice filler property.",
                IsLeased = false,
                Address = "123 Filler St",
                AdminId = 123 
            };
            var created = await _repository.CreateAsync(entity);
            return new PropertyResponse
            {
                Id = created.Id,
                Name = created.Name,
                Campus = created.Campus,
                Bedrooms = created.Bedrooms,
                Bathrooms = created.Bathrooms,
                Price = created.Price,
                Description = created.Description,
                IsLeased = created.IsLeased,
                Address = created.Address,
                NextAvailable = created.NextAvailable
            };
        }

        public async Task<PropertyResponse?> UpdateDescriptionAsync(int id, string description)
        {
            var p = await _repository.GetByIdAsync(id);
            if (p == null)
                return null;

            p.Description = description;

            var updated = await _repository.UpdateAsync(p);

            return new PropertyResponse
            {
                Id = updated.Id,
                Name = updated.Name,
                Campus = updated.Campus,
                Bedrooms = updated.Bedrooms,
                Bathrooms = updated.Bathrooms,
                Price = updated.Price,
                Description = updated.Description,
                IsLeased = updated.IsLeased,
                Address = updated.Address,
                NextAvailable = updated.NextAvailable
            };
        }

        public async Task<PropertyResponse?> UpdateAvailabilityAsync(int id, bool isLeased)
        {
            var p = await _repository.GetByIdAsync(id);
            if (p == null)
                return null;

            p.IsLeased = isLeased;

            var updated = await _repository.UpdateAsync(p);

            return new PropertyResponse
            {
                Id = updated.Id,
                Name = updated.Name,
                Campus = updated.Campus,
                Bedrooms = updated.Bedrooms,
                Bathrooms = updated.Bathrooms,
                Price = updated.Price,
                Description = updated.Description,
                IsLeased = updated.IsLeased,
                Address = updated.Address,
                NextAvailable = updated.NextAvailable
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<PropertyResponse?> GetByNameAsync(string name)
        {
            var properties = await _repository.GetAllAsync();
            var property = properties.FirstOrDefault(p =>
                p.Name.ToLower() == name.ToLower());

            if (property == null)
                return null;

            return new PropertyResponse
            {
                Id = property.Id,
                Name = property.Name,
                Campus = property.Campus,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                Price = property.Price,
                Description = property.Description,
                IsLeased = property.IsLeased,
                Address = property.Address,
                NextAvailable = property.NextAvailable
            };
        }
    }
}