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
using System.Windows.Media.Animation;

namespace DevTask.View.WorkingField.Page_AllTask
{
    public partial class Page_AllTasks : Page
    {
        private const string FirebaseAppUri = "https://devtaskdb-default-rtdb.europe-west1.firebasedatabase.app/";
        private FirebaseClient _client;
        private Dictionary<string, User> _users;
        private string _projectId;
        private string _currentUserId;

        public Page_AllTasks(string projectId, string currentUserId)
        {
            InitializeComponent();
            _client = new FirebaseClient(FirebaseAppUri);
            _projectId = projectId;
            _currentUserId = currentUserId;

            Debug.WriteLine($"Текущий пользователь UserId: {_currentUserId}");
            Debug.WriteLine($"ProjectId: {_projectId}");

            LoadData();
        }

        private async void LoadData()
        {
            Debug.WriteLine($"Загрузка данных для проекта с ID: {_projectId}");
            await LoadUsers();
            var tasks = await LoadTasks();

            if (tasks == null || tasks.Count == 0)
            {
                CustomDialog.CustomDialog.Show("Для этого проекта не найдено ни одной задачи", Brushes.Red);
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
                CustomDialog.CustomDialog.Show($"Ошибка при загрузке пользователей: {ex.Message}", Brushes.Red);
            }
        }

        private async Task<List<TaskModel>> LoadTasks()
        {
            Debug.WriteLine($"Загрузка задачи для ProjectId: {_projectId}");
            try
            {
                var allTasks = await _client
                    .Child("Tasks")
                    .OrderBy("ProjectId")
                    .EqualTo(_projectId)
                    .OnceAsync<TaskModel>();

                var tasks = allTasks.Select(item => new TaskModel
                {
                    Id = item.Key,
                    ProjectId = item.Object.ProjectId,
                    Description = item.Object.Description,
                    ReceiverId = item.Object.ReceiverId,
                    SenderId = item.Object.SenderId
                }).ToList();

                if (tasks.Count == 0)
                {
                    Debug.WriteLine("Задачи не найдены для данного ProjectId.");
                }
                else
                {
                    Debug.WriteLine($"Найдено {tasks.Count} задач для ProjectId: {_projectId}");
                }

                Debug.WriteLine("Все задачи для проекта:");
                foreach (var task in tasks)
                {
                    Debug.WriteLine($"Задача ID: {task.Id}, ProjectId: {task.ProjectId}, Description: {task.Description}");
                }

                TaskStackPanel.Children.Clear();

                foreach (var task in tasks)
                {
                    AddTaskToView(task);
                }

                return tasks;
            }
            catch (Exception ex)
            {
                CustomDialog.CustomDialog.Show($"Ошибка при загрузке задач: {ex.Message}", Brushes.Red);
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
            // Плашка для задач
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(224, 156, 63)),
                CornerRadius = new CornerRadius(40),
                Margin = new Thickness(-10, 50, 0, 0),
                Padding = new Thickness(30),
                Height = 384,
                MinWidth = 381,
                MaxWidth = 381,
                HorizontalAlignment = HorizontalAlignment.Left,
                Tag = false // Используется для отслеживания состояния увеличения/уменьшения
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

            // Блок описания задачи
            var description = new TextBlock
            {
                Text = task.Description ?? "Нет описания",
                TextWrapping = TextWrapping.Wrap,
                FontSize = 40,
                FontFamily = new FontFamily("Inter Medium"),
                TextTrimming = TextTrimming.CharacterEllipsis,
                MaxHeight = 143,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(description, 0);
            Grid.SetRowSpan(description, 2);
            grid.Children.Add(description);

            // Создание Grid_Avatar
            var grid_avatar = new Grid();
            grid_avatar.ColumnDefinitions.Add(new ColumnDefinition 
            { 
                Width = GridLength.Auto 
            });
            grid_avatar.ColumnDefinitions.Add(new ColumnDefinition 
            { 
                Width = GridLength.Auto 
            });
            grid_avatar.ColumnDefinitions.Add(new ColumnDefinition 
            { 
                Width = GridLength.Auto 
            });
            grid_avatar.RowDefinitions.Add(new RowDefinition 
            { 
                Height = GridLength.Auto 
            });
            grid_avatar.RowDefinitions.Add(new RowDefinition 
            { 
                Height = GridLength.Auto 
            });
            grid_avatar.HorizontalAlignment = HorizontalAlignment.Center;
            grid_avatar.VerticalAlignment = VerticalAlignment.Bottom;

            // Блок проверки аватарок
            if (_users != null)
            {
                // Отправитель задачи
                if (!string.IsNullOrEmpty(task.SenderId) && _users.TryGetValue(task.SenderId, out var sender))
                {
                    var senderAvatar = new Image
                    {
                        Source = new BitmapImage(new Uri(sender.GravatarUrl)),
                        Width = 78.7,
                        Height = 78.7,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    // Круглый аватар
                    senderAvatar.Clip = new EllipseGeometry(new Point(senderAvatar.Width / 2, senderAvatar.Height / 2), senderAvatar.Width / 2, senderAvatar.Height / 2);

                    // Добавьте никнейм отправителя под аватаром
                    var senderName = new TextBlock
                    {
                        Text = sender.Username,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0),
                        FontSize = 24,
                        FontFamily = new FontFamily("Inter Medium"),
                        TextTrimming = TextTrimming.CharacterEllipsis,
                        MaxWidth = 112
                    };

                    Grid.SetColumn(senderAvatar, 0);
                    Grid.SetRow(senderAvatar, 0);
                    grid_avatar.Children.Add(senderAvatar);

                    Grid.SetColumn(senderName, 0);
                    Grid.SetRow(senderName, 1);
                    grid_avatar.Children.Add(senderName);
                }

                // Текст стрелки
                var arrow = new TextBlock
                {
                    Text = "→",
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 64,
                    Margin = new Thickness(20, 0, 20, 0),
                    FontFamily = new FontFamily("Inter Medium"),
                };

                Grid.SetColumn(arrow, 1);
                Grid.SetRow(arrow, 0);
                grid_avatar.Children.Add(arrow);

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

                    // Круглый аватар
                    receiverAvatar.Clip = new EllipseGeometry(new Point(receiverAvatar.Width / 2, receiverAvatar.Height / 2), receiverAvatar.Width / 2, receiverAvatar.Height / 2);

                    // Добавьте никнейм получателя под аватаром
                    var receiverName = new TextBlock
                    {
                        Text = receiver.Username,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0),
                        FontSize = 24,
                        FontFamily = new FontFamily("Inter Medium"),
                        TextTrimming = TextTrimming.CharacterEllipsis,
                        MaxWidth = 112
                    };

                    Grid.SetColumn(receiverAvatar, 2);
                    Grid.SetRow(receiverAvatar, 0);
                    grid_avatar.Children.Add(receiverAvatar);

                    Grid.SetColumn(receiverName, 2);
                    Grid.SetRow(receiverName, 1);
                    grid_avatar.Children.Add(receiverName);
                }
            }

