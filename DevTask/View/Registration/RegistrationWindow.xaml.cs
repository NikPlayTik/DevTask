using DevTask.View.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DevTask.View.Registration
{
    public partial class RegistrationWindow : Window
    {
        private const string FirebaseAppUri = "https://devtaskdb-default-rtdb.europe-west1.firebasedatabase.app/";
        private FirebaseClient _client;

        public RegistrationWindow()
        {
            InitializeComponent();
            _client = new FirebaseClient(FirebaseAppUri);
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var email = EmailTextBox.Text;
            var password = PasswordTextBox.Password;

            // Валидация электронной почты
            var emailRegex = new Regex(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$");
            if (!emailRegex.IsMatch(email))
            {
                // Обработка неверного адреса электронной почты
                CustomDialog.CustomDialog.Show("Неверный адрес электронной почты!", Brushes.Red);
                return;
            }

            // Валидация пароля: как минимум две цифры и символы
            var passwordRegex = new Regex(@"^(?=.*\d{2,})(?=.*[a-zA-Z]).+$");
            if (!passwordRegex.IsMatch(password))
            {
                // Обработка неверного пароля
                CustomDialog.CustomDialog.Show("Пароль должен содержать как минимум две цифры и символы!", Brushes.Red);
                return;
            }

            var user = new
            {
                Username = username,
                Email = email,
                Password = password
            };

            await _client
                .Child("Users")
                .PostAsync(user);

            // Сообщение об успешной регистрации
            CustomDialog.CustomDialog.Show("Вы успешно зарегистрировались!", Brushes.Green);
        }

        // Добавить переход в окно
        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            // Создание нового окна входа
            AuthPage authPage = new AuthPage();

            // Создание нового окна и установка AuthPage как его содержимого
            Window window = new Window
            {
                Content = authPage
            };

            // Отображение нового окна
            window.Show();

            // Закрытие текущего окна
            this.Close();
        }
    }
}
