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

namespace DevTask.View.WorkingField.Page_TaskTransfer
{
    public partial class Page_TaskTransfer : Page
    {
        public Page_TaskTransfer()
        {
            InitializeComponent();
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
                Filter = "Image files (*.png;*.jpg)|*.png;*.jpg",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFileName = openFileDialog.FileName;
                BitmapImage bitmap = new BitmapImage(new Uri(selectedFileName));

                // Создаем новое изображение и текстовый блок для названия
                Image newImage = new Image
                {
                    Source = bitmap,
                    Width = bitmap.PixelWidth,
                    Height = bitmap.PixelHeight,
                    Margin = new Thickness(10)
                };

                string fileName = System.IO.Path.GetFileName(selectedFileName);
                TextBlock newTextBlock = new TextBlock
                {
                    Text = fileName.Length > 30 ? fileName.Substring(0, 30) + "..." : fileName,
                    MaxWidth = bitmap.PixelWidth,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(10),
                    Foreground = Brushes.White,
                    FontSize = 25
                };

                // Добавляем их в StackPanel перед кнопкой
                ImagesPanel.Children.Insert(ImagesPanel.Children.Count - 1, newImage);
                ImagesPanel.Children.Insert(ImagesPanel.Children.Count - 1, newTextBlock);
            }
        }
    }
}
