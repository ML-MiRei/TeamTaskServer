using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Infrustructure.RepositoryImplementation
{
    public class NotificationRepository : INotificationRepository
    {
        public async Task<NotificationEntity> CreateNotification(List<int> usersId, string details, string title)
        {
            try
            {
                var notification = await Connections.NotificationServiceClient.CreateNotificationAsync(new CreateNotificationRequest
                {
                    Details = details,
                    Title = title,
                    UserId = { usersId }
                });

                return new NotificationEntity
                {
                    Detail = notification.Details,
                    Title = notification.Title,
                    ID = notification.NotificationId
                };
            }
            catch
            {
                throw new Exception();
            }
        }

        public async void DeleteNotification(int notificationId, int userId)
        {
            try
            {
                var notification = await Connections.NotificationServiceClient.DeleteNotificationAsync(new DeleteNotificationRequest { NotificationId = notificationId, UserId = userId });
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<List<NotificationEntity>> GetNotification(int userId)
        {
            try
            {
                var notifications = await Connections.NotificationServiceClient.GetListNotificationsAsync(new GetListNotificationsRequest { UserId = userId });

                return notifications.Notifications.Select(n => new NotificationEntity
                {
                    Detail = n.Details,
                    Title = n.Title,
                    ID = n.NotificationId

                }).ToList();
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
