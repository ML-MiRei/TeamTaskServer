using GreatDatabase.Data;
using GreatDatabase.Data.Model;
using Grpc.Core;
using NotificationService;

namespace NotificationService.Services
{
    public class NotificationApiService : NotificationService.NotificationServiceBase
    {
        private static MyDbContext db;
        private static ILogger<NotificationApiService> _logger;
        public NotificationApiService(ILogger<NotificationApiService> logger)
        {
            _logger = logger;
            db = new MyDbContext();
        }

        public async override Task<NotificationReply> CreateNotification(CreateNotificationRequest request, ServerCallContext context)
        {
            try
            {
                Notification notification = new Notification()
                {
                    Details = request.Details,
                    Title = request.Title,
                };

                await db.Notifications.AddAsync(notification);

                await db.SaveChangesAsync();

                _logger.LogInformation($"Create notification '{notification.Title}'");

                foreach (int userId in request.UserId)
                {
                    await Console.Out.WriteLineAsync(userId + "");
                    await db.Notifications_Users.AddAsync(new Notification_User { UserId = userId, NotificationId = notification.ID });
                }

                await db.SaveChangesAsync();



                return new NotificationReply
                {
                    Details = request.Details,
                    Title = request.Title,
                    NotificationId = notification.ID
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.Message);
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Create db error"));
            }
        }

        public async override Task<VoidNotificationReply> DeleteNotification(DeleteNotificationRequest request, ServerCallContext context)
        {
            try
            {
                Notification_User notificationUser = db.Notifications_Users.First(n => n.NotificationId == request.NotificationId && n.UserId == request.UserId);

                db.Notifications_Users.Remove(notificationUser);
                await db.SaveChangesAsync();

                _logger.LogInformation($"Delete notification with id = {notificationUser.NotificationId} for user with id = {request.UserId}");


                if (db.Notifications_Users.Any(u => u.NotificationId == request.NotificationId))
                {
                    Notification notification = db.Notifications.First(n => n.ID ==  request.NotificationId);
                    db.Notifications.Remove(notification);

                    await db.SaveChangesAsync();

                    _logger.LogInformation($"Delete notification with id = {notificationUser.NotificationId}");
                }


                return new VoidNotificationReply();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Create db error"));
            }
        }

        public async override Task<GetListNotificationsReply> GetListNotifications(GetListNotificationsRequest request, ServerCallContext context)
        {
            GetListNotificationsReply reply = new GetListNotificationsReply();
            List<NotificationReply> notificationReplies = (from n in db.Notifications
                                                           join nu in db.Notifications_Users on n.ID equals nu.NotificationId
                                                           where nu.UserId == request.UserId
                                                           select new NotificationReply
                                                           {
                                                               Details = n.Details,
                                                               NotificationId = n.ID,
                                                               Title = n.Title

                                                           }).ToList();

            reply.Notifications.AddRange(notificationReplies);

            _logger.LogInformation($"Return {notificationReplies.Count} notifications");

            return reply;

        }
    }
}
