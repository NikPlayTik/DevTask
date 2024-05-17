using DevTask.View.Registration;
using DevTask.View.WorkingField;
using Firebase.Database;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

                // Получение имени пользователя из Firebase
                var firebaseUser = users.FirstOrDefault(user => user.Object.Username == username);
                string firebaseUsername = firebaseUser?.Object.Username;

                // Передача имени пользователя на страницу WorkingField для отображения
                (_mainFrame.Content as WorkingField.WorkingField)?.ShowUsername(firebaseUsername);
                _mainFrame.Content = new WorkingField.WorkingField(_mainFrame);

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
