namespace Getaway.Presentation.Hubs
{
    public class UserConnections
    {
        public List<UserConnection> ListUserConnections = new List<UserConnection>();


        public bool IsConnectedUser(string userTag)
        {
            return ListUserConnections.Any(c => c.Tag == userTag);
        }


        public bool IsConnectedUser(int userId)
        {
            return ListUserConnections.Any(c => c.Id == userId);
        }

        public string GetUserConnectionId(string userTag)
        {
            return ListUserConnections.FirstOrDefault(c => c.Tag == userTag).ConnectionId;
        }

        public int GetUserId(string userTag)
        {
            return ListUserConnections.FirstOrDefault(c => c.Tag == userTag).Id;
        }

        public string GetUserTag(string connectionId)
        {
            return ListUserConnections.FirstOrDefault(c => c.ConnectionId == connectionId).Tag;
        }

        public string GetUserConnectionId(int userId)
        {
            return ListUserConnections.FirstOrDefault(c => c.Id == userId).ConnectionId;
        }

        public void RemoveUserConnectionId(string connectionId)
        {
            if (IsConnectedUser(connectionId))
            {
                ListUserConnections.Remove(ListUserConnections.First(u => u.ConnectionId == connectionId));
            }
        }

    }
    public class UserConnection
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string ConnectionId { get; set; }
    }

}