            Grid.SetRow(grid_avatar, 2);
            grid.Children.Add(grid_avatar);

            border.Child = grid;


            // Контекстное меню для "Добавленной задачи"
            border.ContextMenu = new ContextMenu();

            // Добавьте пункт "Отправить на проверку"
            var sendForReviewMenuItem = new MenuItem
            {
                Header = "Отправить на проверку",
                IsEnabled = false,
                //Command = new RelayCommand(param => SendTaskForReview(task))
            };
            border.ContextMenu.Items.Add(sendForReviewMenuItem);

            // Добавьте разделитель
            border.ContextMenu.Items.Add(new Separator());

            // Добавьте пункт "Редактировать" (неактивный)
            var editMenuItem = new MenuItem
            {
                Header = "Редактировать",
                IsEnabled = false
            };
            border.ContextMenu.Items.Add(editMenuItem);

            // Добавьте пункт "Удалить" (неактивный)
            var deleteMenuItem = new MenuItem
            {
                Header = "Удалить",
                IsEnabled = false
            };
            border.ContextMenu.Items.Add(deleteMenuItem);

            // Добавление обработчика события MouseLeftButtonDown
            border.MouseLeftButtonDown += (sender, e) =>
            {
                bool isExpanded = border.Tag != null && (bool)border.Tag;

                if (isExpanded)
                {
                    AnimateTaskBorder(border, 381, 384);
                }
                else
                {
                    AnimateTaskBorder(border, 381, 768);
                }

                border.Tag = !isExpanded;
            };

            return border;
        }

        private void AnimateTaskBorder(Border border, double newWidth, double newHeight)
        {
            var widthAnimation = new DoubleAnimation
            {
                To = newWidth,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            var heightAnimation = new DoubleAnimation
            {
                To = newHeight,
                Duration = TimeSpan.FromSeconds(0.5)
            };

            // Устанавливаем начальные значения для анимации
            border.Width = border.ActualWidth;
            border.Height = border.ActualHeight;

            border.BeginAnimation(WidthProperty, widthAnimation);
            border.BeginAnimation(HeightProperty, heightAnimation);

            // Анимация для внутренних элементов, чтобы все содержимое стало видимым
            var grid = border.Child as Grid;
            if (grid != null)
            {
                foreach (var child in grid.Children)
                {
                    if (child is TextBlock textBlock)
                    {
                        var maxHeightAnimation = new DoubleAnimation
                        {
                            To = newHeight - 100, // Учитываем отступы и другие элементы внутри
                            Duration = TimeSpan.FromSeconds(0.5)
                        };
                        textBlock.BeginAnimation(TextBlock.MaxHeightProperty, maxHeightAnimation);
                    }
                }
            }
        }

        // Метод для обработки команды "Отправить на проверку"
        private void SendTaskForReview(TaskModel task)
        {
            // Добавьте здесь код для отправки задачи на проверку
        }
    }
}
