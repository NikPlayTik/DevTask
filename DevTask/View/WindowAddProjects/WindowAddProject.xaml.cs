using Firebase.Database;
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
using DevTask.Model.ClassUser;
using DevTask.Model.ClassProject;
using DevTask.View.CustomDialog;
using Firebase.Database.Query;

namespace DevTask.View.WindowAddProjects
{
    public partial class WindowAddProject : Window
    {
        private const string FirebaseAppUri = "https://devtaskdb-default-rtdb.europe-west1.firebasedatabase.app/";
        private FirebaseClient _client;
        private string _adminId;

        public WindowAddProject(string adminId)
        {
            InitializeComponent();
            _client = new FirebaseClient(FirebaseAppUri);
            _adminId = adminId;
        }

        private async void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var projectName = ProjectNameTextBox.Text;
            var invitedUsers = InviteUsersTextBox.Text.Split(',').Select(u => u.Trim()).ToList();

            var projectId = await CreateProject(projectName, invitedUsers);

            if (!string.IsNullOrEmpty(projectId))
            {
                MessageBox.Show("Проект создан успешно!");
                this.Close();
            }
        }

        private async Task<string> CreateProject(string projectName, List<string> invitedUsers)
        {
            var project = new Project
            {
                Name = projectName,
                AdminId = _adminId,
                Members = new List<string> { _adminId }
            };

            var users = await _client.Child("Users").OnceAsync<User>();
            foreach (var user in users)
            {
                if (invitedUsers.Contains(user.Object.Username))
                {
                    project.Members.Add(user.Object.Id);
                    // Добавляем проект в список проектов пользователя
                    await _client.Child("Users").Child(user.Object.Id).Child("Projects").PostAsync(project.Id);
                }
            }

            var projectResult = await _client.Child("Projects").PostAsync(project);
            return projectResult.Key;
        }
    }
}
