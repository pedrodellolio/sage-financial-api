using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public interface INotificationService
    {
        Task<Notification?> GetAsync(Guid notificationId);
        Task<Notification?> GetAsync(Guid transactionId, Guid profileId);
        Task<ICollection<Notification>> GetAllAsync(Guid profileId);
        Task<Notification> PutAsync(Notification notification);
        Task<Notification> PostAsync(NotificationDto request, Guid profileId);
        Task<bool> DeleteAsync(Notification notification);
        Task SendNotificationAsync(string expoPushToken, string title, string message);
        Task SendNotificationAsync(Guid userId, string title, string message);
        Task SendNotificationByProfileAsync(Guid profileId, string title, string message);
    }
}