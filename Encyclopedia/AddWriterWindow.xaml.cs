using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FairyTaleEncyclopedia
{
    public partial class AddWriterWindow : Window
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public string CountryName { get; set; }
        public string Biography { get; set; }
        public byte[] PhotoData { get; set; }


        public AddWriterWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            FirstName = FirstNameBox.Text;
            LastName = LastNameBox.Text;
            Patronymic = PatronymicBox.Text;
            BirthDate = BirthDatePicker.SelectedDate;
            DeathDate = DeathDatePicker.SelectedDate;
            CountryName = CountryNameBox.Text;
            Biography = BiographyBox.Text;

            if (!string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName))
            {
                DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните обязательные поля.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        // Логика для загрузки фотографии
        private void UploadPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (openFileDialog.ShowDialog() == true)
            {
                // Чтение выбранного файла изображения
                PhotoData = File.ReadAllBytes(openFileDialog.FileName);

                // Отображение изображения в интерфейсе
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(PhotoData);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                PhotoPreview.Source = bitmap;
            }
        }
    }
}
