using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Getaway.Application.CQRS.Notification.Commands.DeleteNotification
{
    public class DeleteNotificationHandler(INotificationRepository notificationRepository) : IRequestHandler<DeleteNotificationCommand>
    {
        public async Task Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                 notificationRepository.DeleteNotification(request.NotificationId, request.UserId);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
