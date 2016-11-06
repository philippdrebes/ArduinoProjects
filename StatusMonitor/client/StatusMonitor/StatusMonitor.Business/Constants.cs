using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusMonitor.Business
{
    public class Constants
    {
        public class Config
        {
            public const string TEAMCITYSERVERPATH = "TeamCityServerPath";
            public const string TEAMCITYUSERNAME = "TeamCityUsername";
            public const string TEAMCITYPASSWORD = "TeamCityPassword";
        }

        public class TeamCity
        {
            public class BuildStatus
            {
                public const string FAILURE = "FAILURE";
                public const string SUCCESS = "SUCCESS";
            }
        }
    }
}
