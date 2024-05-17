using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DevTask.View.WorkingField
{
    public partial class WorkingField : Page
    {
        private Frame _mainFrame;
        public WorkingField(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
        }

        public void ShowUsername(string username)
        {
            UsernameTextBlock.Text = username;
        }
    }
}
