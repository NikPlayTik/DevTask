using System;
using System.Windows.Controls;
using System.Windows.Media;
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
            if (!string.IsNullOrEmpty(gravatarUrl))
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(gravatarUrl, UriKind.Absolute);
                    bitmap.EndInit();

                    var brush = new ImageBrush(bitmap);
                    brush.Stretch = Stretch.UniformToFill;

                    AvatarEllipse.Fill = brush;
                    InitialsTextBlock.Visibility = System.Windows.Visibility.Collapsed;
                }
                catch
                {
                    SetInitials(username);
                }
            }
            else
            {
                SetInitials(username);
            }
        }

        private void SetInitials(string username)
        {
            InitialsTextBlock.Text = username.Substring(0, 1).ToUpper();
            InitialsTextBlock.Visibility = System.Windows.Visibility.Visible;
            AvatarEllipse.Fill = new SolidColorBrush(Colors.Gray);
        }
    }
}
