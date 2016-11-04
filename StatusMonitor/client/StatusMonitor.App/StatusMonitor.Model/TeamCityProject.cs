using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCitySharp.DomainEntities;

namespace StatusMonitor.Model
{
    public class TeamCityProject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Watch { get; set; } = false;
        public List<TeamCityBuildConfig> BuildConfigs { get; set; }

        public TeamCityProject(string id, string name, bool getBuildConfigs) {
            Id = id;
            Name = name;

            if (getBuildConfigs)
                BuildConfigs = TeamCityWatcher.Instance.GetBuildConfigs(id);
        }
    }
}
