using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointlessCLI
{
    public class ProjectModel
    {
        public string[] Include { get; set; }
        public string[] CompileOrder { get; set; }
        public ProjectEntryPointModel EntryPoint { get; set; }
    }

    public class ProjectEntryPointModel
    {
        public string Namespace { get; set; }
        public string Method { get; set; }
    }
}
