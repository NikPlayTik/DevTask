using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DevTask.ViewModel.Tasks
{
    public class TaskVM
    {
        public string Description { get; set; }
        public BitmapImage SenderAvatar { get; set; }
        public BitmapImage ReceiverAvatar { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
    }
}
