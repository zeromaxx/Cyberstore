using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Eshop
{
    [HubName("echo")]
    public class EchoHub : Hub
    {
        public void Hello(string name,string message)
        {
            Trace.WriteLine(message);
            //I CAN CALL THE FUNCTION CLIENTS FROM JAVASCRIPT-- CHECK HOW IN Home/Index.cshtml
            var clients = Clients.Caller;
            //clients.addNewMessageToPage(name, message);
            
            Clients.All.receiveNotification(name,message);
        }


        //[HubMethodName("SendNotifications")]
        public void Send(string name, string message)

        {

            Clients.All.broadcastMessage(name, message);

        }
        public void SendNotifications(string message)
        {

            Clients.Caller.receiveNotification(message);

        }

        public void Notify(string friend)
        {
            using(Entities en = new Entities())
            {
                User user = en.Users.Where(u => u.Username.Equals(friend)).FirstOrDefault();
                int friendId = user.UserId;
           

            var clients = Clients.Others;

                clients.frnotify(friend);
            }
        }
        public void NotifyOfMessage(string friend)
        {
            using (Entities en = new Entities())
            { // Init db


                // Get friend id
                User userDTO = en.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
                int friendId = userDTO.UserId;

                // Get message count
                var messageCount = en.SupportMessages.Count(x => x.To == friendId && x.Read == false);

                // Set clients
                var clients = Clients.Others;

                // Call js function
                clients.msgcount(friend, messageCount);
            }
        }
        public void NotifyOfMessageOwner()
            {
                using (Entities en = new Entities())
                { // Init db
                  // Get user id
                    User userDTO = en.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
                int userId = userDTO.UserId;

                // Get message count
                var messageCount = en.SupportMessages.Count(x => x.To == userId && x.Read == false);

                // Set clients
                var clients = Clients.Caller;

                // Call js function
                clients.msgcount(Context.User.Identity.Name, messageCount);
            }
        }


        public override Task OnConnected()
        {
            // Init db

            using (Entities en = new Entities())
            {
                // Log user conn
                Trace.WriteLine("Here I am " + Context.ConnectionId);



                // Get user id
                User userDTO = en.Users.Where(x => x.Email.Equals(Context.User.Identity.Name)).FirstOrDefault();
                int userId = userDTO.UserId;

                // Get conn id
                string connId = Context.ConnectionId;

                // Add onlineDTO
              
                    if (!en.Onlines.Any(x => x.Id == userId) && !en.Onlines.Any(x => x.Id == 0))
                    {
                        Online online = new Online();

                        online.Id = userId;
                        online.ConnId = connId;

                        en.Onlines.Add(online);

                        en.SaveChanges();
                    }
                
             

                // Get all online ids
                List<int> onlineIds = en.Onlines.ToArray().Select(x => x.Id).ToList();

                // Get friend ids
                var friendIds1 = en.Connections.Where(x => x.UserId == userId && x.Active == true).ToArray().Select(x => x.AdminId).ToList();

                var friendIds2 = en.Connections.Where(x => x.AdminId == userId && x.Active == true).ToArray().Select(x => x.UserId).ToList();

                var allFriendsIds = friendIds1.Concat(friendIds2).ToList();

                // Get final set of ids
                List<int> resultList = onlineIds.Where((i) => allFriendsIds.Contains(i)).ToList();

                // Create a dict of friend ids and usernames

                Dictionary<int, string> dictFriends = new Dictionary<int, string>();

                foreach (var id in resultList)
                {
                    var users = en.Users.Find(id);
                    string friend = users.Username;

                    if (!dictFriends.ContainsKey(id))
                    {
                        dictFriends.Add(id, friend);
                    }
                }

                var transformed = from key in dictFriends.Keys
                                  select new { id = key, friend = dictFriends[key] };

                string json = JsonConvert.SerializeObject(transformed);

                // Set clients
                var clients = Clients.Caller;

                // Call js function
                clients.getonlinefriends(Context.User.Identity.Name, json);

                // Update chat
                UpdateChat();

                // Return
                return base.OnConnected();
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {

            using (Entities en = new Entities())
            {
                // Log
                Trace.WriteLine("gone - " + Context.ConnectionId + " " + Context.User.Identity.Name);



                // Get user id
                User userDTO = en.Users.Where(x => x.Email.Equals(Context.User.Identity.Name)).FirstOrDefault();
                int userId = userDTO.UserId;

                // Remove from db
                if (en.Onlines.Any(x => x.Id == userId))
                {
                    Online online = en.Onlines.Find(userId);
                    en.Onlines.Remove(online);
                    en.SaveChanges();
                }

                // Update chat
                UpdateChat();

                // Return
                return base.OnDisconnected(stopCalled);
            }
        }

        public void UpdateChat()
        {
            // Init db
            using (Entities en = new Entities())
            {
                // Get all online ids
                List<int> onlineIds = en.Onlines.ToArray().Select(x => x.Id).ToList();
                if(onlineIds.Count != 0)
                {
                    foreach (var userId in onlineIds)
                    {
                        // Get username
                        User user = en.Users.Find(userId);
                        string username = user.Username;

                        // Get all friend ids

                        var friendIds1 = en.Connections.Where(x => x.UserId == userId && x.Active == true).ToArray().Select(x => x.AdminId).ToList();

                        var friendIds2 = en.Connections.Where(x => x.AdminId == userId && x.Active == true).ToArray().Select(x => x.UserId).ToList();

                        var allFriendsIds = friendIds1.Concat(friendIds2).ToList();

                        // Get final set of ids
                        List<int> resultList = onlineIds.Where((i) => allFriendsIds.Contains(i)).ToList();

                        // Create a dict of friend ids and usernames

                        Dictionary<int, string> dictFriends = new Dictionary<int, string>();

                        foreach (var id in resultList)
                        {
                            var users = en.Users.Find(id);
                            string friend = users.Username;

                            if (!dictFriends.ContainsKey(id))
                            {
                                dictFriends.Add(id, friend);
                            }
                        }

                        var transformed = from key in dictFriends.Keys
                                          select new { id = key, friend = dictFriends[key] };

                        string json = JsonConvert.SerializeObject(transformed);

                        // Set clients
                        var clients = Clients.All;

                        // Call js function
                        clients.updatechat(username, json);
                    }

                }
                // Loop thru onlineids and get friends
            }

        }

        public void SendChat(int friendId, string friendUsername, string message)
        {
            // Init db

            using (Entities en = new Entities())
            {
                // Get user id
                User userDTO = en.Users.Where(x => x.Email.Equals(Context.User.Identity.Name)).FirstOrDefault();
                int userId = userDTO.UserId;

                // Set clients
                var clients = Clients.All;

                // Call js function
                clients.sendchat(userId, Context.User.Identity.Name, friendId, friendUsername, message);
            }
        }

    }
}

