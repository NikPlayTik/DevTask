using DevTask.Model.ClassProject;
using DevTask.Model.ClassUser;
using DevTask.View.CustomDialog;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevTask.View.AddProject
{
    public partial class Page_AddProject : Page
    {
        private const string FirebaseAppUri = "https://devtaskdb-default-rtdb.europe-west1.firebasedatabase.app/";
        private FirebaseClient _client;
        private string _adminId;
        private Frame _mainFrame;

        public Page_AddProject(string adminId, Frame mainFrame)
        {
            InitializeComponent();
            _client = new FirebaseClient(FirebaseAppUri);
            _adminId = adminId;
            _mainFrame = mainFrame;
        }

        private void ProjectNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var projectName = ProjectNameTextBox.Text;
            Debug.WriteLine($"Имя проекта: {projectName}");

            if (string.IsNullOrEmpty(projectName))
            {
                Debug.WriteLine("Имя проекта пустое");
            }
        }

        private void InviteUsersTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                int lineCount = textBox.LineCount;
                double lineHeight = textBox.FontSize + textBox.FontFamily.LineSpacing;
                double newHeight = lineCount * lineHeight;

                textBox.Height = Math.Min(newHeight, 500);
            }

            var invitedUsers = InviteUsersTextBox.Text.Split(',').Select(u => u.Trim()).ToList();
            Debug.WriteLine($"Приглашенные пользователи: {string.Join(", ", invitedUsers)}");

            foreach (var username in invitedUsers)
            {
                if (string.IsNullOrEmpty(username))
                {
                    Debug.WriteLine("Неправильное имя или пользователь с таким именем не найден");
                }
            }
        }

        private async void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var projectName = ProjectNameTextBox.Text;
            if (string.IsNullOrEmpty(projectName))
            {
                CustomDialog.CustomDialog.Show("Введите название проекта!", Brushes.Red);
                return;
            }

            var invitedUsers = InviteUsersTextBox.Text.Split(',')
                .Select(u => u.Trim())
                .Where(u => !string.IsNullOrEmpty(u))
                .ToList();

            var projectId = await CreateProject(projectName, invitedUsers);

            if (!string.IsNullOrEmpty(projectId))
            {
                CustomDialog.CustomDialog.Show("Проект создан успешно!", Brushes.Green);
                _mainFrame.Content = new WorkingField.WorkingField(_mainFrame, _adminId, projectId);
            }
        }

        private async Task<string> CreateProject(string projectName, List<string> invitedUsers)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                CustomDialog.CustomDialog.Show("Введите название проекта!", Brushes.Red);
                return null;
            }

            Debug.WriteLine($"Создание проекта с именем: {projectName}");
            Debug.WriteLine($"Приглашенные пользователи: {string.Join(", ", invitedUsers)}");

            var project = new Project
            {
                Name = projectName,
                AdminId = _adminId,
                Members = new List<string> { _adminId }
            };

            var users = await _client.Child("Users").OnceAsync<User>();
            var usernames = users.Select(u => u.Object.Username).ToList();

            foreach (var username in invitedUsers)
            {
                Debug.WriteLine($"Проверка пользователя: {username}");
                if (string.IsNullOrEmpty(username))
                {
                    CustomDialog.CustomDialog.Show("Введите корректные имена пользователей!", Brushes.Red);
                    return null;
                }

                if (!usernames.Contains(username))
                {
                    CustomDialog.CustomDialog.Show($"Пользователь '{username}' не существует!", Brushes.Red);
                    return null;
                }

                var user = users.FirstOrDefault(u => u.Object.Username == username);
                if (user != null)
                {
                    Debug.WriteLine($"Пользователь найден: {username}, добавление в проект");
                    project.Members.Add(user.Object.Id);
                }
            }

            // Сначала создаем проект в Firebase и получаем ключ
            var projectResult = await _client.Child("Projects").PostAsync(project);
            var projectId = projectResult.Key;

            // Теперь добавляем проект в списки проектов пользователей
            project.Id = projectId;
            foreach (var username in invitedUsers)
            {
                var user = users.FirstOrDefault(u => u.Object.Username == username);
                if (user != null)
                {
                    await _client.Child("Users").Child(user.Object.Id).Child("Projects").PostAsync(projectId);
                }
            }

            return projectId;
        }
    }
}
