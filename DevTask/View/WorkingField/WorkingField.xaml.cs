using DevTask.View.Auth;
using DevTask.View.WindowAddProjects;
using DevTask.View.WorkingField;
using DevTask.Model.ClassProject;
using DevTask.Model.ClassTask;
using DevTask.Model.ClassUser;
using DevTask.View.TaskControl;
using DevTask.View.WorkingField.Page_AllTask;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Firebase.Database;

namespace DevTask.View.WorkingField
{
    public partial class WorkingField : Page
    {
        private Frame _mainFrame;
        private string _currentUserId;
        private string _currentProjectId;

        public WorkingField(Frame mainFrame, string currentUserId, string currentProjectId)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _currentUserId = currentUserId;
            _currentProjectId = currentProjectId;

            LoadProjects();
        }

        // Выход с аккаунта
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Username = string.Empty;
            Properties.Settings.Default.Save();
            _mainFrame.Content = new AuthPage(_mainFrame);
        }

        // Вывод аватарки
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
                    InitialsTextBlock.Visibility = Visibility.Collapsed;
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

        // Установка инициал по умолчанию
        private void SetInitials(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                InitialsTextBlock.Text = "?";
            }
            else
            {
                InitialsTextBlock.Text = username.Substring(0, 1).ToUpper();
            }

            InitialsTextBlock.Visibility = Visibility.Visible;
            AvatarEllipse.Fill = new SolidColorBrush(Colors.Gray);
        }

        // Установка заголовка приветствия
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

            if (string.IsNullOrEmpty(username))
            {
                username = "User";
            }

            GreetingLabel.Content = $"{greeting}, {username}";
        }

        // Обработчик событий для кнопки "Передача задачи"
        private void TaskTransferButton_Click(object sender, RoutedEventArgs e)
        {
            var taskTransferPage = new Page_TaskTransfer.Page_TaskTransfer(_currentUserId, _currentProjectId);
            StatusFrame.Content = taskTransferPage;
        }

        // Обработчик событий для кнопки "Все задачи"
        private void AllTasksButton_Click(object sender, RoutedEventArgs e)
        {
            var allTasksPage = new Page_AllTasks(_currentUserId);
            StatusFrame.Content = allTasksPage;
        }

        // Загрузка проектов пользователя и заполнения ComboBox
        private async void LoadProjects()
        {
            var firebaseClient = new FirebaseClient("https://devtaskdb-default-rtdb.europe-west1.firebasedatabase.app/");
            var projects = await firebaseClient.Child("Projects").OnceAsync<Project>();

            var userProjects = projects.Where(p => p.Object.Members.Contains(_currentUserId)).Select(p => p.Object.Name).ToList();

            ProjectsComboBox.ItemsSource = userProjects;
        }

    }
}
