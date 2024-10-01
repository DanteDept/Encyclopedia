using System;
using System.Windows;

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
    }
}
