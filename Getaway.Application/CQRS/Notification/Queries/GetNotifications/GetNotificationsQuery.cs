using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Notification.Commands.GetNotifications
{
    public class GetNotificationsQuery : IRequest<List<NotificationEntity>>
    {
        public int UserId { get; set; }

    }
}
