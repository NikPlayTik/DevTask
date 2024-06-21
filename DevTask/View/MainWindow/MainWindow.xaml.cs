using DevTask.View.Auth;
using DevTask.View.Registration;
using Firebase.Database;
using Firebase.Database.Query;
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

namespace DevTask.View.MainWindow
{
    public partial class MainWindow : Window
    {
        private const double AspectRatio = 16.0 / 9.0;
        private bool _isResizing = false;

        public MainWindow()
        {
            InitializeComponent();

            // Проверка, есть ли сохраненный логин
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Username))
            {
                // Получение текущего идентификатора пользователя из настроек
                string currentUserId = Properties.Settings.Default.CurrentUserId;

                // Загрузка данных пользователя и текущего проекта из Firebase
                LoadUserData(currentUserId);
            }
            else
            {
                // Перенаправление на страницу регистрации, если пользователь не вошел
                MainFrame.Content = new RegistrationPage(MainFrame);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_isResizing) return;

            _isResizing = true;

            if (e.WidthChanged)
            {
                double newHeight = e.NewSize.Width / AspectRatio;
                if (newHeight >= MinHeight)
                {
                    Height = newHeight;
                }
                else
                {
                    Width = MinHeight * AspectRatio;
                }
            }
            else if (e.HeightChanged)
            {
                double newWidth = e.NewSize.Height * AspectRatio;
                if (newWidth >= MinWidth)
                {
                    Width = newWidth;
                }
                else
                {
                    Height = MinWidth / AspectRatio;
                }
            }

            _isResizing = false;
        }
        private async void LoadUserData(string currentUserId)
        {
            var firebaseClient = new FirebaseClient("https://devtaskdb-default-rtdb.europe-west1.firebasedatabase.app/");

            // Получение данных пользователя
            var user = await firebaseClient
                .Child("Users")
                .Child(currentUserId)
                .OnceSingleAsync<Model.ClassUser.User>();

            if (user == null)
            {
                CustomDialog.CustomDialog.Show("Не удалось загрузить данные пользователя.", Brushes.Red);
                MainFrame.Content = new AuthPage(MainFrame);
                return;
            }

            // Получение идентификатора текущего проекта для пользователя
            var userProjects = await firebaseClient
                .Child("UserProjects")
                .Child(currentUserId)
                .OnceAsync<dynamic>();

            string currentProjectId = userProjects.FirstOrDefault()?.Key;

            if (string.IsNullOrEmpty(currentProjectId))
            {
                CustomDialog.CustomDialog.Show("Не удалось найти текущий проект пользователя.", Brushes.Red);
                MainFrame.Content = new AuthPage(MainFrame);
                return;
            }

            // Переход на рабочее поле
            var workingFieldPage = new WorkingField.WorkingField(MainFrame, currentUserId, currentProjectId);
            MainFrame.Content = workingFieldPage;

            // Передача данных пользователя
            workingFieldPage.ShowUserDetails(user.Username, user.GravatarUrl);
        }
    }
}
