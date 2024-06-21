using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevTask.Model.ClassTask
{
    public class TaskModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string SenderAvatar { get; set; }
        public string ReceiverAvatar { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string ProjectId { get; set; }
        public DateTime? DueDate { get; set; }

        private static FirebaseClient firebaseClient = new FirebaseClient("https://devtaskdb-default-rtdb.europe-west1.firebasedatabase.app/");

        public static async Task<List<TaskModel>> GetTasksAsync()
        {
            var tasks = await firebaseClient
                .Child("Tasks")
                .OnceAsync<TaskModel>();

            return tasks.Select(item => new TaskModel
            {
                Id = item.Key,
                Description = item.Object.Description,
                ImageUrl = item.Object.ImageUrl,
                SenderAvatar = item.Object.SenderAvatar,
                ReceiverAvatar = item.Object.ReceiverAvatar,
                SenderId = item.Object.SenderId,
                ReceiverId = item.Object.ReceiverId,
                DueDate = item.Object.DueDate
            }).ToList();
        }

        public static async Task AddTaskAsync(TaskModel task)
        {
            await firebaseClient
                .Child("Tasks")
                .PostAsync(task);
        }
    }
}
