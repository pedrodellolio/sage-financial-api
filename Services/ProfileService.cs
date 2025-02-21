using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public class ProfileService(AppDbContext context) : IProfileService
    {
        public async Task<Profile?> GetAsync(Guid profileId)
        {
            return await context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId && p.IsActive);
        }

        public async Task<Profile?> GetByTitleAsync(string title)
        {
            return await context.Profiles.FirstOrDefaultAsync(p => p.Title.Equals(title.Trim(), StringComparison.CurrentCultureIgnoreCase) && p.IsActive);
        }

        public async Task<ICollection<Profile>> GetAllAsync(Guid userId)
        {
            return await context.Profiles.Where(p => p.UserId == userId && p.IsActive).ToListAsync();
        }

        public async Task<Profile> PostAsync(ProfileDto request, Guid userId)
        {
            var newProfile = new Profile
            {
                Title = request.Title.Trim().ToUpper(),
                UserId = userId
            };

            context.Profiles.Add(newProfile);
            await context.SaveChangesAsync();
            return newProfile;
        }

        public async Task<Profile> PutAsync(Profile profile)
        {
            context.Profiles.Update(profile);
            await context.SaveChangesAsync();
            return profile;
        }

        public async Task<bool> DeleteAsync(Profile profile)
        {
            context.Profiles.Remove(profile);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }
    }
}