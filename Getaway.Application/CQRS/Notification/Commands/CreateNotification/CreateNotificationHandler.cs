using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Getaway.Application.CQRS.Notification.Commands.CreateNotification
{
    public class CreateNotificationHandler(INotificationRepository notificationRepository) : IRequestHandler<CreateNotificationCommand, NotificationEntity>
    {
        public async Task<NotificationEntity> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var notification = await notificationRepository.CreateNotification(request.UserId, request.Details, request.Title);
                return new NotificationEntity
                {
                    Detail = notification.Detail,
                    ID = notification.ID,
                    Title = notification.Title
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
