using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Notification.Commands.CreateNotification
{
    public class CreateNotificationCommand : IRequest<NotificationEntity>
    {
        public List<int> UserId { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
    }
}
