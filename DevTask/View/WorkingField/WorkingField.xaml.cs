using DevTask.View.Auth;
using DevTask.View.WindowAddProjects;
using DevTask.View.WorkingField;
using DevTask.Model.ClassProject;
using DevTask.Model.ClassTask;
using DevTask.Model.ClassUser;
using Firebase.Database;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Firebase.Database.Query;

namespace DevTask.View.WorkingField
{
    public partial class WorkingField : Page
    {
        private Frame _mainFrame;
        private string _currentUserId;
        private const string FirebaseAppUri = "https://devtaskdb-default-rtdb.europe-west1.firebasedatabase.app/";
        private FirebaseClient _client;

        public WorkingField(Frame mainFrame, string currentUserId)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _client = new FirebaseClient(FirebaseAppUri);
            _currentUserId = currentUserId;
            LoadProjects();
        }

        // Выход с аккаунта
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Очистка логина из Settings
            Properties.Settings.Default.Username = string.Empty;
            Properties.Settings.Default.Save();

            // Перенаправление на страницу авторизации
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

        // Установка инициал по умолчанию
        private void SetInitials(string username)
        {
            InitialsTextBlock.Text = username.Substring(0, 1).ToUpper();
            InitialsTextBlock.Visibility = System.Windows.Visibility.Visible;
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

            GreetingLabel.Content = $"{greeting}, {username}";
        }

        // Обработчик событий для кнопки "Передача задачи"
        private void TaskTransferButton_Click(object sender, RoutedEventArgs e)
        {
            // Создание экземпляра страницы, которую вы хотите отобразить во Frame
            var taskTransferPage = new Page_TaskTransfer.Page_TaskTransfer();

            // Установка содержимого Frame равным новой странице
            StatusFrame.Content = taskTransferPage;
        }

        // Обработчик событий для кнопки "Все задачи"
        private void AllTasksButton_Click(object sender, RoutedEventArgs e)
        {
            var allTasksPage = new Page_AllTasks.Page_AllTasks();
            StatusFrame.Content = allTasksPage;
        }

        // Кнопка "Создать новый проект"
        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var windowAddProject = new WindowAddProject(_currentUserId);
            windowAddProject.ShowDialog();
            LoadProjects(); // Обновление списка проектов после создания нового
        }

        // Загрузка проекта
        private async void LoadProjects()
        {
            ProjectComboBox.Items.Clear();

            // Добавляем элемент "Создать проект" в начале списка
            ProjectComboBox.Items.Add(new ComboBoxItem 
            { 
                Content = "Создать проект", 
                Tag = "create_project" 
            });

            var user = await _client.Child("Users").Child(_currentUserId).OnceSingleAsync<User>();
            var projects = await _client.Child("Projects").OnceAsync<Project>();

            foreach (var project in projects)
            {
                if (user.Projects.Contains(project.Object.Id))
                {
                    ProjectComboBox.Items.Add(new ComboBoxItem 
                    { 
                        Content = project.Object.Name, 
                        Tag = project.Object.Id 
                    });
                }
            }
        }

        // Выделенный элемент ComboBox
        private void ProjectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProjectComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (selectedItem.Tag.ToString() == "create_project")
                {
                    CreateProjectButton_Click(sender, e);
                }
            }
        }
    }
}
