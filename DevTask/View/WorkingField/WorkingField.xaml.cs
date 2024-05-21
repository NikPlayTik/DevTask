using DevTask.View.Auth;
using System;
using System.Windows;
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

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Очистка логина из Settings
            Properties.Settings.Default.Username = string.Empty;
            Properties.Settings.Default.Save();

            // Перенаправление на страницу авторизации
            _mainFrame.Content = new AuthPage(_mainFrame);
        }

        public void ShowUserDetails(string username, string gravatarUrl)
        {
            SetGreeting(username);
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

        private void SetGreeting(string username)
        {
            var currentHour = DateTime.Now.Hour;
            string greeting;

            if (currentHour >= 5 && currentHour < 12)
            {
                greeting = "Доброе утро";
            }
            else if (currentHour >= 12 && currentHour < 17)
            {
                greeting = "Добрый день";
            }
            else if (currentHour >= 17 && currentHour < 22)
            {
                greeting = "Добрый вечер";
            }
            else
            {
                greeting = "Доброй ночи";
            }

            GreetingLabel.Content = $"{greeting}, {username}";
        }
    }
}
