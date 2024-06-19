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
using Firebase.Database;
using Firebase.Database.Query;
using DevTask.Model.ClassTask;
using DevTask.Model.ClassUser;
using DevTask.Model.ClassProject;
using DevTask.View.TaskControl;
using DevTask.ViewModel.Projects;
using DevTask.ViewModel.Tasks;

namespace DevTask.View.WorkingField.Page_AllTask
{
    public partial class Page_AllTasks : Page
    {
        private const string FirebaseAppUri = "https://devtaskdb-default-rtdb.europe-west1.firebasedatabase.app/";
        private FirebaseClient _client;
        private Dictionary<string, User> _users;
        private string _projectId;

        public Page_AllTasks(string projectId)
        {
            InitializeComponent();
            _client = new FirebaseClient(FirebaseAppUri);
            _projectId = projectId;
            LoadData();
        }

        private async void LoadData()
        {
            await LoadUsers();
            await LoadTasks();
        }

        private async Task LoadUsers()
        {
            var users = await _client.Child("Users").OnceAsync<User>();
            _users = users.ToDictionary(user => user.Object.Id, user => user.Object);
        }

        private async Task LoadTasks()
        {
            
        }

        private Border CreateTaskControl(Tasks task)
        {
            var border = new Border
            {
                Style = (Style)FindResource("TaskCardStyle")
            };

            var stackPanel = new StackPanel();

            var description = new TextBlock
            {
                Text = task.Description,
                Style = (Style)FindResource("DescriptionTextStyle")
            };
            stackPanel.Children.Add(description);

            var avatars = new StackPanel { Orientation = Orientation.Horizontal };

            if (_users.TryGetValue(task.SenderId, out var sender))
            {
                var senderAvatar = new Image
                {
                    Source = new BitmapImage(new Uri(sender.GravatarUrl)),
                    Style = (Style)FindResource("AvatarStyle")
                };
                avatars.Children.Add(senderAvatar);
            }

            var arrow = new TextBlock
            {
                Text = "→",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            avatars.Children.Add(arrow);

            if (_users.TryGetValue(task.ReceiverId, out var receiver))
            {
                var receiverAvatar = new Image
                {
                    Source = new BitmapImage(new Uri(receiver.GravatarUrl)),
                    Style = (Style)FindResource("AvatarStyle")
                };
                avatars.Children.Add(receiverAvatar);
            }

            stackPanel.Children.Add(avatars);
            border.Child = stackPanel;

            return border;
        }
    }
}
