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

namespace DevTask.View.CustomDialog
{
    public partial class CustomDialog : Window
    {
        public CustomDialog(string message, Brush messageColor)
        {
            InitializeComponent();
            MessageLabel.Content = message;
            MessageLabel.Foreground = messageColor;
        }
        public CustomDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void Show(string message, Brush messageColor)
        {
            var dialog = new CustomDialog(message, messageColor);
            dialog.ShowDialog();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
