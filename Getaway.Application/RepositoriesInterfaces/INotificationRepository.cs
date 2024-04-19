using Getaway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.RepositoriesInterfaces
{
    public interface INotificationRepository
    {
        Task<List<NotificationEntity>> GetNotification(int userId);

        Task<NotificationEntity> CreateNotification(List<int> usersId, string details, string title);
        void DeleteNotification(int notificationId, int userId);

    }
}
