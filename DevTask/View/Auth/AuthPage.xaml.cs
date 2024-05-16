using DevTask.View.Registration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DevTask.View.Auth
{
    public partial class AuthPage : Page
    {
        private const string FirebaseAppUri = "https://devtaskdb-default-rtdb.europe-west1.firebasedatabase.app/";
        private FirebaseClient _client;
        private Frame _mainFrame;

        public AuthPage(Frame mainFrame)
        {
            InitializeComponent();
            _client = new FirebaseClient(FirebaseAppUri);
            _mainFrame = mainFrame;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordTextBox.Password;

            // Получение списка всех пользователей
            var users = await _client
                .Child("Users")
                .OnceAsync<dynamic>();

            // Проверка наличия пользователя с таким именем и паролем
            if (users.Any(user => user.Object.Username == username && user.Object.Password == password))
            {
                // Обработка ситуации, когда пользователь с таким именем и паролем найден
                CustomDialog.CustomDialog.Show("Вы успешно вошли в систему!", Brushes.Green);
            }
            else
            {
                // Обработка ситуации, когда пользователь с таким именем и паролем не найден
                CustomDialog.CustomDialog.Show("Неверное имя пользователя или пароль!", Brushes.Red);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.Content = new RegistrationPage(_mainFrame);
        }

        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            CustomDialog.CustomDialog.Show("Фифу тебе, а не восстановление пароля)", Brushes.Green);
        }
    }
}
