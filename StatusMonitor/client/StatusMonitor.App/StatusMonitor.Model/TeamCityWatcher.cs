using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCitySharp;

namespace StatusMonitor.Model
{
    public class TeamCityWatcher
    {
        private string _serverPath = null;
        private string _serverUsername = null;
        private string _serverPassword = null;

        private TeamCityWatcher _instance = null;
        public TeamCityWatcher Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TeamCityWatcher();
                return _instance;
            }
        }

        private bool _isGuest = false;
        public bool IsGuest => _isGuest;

        private TeamCityWatcher()
        {
            Initialize();
        }

        public void Initialize()
        {
            _serverPath = ConfigurationManager.AppSettings[Constants.Config.TEAMCITYSERVERPATH];
            _serverUsername = ConfigurationManager.AppSettings[Constants.Config.TEAMCITYUSERNAME];
            _serverPassword = ConfigurationManager.AppSettings[Constants.Config.TEAMCITYPASSWORD];

            TeamCityClient client = new TeamCityClient(_serverPath);

            if (!string.IsNullOrEmpty(_serverUsername) && !string.IsNullOrEmpty(_serverPassword))
            {
                client.Connect(_serverUsername, _serverPassword);
                _isGuest = false;
            }
            else
            {
                client.ConnectAsGuest();
                _isGuest = true;
            }

        }

    }
}
