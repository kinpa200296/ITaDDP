using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UdpChat.Model;
using UdpChat.Utility;

namespace UdpChat.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ChatModel _chat;
        private string _username;
        private int _port;
        private string _message;
        private Visibility _chatInterfaceVisibility;
        private Visibility _loginInterfaceVisibility;
        private Visibility _loginButtonVisibility;
        private Visibility _processingBarVisibility;
        private bool _usernameDenied;

        #region properties

        public ChatModel Chat
        {
            get { return _chat; }
            private set
            {
                _chat = value;
                OnPropertyChanged("Chat");
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value; 
                OnPropertyChanged("Message");
            }
        }

        public Visibility ChatInterfaceVisibility
        {
            get { return _chatInterfaceVisibility; }
            set
            {
                _chatInterfaceVisibility = value;
                OnPropertyChanged("ChatInterfaceVisibility");
            }
        }

        public Visibility LoginInterfaceVisibility
        {
            get { return _loginInterfaceVisibility; }
            set
            {
                _loginInterfaceVisibility = value;
                OnPropertyChanged("LoginInterfaceVisibility");
            }
        }

        public Visibility LoginButtonVisibility
        {
            get { return _loginButtonVisibility; }
            set
            {
                _loginButtonVisibility = value;
                OnPropertyChanged("LoginButtonVisibility");
            }
        }

        public Visibility ProcessingBarVisibility
        {
            get { return _processingBarVisibility; }
            set
            {
                _processingBarVisibility = value;
                OnPropertyChanged("ProcessingBarVisibility");
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged("Username");
            }
        }

        public int Port
        {
            get { return _port; }
            set
            {
                if (value > 0 && value < 65535)
                {
                    _port = value;
                    OnPropertyChanged("Port");
                }
                else
                {
                    Port = _port;
                }
            }
        }

        #endregion

        #region constructors

        public MainWindowViewModel()
        {
            _chatInterfaceVisibility = Visibility.Collapsed;
            _loginInterfaceVisibility = Visibility.Visible;
            _loginButtonVisibility = Visibility.Visible;
            _processingBarVisibility = Visibility.Collapsed;
            _username = "default";
            _port = 49374;
        }

        #endregion

        #region commands

        public ICommand Login { get { return new RelayCommand(LoginExecute);} }
        public ICommand Send { get { return new RelayCommand(SendExecute);} }
        public ICommand Disconnect { get { return new RelayCommand(DisconnectExecute); } }

        #endregion

        #region methods

        public async void LoginExecute()
        {
            if (Username == TextConstants.SystemUsername)
            {
                MessageBox.Show(TextConstants.TextUsernameDenied);
                return;
            }

            Chat = new ChatModel(Username, Port);
            Chat.UsernameDenied += (sender, args) => OnUsernameDenied();
            Chat.DisableNormalChat = true;
            _usernameDenied = false;

            Chat.SendCheckUsername();

            LoginButtonVisibility = Visibility.Collapsed;
            ProcessingBarVisibility = Visibility.Visible;

            await Task.Delay(2000);

            Chat.DisableNormalChat = false;
            Chat.UsernameDenied = null;
            LoginButtonVisibility = Visibility.Visible;
            ProcessingBarVisibility = Visibility.Collapsed;

            if (_usernameDenied)
            {
                MessageBox.Show(TextConstants.TextUsernameDenied);
                return;
            }

            Chat.SendNewUser();

            ChatInterfaceVisibility = Visibility.Visible;
            LoginInterfaceVisibility = Visibility.Collapsed;
        }

        public void SendExecute()
        {
            if (!String.IsNullOrWhiteSpace(Message))
            {
                Chat.SendNewMessage(Message);
                Message = "";
            }
        }

        public void OnUsernameDenied()
        {
            _usernameDenied = true;
        }

        public void DisconnectExecute()
        {
            Chat.SendUserLeaving();
            Chat.Disconnect();
            _chat = null;
            ChatInterfaceVisibility = Visibility.Collapsed;
            LoginInterfaceVisibility = Visibility.Visible;
        }

        #endregion
    }
}
