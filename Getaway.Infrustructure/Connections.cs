using Getaway.Core.Exceptions;
using Grpc.Net.Client;
using MessengerService;

namespace Getaway.Infrustructure
{
    public static class Connections
    {
        
        
        private static AuthenticationService.AuthenticationServiceClient _authenticationServiceClient;
        private static UserService.UserServiceClient _userServiceClient;
        private static TeamService.TeamServiceClient _teamServiceClient;
        private static NotificationService.NotificationServiceClient _notificationServiceClient;

        private static ProjectService.ProjectServiceClient _projectServiceClient;
        private static ProjectTaskService.ProjectTaskServiceClient _projectTaskServiceClient;
        private static SprintService.SprintServiceClient _sprintServiceClient;

        private static MessageService.MessageServiceClient _messageServiceClient;
        private static ChatService.ChatServiceClient _chatServiceClient;
        
        

        public static AuthenticationService.AuthenticationServiceClient AuthenticationServiceClient
        {
            get
            {
                if (_authenticationServiceClient == null)
                {
                    try
                    {
                        var channel = GrpcChannel.ForAddress("https://localhost:7245");
                        _authenticationServiceClient = new AuthenticationService.AuthenticationServiceClient(channel);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new ConnectionServiceException();
                    }

                }
                return _authenticationServiceClient;
            }
        }
        public static UserService.UserServiceClient UserServiceClient
        {
            get
            {
                if (_userServiceClient == null)
                {
                    _userServiceClient = new UserService.UserServiceClient(GrpcChannel.ForAddress("https://localhost:7260"));
                }
                return _userServiceClient;
            }
        }
        public static TeamService.TeamServiceClient TeamServiceClient
        {
            get
            {
                if (_teamServiceClient == null)
                {
                    try
                    {
                        var channel = GrpcChannel.ForAddress("https://localhost:7024");
                        _teamServiceClient = new TeamService.TeamServiceClient(channel);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new ConnectionServiceException();
                    }

                }
                return _teamServiceClient;
            }
        }     
        public static NotificationService.NotificationServiceClient NotificationServiceClient
        {
            get
            {
                if (_notificationServiceClient == null)
                {
                    try
                    {
                        var channel = GrpcChannel.ForAddress("https://localhost:7029");
                        _notificationServiceClient = new NotificationService.NotificationServiceClient(channel);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new ConnectionServiceException();
                    }

                }
                return _notificationServiceClient;
            }
        }


        
        public static ProjectService.ProjectServiceClient ProjectServiceClient
        {
            get
            {
                if (_projectServiceClient == null)
                {
                    try
                    {
                        var channel = GrpcChannel.ForAddress("https://localhost:7139");
                        _projectServiceClient = new ProjectService.ProjectServiceClient(channel);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new ConnectionServiceException();
                    }

                }
                return _projectServiceClient;
            }
        }
        public static ProjectTaskService.ProjectTaskServiceClient ProjectTaskServiceClient
        {
            get
            {
                if (_projectTaskServiceClient == null)
                {
                    try
                    {
                        var channel = GrpcChannel.ForAddress("https://localhost:7139");
                        _projectTaskServiceClient = new ProjectTaskService.ProjectTaskServiceClient(channel);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new ConnectionServiceException();
                    }

                }
                return _projectTaskServiceClient;
            }
        }
        public static SprintService.SprintServiceClient SprintServiceClient
        {
            get
            {
                if (_sprintServiceClient == null)
                {
                    try
                    {
                        var channel = GrpcChannel.ForAddress("https://localhost:7139");
                        _sprintServiceClient = new SprintService.SprintServiceClient(channel);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new ConnectionServiceException();
                    }

                }
                return _sprintServiceClient;
            }
        }



        public static MessageService.MessageServiceClient MessageServiceClient
        {
            get
            {
                if (_messageServiceClient == null)
                {
                    try
                    {
                        var channel = GrpcChannel.ForAddress("https://localhost:7274");
                        _messageServiceClient = new MessageService.MessageServiceClient(channel);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new ConnectionServiceException();
                    }

                }
                return _messageServiceClient;
            }
        }
        public static ChatService.ChatServiceClient ChatServiceClient
        {
            get
            {
                if (_chatServiceClient == null)
                {
                    try
                    {
                        var channel = GrpcChannel.ForAddress("https://localhost:7274");
                        _chatServiceClient = new ChatService.ChatServiceClient(channel);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new ConnectionServiceException();
                    }

                }
                return _chatServiceClient;
            }
        }



    }
}
