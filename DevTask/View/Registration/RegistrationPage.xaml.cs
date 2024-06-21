using DevTask.Model.ClassProject;
using DevTask.View.Auth;
using DevTask.View.WorkingField;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevTask.View.Registration
{
    public partial class RegistrationPage : Page
    {
        private const string FirebaseAppUri = "https://devtaskdb-default-rtdb.europe-west1.firebasedatabase.app/";
        private FirebaseClient _client;
        private Frame _mainFrame;

        public RegistrationPage(Frame mainFrame)
        {
            InitializeComponent();
            _client = new FirebaseClient(FirebaseAppUri);
            _mainFrame = mainFrame;
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
                CustomDialog.CustomDialog.Show("Неверный адрес электронной почты!", Brushes.Red);
                return;
            }

            // Валидация пароля: как минимум две цифры и символы
            var passwordRegex = new Regex(@"^(?=.*\d{2,})(?=.*[a-zA-Z]).+$");
            if (!passwordRegex.IsMatch(password))
            {
                CustomDialog.CustomDialog.Show("Пароль должен содержать как минимум две цифры и символы!", Brushes.Red);
                return;
            }

            // Получение списка всех пользователей
            var users = await _client.Child("Users").OnceAsync<dynamic>();

            // Проверка наличия пользователя с таким именем или почтой
            if (users.Any(user => user.Object.Username == username || user.Object.Email == email))
            {
                CustomDialog.CustomDialog.Show("Пользователь с таким именем или почтой уже существует!", Brushes.Red);
                return;
            }

            // Получение первой буквы имени пользователя
            var profileLetter = username.FirstOrDefault().ToString();

            // Создание хэша MD5 от электронной почты для получения аватарки Gravatar
            var emailHash = CreateMD5(email.Trim().ToLower());
            var gravatarUrl = $"https://www.gravatar.com/avatar/{emailHash}?s=200&d=identicon";

            // Создание нового пользователя с использованием класса User
            var newUser = new Model.ClassUser.User
            {
                Username = username,
                Email = email,
                Password = password,
                ProfileLetter = profileLetter,
                GravatarUrl = gravatarUrl
            };

            // Регистрация нового пользователя и получение уникального ключа Firebase
            var firebaseObject = await _client.Child("Users").PostAsync(newUser);

            // Установка Id пользователя равным уникальному ключу Firebase
            newUser.Id = firebaseObject.Key;

            // Получение уникального ключа Firebase
            string currentUserId = firebaseObject.Key;

            // Обновление пользователя с новым Id
            await _client.Child("Users").Child(firebaseObject.Key).PutAsync(newUser);

            // Создание нового проекта для пользователя
            var newProject = new Project
            {
                Name = "Default Project",
                AdminId = currentUserId,
                Members = new List<string> { currentUserId }
            };

            var projectObject = await _client.Child("Projects").PostAsync(newProject);
            string currentProjectId = projectObject.Key;

            // Добавление проекта пользователю
            await _client.Child("UserProjects").Child(currentUserId).Child(currentProjectId).PutAsync(true);

            // Сообщение об успешной регистрации
            CustomDialog.CustomDialog.Show("Вы успешно зарегистрировались!", Brushes.Green);

            // Перенаправление пользователя на рабочее поле и отображение данных пользователя
            var workingFieldPage = new WorkingField.WorkingField(_mainFrame, currentUserId, currentProjectId);
            _mainFrame.Content = workingFieldPage;
            workingFieldPage.ShowUserDetails(username, gravatarUrl);
        }

        // Функция создания MD5 хэша
        public static string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.Content = new AuthPage(_mainFrame);
        }
    }
}
