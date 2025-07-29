using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure.Persistence
{
    public class DatabaseSettings
    {
        public string Provider { get; set; } = "SqlServer";
        public Dictionary<string, string> ConnectionStrings { get; set; } = new();
    }
}
