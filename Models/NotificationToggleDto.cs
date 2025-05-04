using SageFinancialAPI.Entities;

namespace SageFinancialAPI.Models
{
    public class NotificationToggleDto
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; }

    }
}