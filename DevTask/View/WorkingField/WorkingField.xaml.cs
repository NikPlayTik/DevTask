using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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

        public void ShowUserDetails(string username, string gravatarUrl)
        {
            UsernameTextBlock.Text = username;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(gravatarUrl, UriKind.Absolute);
            bitmap.EndInit();
            AvatarImage.Source = bitmap;
        }
    }
}
