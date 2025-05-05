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
    public class ProfileService(AppDbContext context, IWalletService walletService) : IProfileService
    {
        public async Task<Profile?> GetAsync(Guid profileId)
        {
            return await context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId && p.IsActive);
        }

        public async Task<Profile?> GetDefaultAsync(Guid userId)
        {
            return await context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId && p.IsActive && p.IsDefault);
        }

        public async Task<Profile?> GetByTitleAsync(string title)
        {
            return await context.Profiles.FirstOrDefaultAsync(p => p.Title == title.Trim().ToUpper() && p.IsActive);
        }

        public async Task<ICollection<Profile>> GetAllAsync(Guid userId)
        {
            return await context.Profiles.Where(p => p.UserId == userId && p.IsActive).ToListAsync();
        }

        public async Task<ICollection<ProfileBalanceDto>> GetAllProfileBalanceAsync(int month, int year, Guid userId)
        {
            var profiles = await GetAllAsync(userId);
            var profilesBalance = new List<ProfileBalanceDto>();
            foreach (var profile in profiles)
            {
                var wallet = await walletService.GetByMonthAndYearAsync(month, year, profile.Id);
                var balance = (wallet?.IncomesBrl ?? 0) - (wallet?.ExpensesBrl ?? 0);
                var profileBalance = new ProfileBalanceDto() { Profile = profile, Balance = balance };
                profilesBalance.Add(profileBalance);
            }
            return profilesBalance;
        }

        public async Task<Profile> PostAsync(ProfileDto request, Guid userId)
        {
            var newProfile = new Profile
            {
                Title = request.Title,
                UserId = userId,
                IsDefault = request.IsDefault
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
            if (profile.IsDefault)
                throw new ApplicationException("Não é possível desativar perfis padrões.");
            profile.IsActive = false;
            context.Profiles.Update(profile);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }
    }
}