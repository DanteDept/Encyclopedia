using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace FairyTaleEncyclopedia
{
    /// <summary>
    /// Логика взаимодействия для EditWriterWindow.xaml
    /// </summary>
    public partial class EditWriterWindow : Window
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public string CountryName { get; set; }
        public string Biography { get; set; }
        public byte[] PhotoData { get; set; }

        public EditWriterWindow(int writerID, string firstName, string lastName, string patronymic, DateTime? birthDate,
                                DateTime? deathDate, string countryName, string biography, byte[] photoData)
        {
            InitializeComponent();

            // Заполняем поля окна редактирования
            FirstNameBox.Text = firstName;
            LastNameBox.Text = lastName;
            PatronymicBox.Text = patronymic;
            BirthDatePicker.SelectedDate = birthDate;
            DeathDatePicker.SelectedDate = deathDate;
            CountryNameBox.Text = countryName;
            BiographyBox.Text = biography;

            // Если есть фото, отображаем его
            if (photoData != null)
            {
                PhotoData = photoData;
                // Логика для отображения изображения
            }
        }

        // Здесь реализуем логику сохранения данных в свойства окна (например, при нажатии на кнопку "Сохранить")
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            FirstName = FirstNameBox.Text;
            LastName = LastNameBox.Text;
            Patronymic = PatronymicBox.Text;
            BirthDate = BirthDatePicker.SelectedDate;
            DeathDate = DeathDatePicker.SelectedDate;
            CountryName = CountryNameBox.Text;
            Biography = BiographyBox.Text;

            // Логика для обработки фото
            // Например, если есть новое фото, сохраняем его в PhotoData

            this.DialogResult = true;
            this.Close();
        }

        // Логика для загрузки фотографии
        private void UploadPhoto_Click(object sender, RoutedEventArgs e)
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



        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }


}
