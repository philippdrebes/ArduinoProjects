using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StatusMonitor.Business
{
    [Serializable]
    public class AppSettings : ISerializable
    {
        private string _teamCityServerPath;
        public string TeamCityServerPath
        {
            get { return _teamCityServerPath; }
            set { _teamCityServerPath = value; }
        }

        private string _teamCityUserName;
        public string TeamCityUserName
        {
            get { return _teamCityUserName; }
            set { _teamCityUserName = value; }
        }

        private string _teamCityPassword;
        public string TeamCityPassword
        {
            get { return _teamCityPassword; }
            set { _teamCityPassword = value; }
        }

        public AppSettings()
        {
        }

        public AppSettings(SerializationInfo info, StreamingContext context)
        {
            try
            {
                if (info.GetValue(Constants.Config.TEAMCITYSERVERPATH, typeof(string)) != null)
                    _teamCityServerPath = (string)info.GetValue(Constants.Config.TEAMCITYSERVERPATH, typeof(string));

                if (info.GetValue(Constants.Config.TEAMCITYUSERNAME, typeof(string)) != null)
                    _teamCityUserName = (string)info.GetValue(Constants.Config.TEAMCITYUSERNAME, typeof(string));

                if (info.GetValue(Constants.Config.TEAMCITYPASSWORD, typeof(string)) != null)
                    _teamCityPassword = (string)info.GetValue(Constants.Config.TEAMCITYPASSWORD, typeof(string));
            }
            catch (Exception ex)
            {
                // TODO
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(Constants.Config.TEAMCITYSERVERPATH, _teamCityServerPath, typeof(string));
            info.AddValue(Constants.Config.TEAMCITYUSERNAME, _teamCityUserName, typeof(string));
            info.AddValue(Constants.Config.TEAMCITYPASSWORD, _teamCityPassword, typeof(string));
        }

    }
}
