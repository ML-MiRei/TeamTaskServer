using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Notification.Commands.DeleteNotification
{
    public class DeleteNotificationCommand : IRequest
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }

    }
}
