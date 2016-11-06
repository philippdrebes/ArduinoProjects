using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeamCitySharp;
using TeamCitySharp.DomainEntities;

namespace StatusMonitor.Business
{
    public class TeamCityWatcher
    {
        private string _serverPath = null;
        private string _serverUsername = null;
        private string _serverPassword = null;
        private List<Project> _projects = null;

        private Thread _worker = null;

        public ObservableCollection<TeamCityProject> Projects { get; set; } = new ObservableCollection<TeamCityProject>();

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
            _worker = new Thread(WatchProjects);
            _worker.Start();
        }

        ~TeamCityWatcher()
        {
            if (_worker.IsAlive)
            {
                _worker.Abort();
            }
        }

        private void Initialize()
        {
            _serverPath = AppSettingsHelper.Instance.Settings?.TeamCityServerPath;
            _serverUsername = AppSettingsHelper.Instance.Settings?.TeamCityUserName;
            _serverPassword = AppSettingsHelper.Instance.Settings?.TeamCityPassword;

            if (!string.IsNullOrEmpty(_serverPath))
                _client = new TeamCityClient(_serverPath);
        }

        public void Start()
        {
            Initialize();

            try
            {
                if (!string.IsNullOrEmpty(_serverUsername) && !string.IsNullOrEmpty(_serverPassword) && _serverUsername != "guest")
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

                Projects.Clear();
                foreach (Project project in _projects)
                    Projects.Add(new TeamCityProject(project.Id, project.Name, true));
            }
            catch (Exception ex)
            {
                // TODO Error logging
                Console.WriteLine(ex.Message);
            }
        }

        public void Stop()
        {
            if (_worker.IsAlive)
            {
                _worker.Abort();
            }
        }

        public List<TeamCityBuildConfig> GetBuildConfigs(string projectId)
        {
            try
            {
                List<TeamCityBuildConfig> ret = new List<TeamCityBuildConfig>();
                List<BuildConfig> buildconfigs = _client.BuildConfigs.ByProjectId(projectId);
                foreach (BuildConfig bldConf in buildconfigs)
                    ret.Add(new TeamCityBuildConfig() { Id = bldConf.Id, Name = bldConf.Name });
                return ret;
            }
            catch (Exception ex)
            {
                // TODO Error logging
                Console.WriteLine(ex.Message);
                return new List<TeamCityBuildConfig>();
            }
        }

        private void WatchProjects()
        {
            while (true)
            {
                if (_client == null && _projects == null && Projects == null)
                {
                    //TODO
                    Thread.Sleep(20000);
                    continue; // return;
                }

                List<TeamCityProject> projects = Projects.Where(x => x.Watch).ToList();

                foreach (TeamCityProject project in projects)
                {
                    List<TeamCityBuildConfig> configs = project.BuildConfigs.Where(x => x.Watch).ToList();
                    foreach (TeamCityBuildConfig bldConf in configs)
                    {
                        Build lastBuild = _client.Builds.LastBuildByBuildConfigId(bldConf.Id);
                        Console.WriteLine($"Project: {project.Name} BuildConfig: {bldConf.Name} Last build status: {lastBuild.Status}");

                        if (lastBuild.Status == Constants.TeamCity.BuildStatus.SUCCESS)
                            ArduinoCommunicator.Instance.SendState(true);
                        else
                            ArduinoCommunicator.Instance.SendState(false);
                    }
                }

                Thread.Sleep(10000);
            }
        }
    }
}
