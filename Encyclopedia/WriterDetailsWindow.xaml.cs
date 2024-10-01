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
using System.Windows.Shapes;

namespace Encyclopedia
{
    /// <summary>
    /// Логика взаимодействия для WriterDetailsWindow.xaml
    /// </summary>
    public partial class WriterDetailsWindow : Window
    {
        public WriterDetailsWindow()
        {
            InitializeComponent();
        }

        // Метод для заполнения окна данными писателя
        public void SetWriterDetails(string firstName, string lastName, string patronymic, DateTime? birthDate, DateTime? deathDate, string countryName, string biography, byte[] imageData)
        {
            FirstNameText.Text = firstName;
            LastNameText.Text = lastName;
            PatronymicText.Text = string.IsNullOrEmpty(patronymic) ? "—" : patronymic;
            BirthDateText.Text = birthDate.HasValue ? birthDate.Value.ToString("dd.MM.yyyy") : "—";
            DeathDateText.Text = deathDate.HasValue ? deathDate.Value.ToString("dd.MM.yyyy") : "—";
            CountryNameText.Text = string.IsNullOrEmpty(countryName) ? "—" : countryName;
            BiographyText.Text = string.IsNullOrEmpty(biography) ? "—" : biography;

            // Если есть изображение, отображаем его
            if (imageData != null && imageData.Length > 0)
            {
                using (var ms = new System.IO.MemoryStream(imageData))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    WriterImage.Source = image;
                }
            }
            else
            {
                WriterImage.Source = null;  // Нет изображения
            }
        }
    }

}
