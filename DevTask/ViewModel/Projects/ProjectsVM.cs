using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using DevTask.Model.ClassProject;
using DevTask.Model.ClassUser;
using Firebase.Database;
using Firebase.Database.Query;

namespace DevTask.ViewModel.Projects
{
    public class ProjectsVM : INotifyPropertyChanged
    {
        private const string FirebaseAppUri = "https://devtaskdb-default-rtdb.europe-west1.firebasedatabase.app/";
        private FirebaseClient _client;

        public ObservableCollection<Project> Projects { get; private set; }
        public string CurrentUserId { get; set; }

        public ProjectsVM()
        {
            _client = new FirebaseClient(FirebaseAppUri);
            Projects = new ObservableCollection<Project>();
        }

        public async void LoadProjects(string currentUserId)
        {
            Projects.Clear();

            var user = await _client.Child("Users").Child(currentUserId).OnceSingleAsync<User>();
            var projects = await _client.Child("Projects").OnceAsync<Project>();

            Projects.Add(new Project 
            { 
                Name = "Создать проект", 
                Id = "create_project" 
            });

            foreach (var project in projects)
            {
                if (user.Projects.Contains(project.Object.Id))
                {
                    Projects.Add(project.Object);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
