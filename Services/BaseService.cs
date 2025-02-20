using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;

public abstract class BaseService
{
    protected readonly AppDbContext _context;
    protected readonly IHttpContextAccessor _httpContextAccessor;

    protected BaseService(IHttpContextAccessor httpContextAccessor, AppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    protected ClaimsPrincipal? CurrentUser => _httpContextAccessor.HttpContext?.User;

    protected Guid GetUserId()
    {
        var userIdClaim = CurrentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null ? Guid.Parse(userIdClaim) : Guid.Empty;
    }

    protected async Task<T?> GetEntityByUserAsync<T>(Guid entityId) where T : BaseEntity, IUserOwnedEntity
    {
        var userId = GetUserId();
        return await _context.Set<T>().FirstOrDefaultAsync(e => e.Id == entityId && e.UserId == userId);
    }

    protected async Task<List<T>> GetAllEntitiesByUserAsync<T>() where T : BaseEntity, IUserOwnedEntity
    {
        var userId = GetUserId();
        return await _context.Set<T>().Where(e => e.UserId == userId).ToListAsync();
    }

    protected async Task<T?> GetEntityByUserAndPropertyAsync<T>(
        Expression<Func<T, bool>> propertyPredicate) where T : class, IUserOwnedEntity
    {
        var userId = GetUserId();
        return await _context.Set<T>()
            .Where(e => e.UserId == userId)
            .Where(propertyPredicate)
            .FirstOrDefaultAsync();
    }

    // Método para criar uma nova entidade associada ao usuário atual
    protected async Task<T> CreateEntityAsync<T>(T entity) where T : BaseEntity, IUserOwnedEntity
    {
        entity.UserId = GetUserId();
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    // Método para atualizar uma entidade existente do usuário atual
    protected async Task<T?> UpdateEntityAsync<T>(T entity) where T : BaseEntity, IUserOwnedEntity
    {
        var userId = GetUserId();
        var existingEntity = await _context.Set<T>().AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entity.Id && e.UserId == userId);

        if (existingEntity is null)
            return null;

        entity.UserId = userId; // Garantir que o UserId não seja alterado
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    // Método para deletar uma entidade do usuário atual
    protected async Task<bool> DeleteEntityAsync<T>(Guid entityId) where T : BaseEntity, IUserOwnedEntity
    {
        var userId = GetUserId();
        var entity = await _context.Set<T>()
            .FirstOrDefaultAsync(e => e.Id == entityId && e.UserId == userId);

        if (entity == null)
            return false;

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
