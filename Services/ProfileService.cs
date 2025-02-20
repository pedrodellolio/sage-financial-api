using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public class ProfileService : BaseService, IProfileService
    {
        public ProfileService(IHttpContextAccessor httpContextAccessor, AppDbContext context)
            : base(httpContextAccessor, context)
        {
        }

        public async Task<Profile?> GetAsync(Guid profileId)
        {
            return await GetEntityByUserAsync<Profile>(profileId);
        }

        public async Task<ICollection<Profile>> GetAllAsync()
        {
            return await GetAllEntitiesByUserAsync<Profile>();
        }

        public async Task<Profile?> GetByTitleAsync(string title)
        {
            return await GetEntityByUserAndPropertyAsync<Profile>(p => p.Title == title);
        }

        public async Task<Profile> PostAsync(ProfileDto request)
        {
            var newProfile = new Profile
            {
                Title = request.Title,
            };

            return await CreateEntityAsync(newProfile);
        }

        public async Task<Profile?> PutAsync(Profile request)
        {
            return await UpdateEntityAsync(request);
        }

        public async Task<bool> DeleteAsync(Guid profileId)
        {
            return await DeleteEntityAsync<Profile>(profileId);
        }
    }
}