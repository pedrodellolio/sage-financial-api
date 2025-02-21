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
    public class LabelService(AppDbContext context) : ILabelService
    {
        public async Task<Label?> GetAsync(Guid labelId)
        {
            return await context.Labels.FindAsync(labelId);
        }

        public async Task<ICollection<Label>> GetAllAsync(Guid profileId)
        {
            return await context.Labels.Where(l => l.ProfileId == profileId && l.IsActive).ToListAsync();
        }

        public async Task<Label> PostAsync(LabelDto request, Guid profileId)
        {
            var newLabel = new Label
            {
                Title = request.Title,
                ColorHex = request.ColorHex,
                IsDefault = request.IsDefault
            };

            context.Labels.Add(newLabel);
            await context.SaveChangesAsync();
            return newLabel;
        }

        public async Task<Label> PutAsync(Label label)
        {
            context.Labels.Update(label);
            await context.SaveChangesAsync();
            return label;
        }

        public async Task<bool> DeleteAsync(Label label)
        {
            context.Labels.Remove(label);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }
    }
}