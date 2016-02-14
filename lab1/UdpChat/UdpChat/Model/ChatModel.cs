using System;
using System.Collections.ObjectModel;
using System.Text;
using Newtonsoft.Json.Linq;
using UdpChat.Utility;

namespace UdpChat.Model
{
    public class ChatModel : IUdpChatCallback
    {
        private Utility.UdpChat _udpChat;
        private int _port;
        private string _username;
        private ObservableCollection<string> _messages; 
        private ObservableCollection<string> _users;
        private bool _disableNormalChat;

        public EventHandler UsernameDenied;

        public int Port
        {
            get { return _port; }
        }

        public string Username
        {
            get { return _username; }
        }

        public ObservableCollection<string> Messages
        {
            get { return _messages; }
        }

        public ObservableCollection<string> Users
        {
            get { return _users; }
        }

        public bool DisableNormalChat
        {
            get { return _disableNormalChat; }
            set { _disableNormalChat = value; }
        }

        public ChatModel(string username)
        {
            _udpChat = new Utility.UdpChat(this);
            _udpChat.StartListening();
            _messages = new ObservableCollection<string>();
            _users = new ObservableCollection<string>();
            _username = username;
            _port = _udpChat.Port;
            _disableNormalChat = false;
        }

        public ChatModel(string username, int port)
        {
            _udpChat = new Utility.UdpChat(this, port);
            _udpChat.StartListening();
            _messages = new ObservableCollection<string>();
            _users = new ObservableCollection<string>();
            _username = username;
            _port = port;
            _disableNormalChat = false;
        }

        public void Disconnect()
        {
            _udpChat.StopListening();
        }

        public void ProcessUdpMessage(byte[] data, int dataSize)
        {
            char[] chars = new char[dataSize / sizeof(char)];
            Buffer.BlockCopy(data, 0, chars, 0, dataSize);
            var jObject = JObject.Parse(new string(chars));
            var action = jObject.SelectToken(TextConstants.ActionPropertyName);
            if (action != null)
            {
                switch (action.Value<string>())
                {
                    case TextConstants.ActionNewMessage:
                        ProcessNewMessage(jObject);
                        break;
                    case TextConstants.ActionNewUser:
                        ProcessNewUser(jObject);
                        break;
                    case TextConstants.ActionCheckUsername:
                        ProcessCheckUsername(jObject);
                        break;
                    case TextConstants.ActionDenyUsername:
                        ProcessDenyUsername(jObject);
                        break;
                    case TextConstants.ActionUserExists:
                        ProcessUserExists(jObject);
                        break;
                    case TextConstants.ActionUserLeaving:
                        ProcessUserLeaving(jObject);
                        break;
                }
            }
        }

        public void AddMessage(string username, string message)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            sb.Append(DateTime.Now.ToString("T"));
            sb.Append("] ");
            sb.Append(username);
            sb.Append(" > ");
            sb.Append(message);
            _messages.Add(sb.ToString());
        }

        private void ProcessNewMessage(JObject jObject)
        {
            if (DisableNormalChat) return;
            var jsonUsername = jObject.SelectToken(TextConstants.UsernamePropertyName);
            var jsonMessage = jObject.SelectToken(TextConstants.MessagePropertyName);
            AddMessage(jsonUsername.Value<string>(), jsonMessage.Value<string>());
        }

        private void ProcessNewUser(JObject jObject)
        {
            if (DisableNormalChat) return;
            var jsonUsername = jObject.SelectToken(TextConstants.UsernamePropertyName);
            Users.Add(jsonUsername.Value<string>());
            if (jsonUsername.Value<string>() != Username)
                SendUserExists();
            AddMessage(TextConstants.SystemUsername, jsonUsername.Value<string>() +
                TextConstants.TextUserConnected);
        }

        private void ProcessCheckUsername(JObject jObject)
        {
            if (DisableNormalChat) return;
            var jsonUsername = jObject.SelectToken(TextConstants.UsernamePropertyName);
            if (Username == jsonUsername.Value<string>())
            {
                SendDenyUsername();
            }
        }

        private void ProcessDenyUsername(JObject jObject)
        {
            var jsonUsername = jObject.SelectToken(TextConstants.UsernamePropertyName);
            if (UsernameDenied != null && jsonUsername.Value<string>() == Username)
                UsernameDenied(this, new EventArgs());
        }

        private void ProcessUserExists(JObject jObject)
        {
            if (DisableNormalChat) return;
            var jsonUsername = jObject.SelectToken(TextConstants.UsernamePropertyName);
            if (!Users.Contains(jsonUsername.Value<string>()))
                Users.Add(jsonUsername.Value<string>());
        }

        private void ProcessUserLeaving(JObject jObject)
        {
            if (DisableNormalChat) return;
            var jsonUsername = jObject.SelectToken(TextConstants.UsernamePropertyName);
            if (Users.Contains(jsonUsername.Value<string>()))
                Users.Remove(jsonUsername.Value<string>());
            AddMessage(TextConstants.SystemUsername, jsonUsername.Value<string>() +
                TextConstants.TextUserDisconnected);
        }

        public void Send(JObject jObject)
        {
            var msg = jObject.ToString();
            byte[] bytes = new byte[msg.Length * sizeof(char)];
            Buffer.BlockCopy(msg.ToCharArray(), 0, bytes, 0, bytes.Length);
            _udpChat.Send(bytes);
        }

        public void SendNewMessage(string message)
        {
            Send(new JObject
            {
                {TextConstants.ActionPropertyName, new JValue(TextConstants.ActionNewMessage)},
                {TextConstants.UsernamePropertyName, new JValue(Username)},
                {TextConstants.MessagePropertyName, new JValue(message)}
            });
        }

        public void SendNewUser()
        {
            Send(new JObject
            {
                {TextConstants.ActionPropertyName, new JValue(TextConstants.ActionNewUser)},
                {TextConstants.UsernamePropertyName, new JValue(Username)}
            });
        }

        public void SendCheckUsername()
        {
            Send(new JObject
            {
                {TextConstants.ActionPropertyName, new JValue(TextConstants.ActionCheckUsername)},
                {TextConstants.UsernamePropertyName, new JValue(Username)}
            });
        }

        public void SendDenyUsername()
        {
            Send(new JObject
            {
                {TextConstants.ActionPropertyName, new JValue(TextConstants.ActionDenyUsername)},
                {TextConstants.UsernamePropertyName, new JValue(Username)}
            });
        }

        public void SendUserExists()
        {
            Send(new JObject
            {
                {TextConstants.ActionPropertyName, new JValue(TextConstants.ActionUserExists)},
                {TextConstants.UsernamePropertyName, new JValue(Username)}
            });
        }

        public void SendUserLeaving()
        {
            Send(new JObject
            {
                {TextConstants.ActionPropertyName, new JValue(TextConstants.ActionUserLeaving)},
                {TextConstants.UsernamePropertyName, new JValue(Username)}
            });
        }
    }
}
