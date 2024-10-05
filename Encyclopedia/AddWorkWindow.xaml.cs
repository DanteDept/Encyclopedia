using System;
using System.Collections.Generic;
using System.Windows;
using MySql.Data.MySqlClient;

namespace FairyTaleEncyclopedia
{
    public partial class AddWorkWindow : Window
    {
        public string Title { get; set; }
        public string GenreName { get; set; }
        public int? YearOfPublication { get; set; }
        public string Description { get; set; }
        public int WriterID { get; set; }

        public AddWorkWindow(int writerId)
        {
            InitializeComponent();
            WriterID = writerId;
            LoadGenres();
            LoadWriters();
        }

        // Загрузка списка жанров из базы данных
        private void LoadGenres()
        {
            List<string> genres = new List<string>();
            string connectionString = "server=localhost;user=root;database=FairyTaleEncyclopedia;password=;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("SELECT GenreName FROM Genres", connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            genres.Add(reader.GetString("GenreName"));
                        }
                    }

                    GenreComboBox.ItemsSource = genres;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки жанров: " + ex.Message);
                }
            }
        }

        // Загрузка списка писателей из базы данных
        private void LoadWriters()
        {
            Dictionary<int, string> writers = new Dictionary<int, string>();
            string connectionString = "server=localhost;user=root;database=FairyTaleEncyclopedia;password=;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("SELECT WriterID, CONCAT(FirstName, ' ', LastName) AS FullName FROM Writers", connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int writerId = reader.GetInt32("WriterID");
                            string fullName = reader.GetString("FullName");
                            writers.Add(writerId, fullName);
                        }
                    }

                    WriterComboBox.ItemsSource = writers;
                    WriterComboBox.DisplayMemberPath = "Value";
                    WriterComboBox.SelectedValuePath = "Key"; 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки писателей: " + ex.Message);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Title = TitleBox.Text;
            GenreName = GenreComboBox.Text;
            if (int.TryParse(YearBox.Text, out int year))
            {
                YearOfPublication = year;
            }
            // Получаем WriterID из выбранного элемента ComboBox
            if (WriterComboBox.SelectedValue != null)
            {
                WriterID = (int)WriterComboBox.SelectedValue;
            }
            Description = DescriptionBox.Text;

            if (!string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(GenreName) && WriterID > 0)
            {
                if (AddWorkToDatabase())
                {
                    MessageBox.Show("Произведение добавлено успешно!");
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Произведение не удалось добавить.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
            }
        }

        private bool AddWorkToDatabase()
        {
            string connectionString = "server=localhost;user=root;database=FairyTaleEncyclopedia;password=;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("INSERT INTO Works (Title, GenreName, YearOfPublication, Description, WriterID) " +
                                                            "VALUES (@Title, @GenreName, @YearOfPublication, @Description, @WriterID)", connection);
                    command.Parameters.AddWithValue("@Title", Title);
                    command.Parameters.AddWithValue("@GenreName", GenreName);
                    command.Parameters.AddWithValue("@YearOfPublication", YearOfPublication.HasValue ? (object)YearOfPublication.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Description", Description);
                    command.Parameters.AddWithValue("@WriterID", WriterID);

                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении произведения: " + ex.Message);
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
