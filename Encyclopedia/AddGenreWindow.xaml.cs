using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace FairyTaleEncyclopedia
{
    public partial class AddGenreWindow : Window
    {
        public string GenreName { get; set; }
        public string Description { get; set; }

        public AddGenreWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            GenreName = GenreNameBox.Text;
            Description = DescriptionBox.Text;

            if (!string.IsNullOrEmpty(GenreName))
            {
                if (AddGenreToDatabase())
                {
                    MessageBox.Show("Жанр добавлен успешно!");
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось добавить жанр.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите название жанра.");
            }
        }

        private bool AddGenreToDatabase()
        {
            string connectionString = "server=localhost;user=root;database=FairyTaleEncyclopedia;password=;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("INSERT INTO Genres (GenreName), (Description) VALUES (@GenreName), (@Description)", connection);
                    command.Parameters.AddWithValue("@GenreName", GenreName);
                    command.Parameters.AddWithValue("@Description", Description);

                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении жанра: " + ex.Message);
                    return false;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
