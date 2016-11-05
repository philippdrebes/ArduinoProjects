using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCitySharp.DomainEntities;

namespace StatusMonitor.Business
{
    public class TeamCityBuildConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Watch { get; set; } = true;
    }
}
