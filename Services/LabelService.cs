using Microsoft.EntityFrameworkCore;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Extensions;
using SageFinancialAPI.Generators;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public class LabelService(AppDbContext context) : ILabelService
    {
        public async Task<Label?> GetAsync(Guid labelId)
        {
            return await context.Labels.FirstOrDefaultAsync(l => l.Id == labelId && l.IsActive);
        }

        public async Task<Label?> GetByTitleAsync(string title)
        {
            return await context.Labels.FirstOrDefaultAsync(l => l.Title == title.ToUpper() && l.IsActive);
        }

        public async Task<ICollection<LabelDto>> GetAllNotInBudgetGoalAsync(int month, int year, Guid profileId)
        {
            return await context.Labels
                .Where(label =>
                    label.ProfileId == profileId &&
                    label.IsActive &&
                    !label.BudgetGoals.Any(bg =>
                        bg.Budget.Month == month &&
                        bg.Budget.Year == year &&
                        bg.Budget.ProfileId == profileId))
                .Select(label => label.ToDto())
                .ToListAsync();
        }


        public async Task<ICollection<LabelDto>> GetAllAsync(Guid profileId)
        {
            return await context.Labels
                .Include(l => l.Profile)
                .Where(l => l.ProfileId == profileId && l.IsActive)
                .Select(label => label.ToDto()).ToListAsync();
        }

        public async Task<Label> PostAsync(LabelDto request, Guid profileId)
        {
            var newLabel = new Label
            {
                Title = request.Title,
                ColorHex = ColorGenerator.GenerateDarkModeColor(),
                ProfileId = profileId
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
            if (label.IsDefault)
                throw new ApplicationException("Não é possível desativar categorias padrões.");

            label.IsActive = false;
            label.IsActive = false;
            context.Labels.Update(label);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }
    }
}