using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTask.Model.ClassTask
{
    public class Tasks
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
    }
}
