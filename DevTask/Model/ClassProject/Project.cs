using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTask.Model.ClassProject
{
    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AdminId { get; set; }
        public List<string> Members { get; set; }
    }
}
