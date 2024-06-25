using Microsoft.Win32;
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
using DevTask.View.WorkingField.Page_TaskTransfer;
using DevTask.Model.ClassTask;
using Firebase.Database;
using Firebase.Database.Query;
using DevTask.Model.ClassProject;
using Firebase.Storage;
using System.IO;

namespace DevTask.View.WorkingField.Page_TaskTransfer
{
    public partial class Page_TaskTransfer : Page
    {
        private string _currentUserId;
        private string _currentProjectId;
        private string _selectedImagePath;

        public Page_TaskTransfer(string currentUserId, string currentProjectId)
        {
            InitializeComponent();
            _currentUserId = currentUserId;
            _currentProjectId = currentProjectId;
            LoadUsers();
        }

        private async void LoadUsers()
        {
            var firebaseClient = new FirebaseClient("https://devtaskdb-default-rtdb.europe-west1.firebasedatabase.app/");
            var project = await firebaseClient.Child("Projects").Child(_currentProjectId).OnceSingleAsync<Project>();
            var users = await firebaseClient.Child("Users").OnceAsync<Model.ClassUser.User>();

            var filteredUsers = users.Where(u => project.Members.Contains(u.Key)).Select(u => u.Object.Username).ToList();

            TaskTransferComboBox.ItemsSource = filteredUsers;
        }

        private void TaskDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TaskDatePicker.SelectedDate.HasValue)
            {
                TaskDatePicker.Text = TaskDatePicker.SelectedDate.Value.ToString("dd.MM.yyyy");
            }
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Выберите изображение (*.png;*.jpg)|*.png;*.jpg",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImagePath = openFileDialog.FileName;
                BitmapImage bitmap = new BitmapImage(new Uri(_selectedImagePath, UriKind.RelativeOrAbsolute));

                Image newImage = new Image
                {
                    Source = bitmap,
                    MaxWidth = 1000,
                    MaxHeight = 1000,
                    Stretch = Stretch.UniformToFill
                };

                Border imageBorder = new Border
                {
                    Width = 1000,
                    Height = 1000,
                    CornerRadius = new CornerRadius(20),
                    ClipToBounds = true,
                    Margin = new Thickness(10),
                    Child = newImage
                };

                RectangleGeometry clipGeometry = new RectangleGeometry(new Rect(0, 0, 1000, 1000), 20, 20);
                newImage.Clip = clipGeometry;

                imageBorder.PreviewMouseWheel += ImagesPanel_PreviewMouseWheel;

                string fileName = System.IO.Path.GetFileName(_selectedImagePath);
                TextBlock newTextBlock = new TextBlock
                {
                    Text = fileName.Length > 30 ? fileName.Substring(0, 30) + "..." : fileName,
                    MaxWidth = 998,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(10),
                    Foreground = Brushes.White,
                    FontSize = 25
                };
                newTextBlock.PreviewMouseWheel += ImagesPanel_PreviewMouseWheel;

                ImagesPanel.Children.Insert(ImagesPanel.Children.Count - 1, imageBorder);
                ImagesPanel.Children.Insert(ImagesPanel.Children.Count - 1, newTextBlock);
            }
        }

        private void ImagesPanel_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void DescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                int lineCount = textBox.LineCount;
                double lineHeight = textBox.FontSize + textBox.FontFamily.LineSpacing;
                double newHeight = lineCount * lineHeight;

                textBox.Height = Math.Min(newHeight, 570);
            }
        }

        private async void SaveTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskTransferComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для передачи задачи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedUsername = (string)TaskTransferComboBox.SelectedItem;
            var firebaseClient = new FirebaseClient("https://devtaskdb-default-rtdb.europe-west1.firebasedatabase.app/");
            var users = await firebaseClient.Child("Users").OnceAsync<Model.ClassUser.User>();
            var selectedUser = users.FirstOrDefault(u => u.Object.Username == selectedUsername);
            string selectedUserId = selectedUser?.Key;


            string description = DescriptionTextBox.Text;
            DateTime? dueDate = TaskDatePicker.SelectedDate;

            // Загрузка изображения в Firebase Storage
            string imageUrl = await UploadImageToFirebaseStorage(_selectedImagePath);

            var newTask = new TaskModel
            {
                Description = description,
                DueDate = dueDate,
                SenderId = _currentUserId,
                ReceiverId = selectedUserId,
                ProjectId = _currentProjectId,
                ImageUrl = imageUrl
            };

            //var firebaseClient = new FirebaseClient("https://devtaskdb-default-rtdb.europe-west1.firebasedatabase.app/");
            await firebaseClient.Child("Tasks").PostAsync(newTask);

            MessageBox.Show("Задача успешно передана.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task<string> UploadImageToFirebaseStorage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                MessageBox.Show("Пожалуйста, выберите изображение.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            var storage = new FirebaseStorage("devtaskdb.appspot.com");
            var fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(imagePath);

            // Использование MemoryStream для загрузки изображения
            using (var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var task = storage.Child("images").Child(fileName).PutAsync(memoryStream);

                // Ожидание завершения загрузки
                var downloadUrl = await task;

                return downloadUrl;
            }
        }
    }
}
