using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Getaway.Application.CQRS.Notification.Commands.GetNotifications
{
    public class GetNotificationsHandler(INotificationRepository notificationRepository) : IRequestHandler<GetNotificationsQuery, List<NotificationEntity>>
    {
        public async Task<List<NotificationEntity>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var notifications = await notificationRepository.GetNotification(request.UserId);
                return notifications.Select(n => new NotificationEntity
                {
                    Detail = n.Detail,
                    ID = n.ID,
                    Title = n.Title

                }).ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
