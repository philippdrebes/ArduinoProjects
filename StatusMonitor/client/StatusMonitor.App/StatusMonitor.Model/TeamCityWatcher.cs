using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCitySharp;
using TeamCitySharp.DomainEntities;

namespace StatusMonitor.Model
{
    public class TeamCityWatcher
    {
        private string _serverPath = null;
        private string _serverUsername = null;
        private string _serverPassword = null;

        private List<Project> _projects = null;

        public List<Project> Projects
        {
            get { return _projects; }
        }

        private TeamCityClient _client = null;

        private static TeamCityWatcher _instance = null;

        public static TeamCityWatcher Instance
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

        private void Initialize()
        {
            _serverPath = AppSettingsHelper.Instance.Settings.TeamCityServerPath;
            _serverUsername = AppSettingsHelper.Instance.Settings.TeamCityUserName;
            _serverPassword = AppSettingsHelper.Instance.Settings.TeamCityPassword;

            if (!string.IsNullOrEmpty(_serverPath))
                _client = new TeamCityClient(_serverPath);
        }

        public void Start()
        {
            Initialize();

            try
            {
                if (!string.IsNullOrEmpty(_serverUsername) && !string.IsNullOrEmpty(_serverPassword))
                {
                    _client.Connect(_serverUsername, _serverPassword);
                    _isGuest = false;
                }
                else
                {
                    _client.ConnectAsGuest();
                    _isGuest = true;
                }

                _projects = _client.Projects.All();
            }
            catch (Exception ex)
            {
                // TODO Error logging
                Console.WriteLine(ex.Message);
            }
        }
    }
}
