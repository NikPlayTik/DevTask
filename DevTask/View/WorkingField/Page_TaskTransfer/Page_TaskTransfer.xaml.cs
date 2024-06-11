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
                Filter = "Выберите изображение (*.png;*.jpg)|*.png;*.jpg",
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
                    MaxWidth = 1000,  // Устанавливаем максимальную ширину
                    MaxHeight = 1000, // Устанавливаем максимальную высоту
                    Stretch = Stretch.UniformToFill
                };

                // Создаем Border для закругления краев изображения
                Border imageBorder = new Border
                {
                    Width = 1000,
                    Height = 1000,
                    CornerRadius = new CornerRadius(20),
                    ClipToBounds = true,
                    Margin = new Thickness(10),
                    Child = newImage
                };

                // Создаем закругленный Clip для изображения
                RectangleGeometry clipGeometry = new RectangleGeometry(new Rect(0, 0, 1000, 1000), 20, 20);
                newImage.Clip = clipGeometry;

                imageBorder.PreviewMouseWheel += ImagesPanel_PreviewMouseWheel; // Подписываем новый элемент на событие PreviewMouseWheel

                string fileName = System.IO.Path.GetFileName(selectedFileName);
                TextBlock newTextBlock = new TextBlock
                {
                    Text = fileName.Length > 30 ? fileName.Substring(0, 30) + "..." : fileName,
                    MaxWidth = 998,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(10),
                    Foreground = Brushes.White,
                    FontSize = 25
                };
                newTextBlock.PreviewMouseWheel += ImagesPanel_PreviewMouseWheel; // Подписываем TextBlock на событие PreviewMouseWheel

                // Добавляем их в StackPanel перед кнопкой
                ImagesPanel.Children.Insert(ImagesPanel.Children.Count - 1, imageBorder);
                ImagesPanel.Children.Insert(ImagesPanel.Children.Count - 1, newTextBlock);
            }
        }

        private void ImagesPanel_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
