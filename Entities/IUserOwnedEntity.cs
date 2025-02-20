namespace SageFinancialAPI.Entities
{
    public interface IUserOwnedEntity
    {
        Guid UserId { get; set; }
    }
}