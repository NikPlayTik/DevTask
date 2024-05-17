using DevTask.View.Auth;
using DevTask.View.Registration;
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
using System.Windows.Shapes;

namespace DevTask.View.MainWindow
{
    public partial class MainWindow : Window
    {
        private const double AspectRatio = 16.0 / 9.0;
        private bool _isResizing = false;

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new RegistrationPage(MainFrame);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_isResizing) return;

            _isResizing = true;

            if (e.WidthChanged)
            {
                double newHeight = e.NewSize.Width / AspectRatio;
                if (newHeight >= MinHeight)
                {
                    Height = newHeight;
                }
                else
                {
                    Width = MinHeight * AspectRatio;
                }
            }
            else if (e.HeightChanged)
            {
                double newWidth = e.NewSize.Height * AspectRatio;
                if (newWidth >= MinWidth)
                {
                    Width = newWidth;
                }
                else
                {
                    Height = MinWidth / AspectRatio;
                }
            }

            _isResizing = false;
        }
    }
}
