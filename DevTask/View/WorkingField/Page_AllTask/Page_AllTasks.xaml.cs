﻿using System;
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
using DevTask.View.WorkingField.Page_AllTask;
using DevTask.ViewModel.TaskControl;
using System.Diagnostics;
using System.Windows.Threading;

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
            Debug.WriteLine(LoadTasks());
        }

        private async void LoadData()
        {
            Debug.WriteLine($"Загрузка данных для проекта с ID: {_projectId}");
            await LoadUsers();
            var tasks = await LoadTasks();

            if (tasks == null || tasks.Count == 0)
            {
                MessageBox.Show("Для этого проекта не найдено ни одной задачи.");
            }
            else
            {
                Debug.WriteLine($"Загружено {tasks.Count} задач для проекта с ID: {_projectId}");
            }
        }

        private async Task LoadUsers()
        {
            try
            {
                var users = await _client.Child("Users").OnceAsync<User>();
                _users = users.ToDictionary(user => user.Object.Id, user => user.Object);
                Debug.WriteLine($"Загружено {_users.Count} пользователей.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}");
            }
        }

        private async Task<List<TaskModel>> LoadTasks()
        {
            try
            {
                var tasks = await _client.Child("Tasks").OnceAsync<TaskModel>();

                // Выводим все задачи для отладки
                Debug.WriteLine("Все задачи:");
                foreach (var task in tasks)
                {
                    Debug.WriteLine($"Задача ID: {task.Key}, ProjectId: {task.Object.ProjectId}, Description: {task.Object.Description}");
                }

                TaskStackPanel.Children.Clear();

                foreach (var task in tasks)
                {
                    AddTaskToView(task.Object);
                }

                return tasks.Select(t => t.Object).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке задач: {ex.Message}");
                return null;
            }
        }

        private void AddTaskToView(TaskModel task)
        {
            var taskControl = CreateTaskControl(task);
            TaskStackPanel.Children.Add(taskControl);
        }

        private Border CreateTaskControl(TaskModel task)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(224, 156, 63)),
                CornerRadius = new CornerRadius(40),
                Margin = new Thickness(-10, 50, 0, 0),
                Padding = new Thickness(40),
                Height = 384,
                MinWidth = 381,
                MaxWidth = 381,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition 
            { 
                Height = new GridLength(1, GridUnitType.Star) 
            });
            grid.RowDefinitions.Add(new RowDefinition 
            { 
                Height = new GridLength(1, GridUnitType.Star) 
            });
            grid.RowDefinitions.Add(new RowDefinition 
            { 
                Height = new GridLength(1, GridUnitType.Star) 
            });

            var description = new TextBlock
            {
                Text = task.Description ?? "Нет описания",
                TextWrapping = TextWrapping.Wrap,
                FontSize = 40,
                FontFamily = new FontFamily("Cygre"),
                TextTrimming = TextTrimming.CharacterEllipsis,
                MaxHeight = 143,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(description, 0);
            Grid.SetRowSpan(description, 2);
            grid.Children.Add(description);

            var avatars = new StackPanel 
            { 
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Отправитель задачи
            if (_users != null)
            {
                if (!string.IsNullOrEmpty(task.SenderId) && _users.TryGetValue(task.SenderId, out var sender))
                {
                    var senderAvatar = new Image
                    {
                        Source = new BitmapImage(new Uri(sender.GravatarUrl)),
                        Width = 78.7,
                        Height = 78.7,
                        HorizontalAlignment = HorizontalAlignment.Center
                        
                    };
                    avatars.Children.Add(senderAvatar);
                }

                var arrow = new TextBlock
                {
                    Text = "→",
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 64,
                    Margin = new Thickness(20, 0, 20, 0),
                    FontFamily = new FontFamily("Inter")
                };
                avatars.Children.Add(arrow);

                // Принимающий задачу
                if (!string.IsNullOrEmpty(task.ReceiverId) && _users.TryGetValue(task.ReceiverId, out var receiver))
                {
                    var receiverAvatar = new Image
                    {
                        Source = new BitmapImage(new Uri(receiver.GravatarUrl)),
                        Width = 78.7,
                        Height = 78.7,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    avatars.Children.Add(receiverAvatar);
                }
            }

            Grid.SetRow(avatars, 2);
            grid.Children.Add(avatars);

            border.Child = grid;

            return border;
        }
    }
}
