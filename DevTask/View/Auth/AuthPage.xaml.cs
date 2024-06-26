﻿using DevTask.View.Registration;
using DevTask.View.WorkingField;
using Firebase.Database;
using Firebase.Database.Query;
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
            var firebaseUser = users.FirstOrDefault(user => user.Object.Username == username && user.Object.Password == password);

            // В методе LoginButton_Click, после успешной аутентификации
            if (firebaseUser != null)
            {
                string firebaseUsername = firebaseUser.Object.Username;
                string gravatarUrl = firebaseUser.Object.GravatarUrl;

                // Сохранение логина в Settings
                Properties.Settings.Default.Username = firebaseUsername;
                Properties.Settings.Default.Save();

                // Получение текущего пользователя
                string currentUserId = firebaseUser.Key;

                // Получение currentProjectId для текущего пользователя
                var userProjects = await _client
                    .Child("UserProjects")
                    .Child(currentUserId)
                    .OnceAsync<dynamic>();

                string currentProjectId = userProjects.FirstOrDefault()?.Key;

                if (string.IsNullOrEmpty(currentProjectId))
                {
                    CustomDialog.CustomDialog.Show("Не удалось найти текущий проект пользователя.", Brushes.Red);
                    return;
                }

                var workingFieldPage = new WorkingField.WorkingField(_mainFrame, currentUserId, currentProjectId);
                _mainFrame.Content = workingFieldPage;
                workingFieldPage.ShowUserDetails(firebaseUsername, gravatarUrl);
            }
            else
            {
                CustomDialog.CustomDialog.Show("Неверное имя пользователя или пароль!", Brushes.Red);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.Content = new RegistrationPage(_mainFrame);
        }

        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            CustomDialog.CustomDialog.Show("Нет восстановления пароля)", Brushes.Green);
        }
    }
}
